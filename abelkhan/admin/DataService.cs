using consts;
using core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace admin;

/// <summary>
/// 全局邮件的领取条件（0 表示不限制）。
/// 等级/建角天数范围、是否拍脸、发布后有效天数。
/// </summary>
public record GlobalMailRule(
    int MinLevel = 0,
    int MaxLevel = 0,
    int MinCreateDays = 0,
    int MaxCreateDays = 0,
    bool PopUp = false,
    int ValidDays = 0);

/// <summary>
/// 管理端数据访问：Redis（在线数、GM/邮件队列）+ MongoDB（玩家/支付/邮件/GM 日志）。
/// 字段名（注册时间、支付时间、支付金额）可在配置里覆盖，方便接入不同游戏的数据约定。
/// 时间字段同时兼容「Unix 毫秒（long）」和「BSON DateTime」两种存储。
/// </summary>
public class AdminDataService
{
    private readonly RedisHandle _redis;
    private readonly MongoClient _mongo;
    private readonly AdminConfig _cfg;
    private readonly IMongoDatabase _db;

    public AdminDataService(AdminConfig cfg)
    {
        _cfg = cfg;
        _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
        _mongo = new MongoClient(cfg.MongoUrl);
        _db = _mongo.GetDatabase(cfg.MongoDb);
    }

    private IMongoCollection<BsonDocument> Col(string name) => _db.GetCollection<BsonDocument>(name);

    // ---------------- 在线数（来自各 gate 周期上报的 Redis key） ----------------

    public async Task<long> OnlineCount(string keyFormat)
    {
        var keys = await _redis.GetKeys(string.Format(keyFormat, "*"));
        long total = 0;
        foreach (var key in keys)
        {
            var value = await _redis.GetStrData(key);
            if (!value.IsNull && long.TryParse(value.ToString(), out var n))
            {
                total += n;
            }
        }
        return total;
    }

    // ---------------- 玩家 ----------------

    public Task<long> TotalPlayers() =>
        Col(_cfg.PlayerCollection).CountDocumentsAsync(new BsonDocument());

    public async Task<long> CountPlayersByRegister(DateTime start, DateTime end)
    {
        var filter = RangeFilter(_cfg.PlayerRegisterField, start, end);
        return await Col(_cfg.PlayerCollection).CountDocumentsAsync(filter);
    }

