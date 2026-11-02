using consts;
using core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace admin;

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

    public async Task SendMail(string? userId, string title, string content, int expireSeconds)
    {
        var target = string.IsNullOrWhiteSpace(userId) ? Consts.AdminMailBroadcast : userId!;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var doc = new BsonDocument
        {
            { "UserId", target },
            { "Title", title },
            { "Content", content },
            { "CreateTime", now },
            { "ExpireSeconds", expireSeconds },
        };
        await Col(_cfg.MailCollection).InsertOneAsync(doc);

        // 游戏服邮件模块从这里消费：RPUSH 一条 JSON，key 见 Consts.AdminMailQueueKey
        var payload = new Dictionary<string, object?>
        {
            { "userId", target },
            { "title", title },
            { "content", content },
            { "createTime", now },
            { "expireSeconds", expireSeconds },
        };
        await _redis.PushList(string.Format(Consts.AdminMailQueueKey, target), payload);
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

    public async Task PushGmCommand(string command, string? target, string args)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var doc = new BsonDocument
        {
            { "Command", command },
            { "Target", string.IsNullOrWhiteSpace(target) ? string.Empty : target! },
            { "Args", args },
            { "CreateTime", now },
        };
        await Col(_cfg.GmCollection).InsertOneAsync(doc);

        // 游戏服 GM 模块从这里消费：RPUSH 一条 JSON，key 见 Consts.AdminGmQueueKey
        var payload = new Dictionary<string, object?>
        {
            { "command", command },
            { "target", string.IsNullOrWhiteSpace(target) ? string.Empty : target! },
            { "args", args },
            { "createTime", now },
        };
        await _redis.PushList(Consts.AdminGmQueueKey, payload);
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