    public async Task<(long Total, List<BsonDocument> Items)> QueryPlayers(string keyword, int skip, int limit)
    {
        var col = Col(_cfg.PlayerCollection);
        BsonDocument filter;
        if (string.IsNullOrWhiteSpace(keyword))
        {
            filter = new BsonDocument();
        }
        else
        {
            filter = new BsonDocument("$or", new BsonArray
            {
                new BsonDocument("EntityId", keyword),
                new BsonDocument("UserId", keyword),
                new BsonDocument("EntityId", new BsonDocument("$regex", keyword)),
                new BsonDocument("UserId", new BsonDocument("$regex", keyword)),
            });
        }

        var total = await col.CountDocumentsAsync(filter);
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument>
        {
            Skip = skip,
            Limit = limit,
            Sort = Builders<BsonDocument>.Sort.Descending("_id"),
        });
        return (total, await cursor.ToListAsync());
    }

    public async Task<BsonDocument?> GetPlayer(string id)
    {
        var col = Col(_cfg.PlayerCollection);
        var filter = new BsonDocument("$or", new BsonArray
        {
            new BsonDocument("EntityId", id),
            new BsonDocument("UserId", id),
        });
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument> { Limit = 1 });
        var items = await cursor.ToListAsync();
        return items.FirstOrDefault();
    }

    // ---------------- 支付 ----------------

    public async Task<long> SumPayments(DateTime start, DateTime end)
    {
        var col = Col(_cfg.PaymentCollection);
        var match = RangeFilter(_cfg.PaymentTimeField, start, end);
        var group = new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "total", new BsonDocument("$sum", "$" + _cfg.PaymentAmountField) },
        };
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", match),
            new BsonDocument("$group", group),
        };

        var cursor = await col.AggregateAsync<BsonDocument>(
            PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline));
        if (await cursor.MoveNextAsync() && cursor.Current != null && cursor.Current.Any())
        {
            var first = cursor.Current.First();
            if (first.TryGetValue("total", out var total) && total.IsNumeric)
            {
                return total.ToInt64();
            }
        }
        return 0;
    }

    public async Task<(long Total, List<BsonDocument> Items)> QueryPayments(
        string? userId, DateTime? start, DateTime? end, int skip, int limit)
    {
        var col = Col(_cfg.PaymentCollection);
        var filters = new List<BsonDocument>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            filters.Add(new BsonDocument("UserId", userId!));
        }
        if (start.HasValue && end.HasValue)
        {
            filters.Add(RangeFilter(_cfg.PaymentTimeField, start.Value, end.Value));
        }

        var filter = new BsonDocument();
        if (filters.Count > 0)
        {
            var and = new BsonArray();
            foreach (var f in filters)
            {
                and.Add(f);
            }
            filter = new BsonDocument("$and", and);
        }

        var total = await col.CountDocumentsAsync(filter);
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument>
        {
            Skip = skip,
            Limit = limit,
            Sort = Builders<BsonDocument>.Sort.Descending(_cfg.PaymentTimeField),
        });
        return (total, await cursor.ToListAsync());
    }

    // ---------------- 邮件 ----------------

    /// <summary>
    /// 发送邮件。playerIds 为空 = 全局邮件（所有玩家共用一份，存 Redis list），
    /// 否则给这批玩家逐个写 per-player 队列。
    /// </summary>
    public async Task SendMail(string title, string content, string[] playerIds, int expireSeconds,
        GlobalMailRule rule)
    {
        var isGlobal = playerIds.Length == 0;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var mailId = Guid.NewGuid().ToString("N");

        // 落库留档：一条记录，PlayerIds 为空数组表示全局邮件
        var ids = new BsonArray();
        foreach (var id in playerIds)
        {
            ids.Add(id);
        }
        var doc = new BsonDocument
        {
            { "MailId", mailId },
            { "Title", title },
            { "Content", content },
            { "PlayerIds", ids },
            { "CreateTime", now },
            { "ExpireSeconds", expireSeconds },
            { "ValidDays", rule.ValidDays },
            { "PopUp", rule.PopUp },
            { "MinLevel", rule.MinLevel },
            { "MaxLevel", rule.MaxLevel },
            { "MinCreateDays", rule.MinCreateDays },
            { "MaxCreateDays", rule.MaxCreateDays },
        };
        await Col(_cfg.MailCollection).InsertOneAsync(doc);

        if (isGlobal)
        {
            // 全局邮件只存一份，所有玩家共用。玩家请求邮件时由游戏服按频率读取该 list，
            // 再用 admin_mail_claimed_{playerId} 判断是否已领取（见 core.GlobalMailBox）。
            var mail = new Dictionary<string, object?>
            {
                { "mailId", mailId },
                { "title", title },
                { "content", content },
                { "createTime", now },
                { "expireSeconds", expireSeconds },
                { "validDays", rule.ValidDays },
                { "popUp", rule.PopUp },
                { "minLevel", rule.MinLevel },
                { "maxLevel", rule.MaxLevel },
                { "minCreateDays", rule.MinCreateDays },
                { "maxCreateDays", rule.MaxCreateDays },
            };
            await _redis.PushList(Consts.AdminMailGlobalListKey, mail);

            // 只保留最近 N 封，避免 list 无限增长
            var max = _cfg.MailGlobalMaxCount > 0 ? _cfg.MailGlobalMaxCount : 200;
            await _redis.LTrim(Consts.AdminMailGlobalListKey, -max, -1);
        }
        else
        {
            // 批量 / 单玩家：每个玩家一条，写入该玩家的队列，由游戏服按玩家消费。
            foreach (var id in playerIds)
            {
                var payload = new Dictionary<string, object?>
                {
                    { "mailId", mailId },
                    { "userId", id },
                    { "title", title },
                    { "content", content },
                    { "createTime", now },
                    { "expireSeconds", expireSeconds },
                };
                await _redis.PushList(string.Format(Consts.AdminMailQueueKey, id), payload);
            }
        }
    }

    public async Task<(long Total, List<BsonDocument> Items)> QueryMail(int skip, int limit)
    {
        var col = Col(_cfg.MailCollection);
        var filter = new BsonDocument();
        var total = await col.CountDocumentsAsync(filter);
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument>
        {
            Skip = skip,
            Limit = limit,
            Sort = Builders<BsonDocument>.Sort.Descending("CreateTime"),
        });
        return (total, await cursor.ToListAsync());
    }

    // ---------------- GM ----------------

    public async Task RecordGmCommand(string command, string playerId, string args, string result)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var doc = new BsonDocument
        {
            { "Command", command },
            { "PlayerId", playerId },
            { "Args", args },
            { "Result", result },
            { "CreateTime", now },
        };
        await Col(_cfg.GmCollection).InsertOneAsync(doc);
    }

    /// <summary>定向玩家 GM：写入该玩家的队列，由游戏服消费。</summary>
    public async Task PushPlayerGmCommand(string command, string playerId, string args)
    {
        var payload = new Dictionary<string, object?>
        {
            { "command", command },
            { "playerId", playerId },
            { "args", args },
            { "createTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
        };
        await _redis.PushList(string.Format(Consts.AdminGmQueueKey, playerId), payload);
    }

    public async Task<(long Total, List<BsonDocument> Items)> QueryGmLogs(int skip, int limit)
    {
        var col = Col(_cfg.GmCollection);
        var filter = new BsonDocument();
        var total = await col.CountDocumentsAsync(filter);
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument>
        {
            Skip = skip,
            Limit = limit,
            Sort = Builders<BsonDocument>.Sort.Descending("CreateTime"),
        });
        return (total, await cursor.ToListAsync());
    }

    // 时间范围过滤器：同时兼容 Unix 毫秒（long）与 BSON DateTime 两种存储
    private static BsonDocument RangeFilter(string field, DateTime start, DateTime end)
    {
        var startMs = new DateTimeOffset(start).ToUnixTimeMilliseconds();
        var endMs = new DateTimeOffset(end).ToUnixTimeMilliseconds();

        var byMs = new BsonDocument
        {
            { field, new BsonDocument { { "$gte", startMs }, { "$lt", endMs } } },
        };
        var byDate = new BsonDocument
        {
            { field, new BsonDocument { { "$gte", start }, { "$lt", end } } },
        };
        return new BsonDocument("$or", new BsonArray { byMs, byDate });
    }
}
