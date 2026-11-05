using consts;
using core;
using leaderboard;
using MongoDB.Bson;
using MongoDB.Driver;
using shop;

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

/// <summary>邮件附件 / 商品奖励（后台录入用）：物品 id + 数量。</summary>
public record MailAttachmentInput(string ItemId, int Count = 1);

/// <summary>
/// 后台编辑商品用的入参。SaleMode：1 = 一口价，2 = 限时拍卖。
/// 时间字段都是 Unix 毫秒，0 = 不限制。
/// </summary>
public record ShopProductInput(
    string Name,
    string? Description = null,
    string? Icon = null,
    string? Category = null,
    int SaleMode = 1,
    string? CurrencyItemId = null,
    long Price = 0,
    long StartPrice = 0,
    long MinIncrement = 1,
    int AuctionDurationSeconds = 0,
    bool AutoRestart = false,
    int Stock = -1,
    int PerUserLimit = 0,
    bool Enabled = true,
    int Sort = 0,
    long StartTime = 0,
    long EndTime = 0,
    MailAttachmentInput[]? Rewards = null,
    string? ProductId = null);

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
    private readonly ShopStore _shop;
    private readonly LeaderboardStore _leaderboard;

    public AdminDataService(AdminConfig cfg)
    {
        _cfg = cfg;
        _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
        _mongo = new MongoClient(cfg.MongoUrl);
        _db = _mongo.GetDatabase(cfg.MongoDb);
        // admin 与 hub 共用同一份 Shop 配置（KeyPrefix / 货币），否则上架商品游戏服读不到
        _shop = new ShopStore(_redis, cfg.Shop);
        // admin 与 hub 共用同一份 Leaderboard 配置（KeyPrefix），否则重置的不是游戏服的榜
        _leaderboard = new LeaderboardStore(_redis, cfg.Leaderboard);
    }

    public ShopStore Shop => _shop;

    public LeaderboardStore Leaderboard => _leaderboard;

    private IMongoCollection<BsonDocument> Col(string name) => _db.GetCollection<BsonDocument>(name);

    /// <summary>健康检查用：Redis 能 PING 才算健康。</summary>
    public async Task PingRedisAsync()
    {
        await _redis.Execute("health.ping", db => db.PingAsync());
    }

    // ---------------- 排行榜 ----------------

    /// <summary>重置（清空）某个排行榜。</summary>
    public async Task ResetLeaderboard(string boardId)
    {
        await _leaderboard.ResetAsync(boardId);
    }

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
    public async Task SendMail(string title, string content, string sender, MailAttachmentInput[] attachments,
        string[] playerIds, int expireSeconds, GlobalMailRule rule)
    {
        var isGlobal = playerIds.Length == 0;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var mailId = Guid.NewGuid().ToString("N");

        // 附件：Mongo 留档用 PascalCase，Redis 队列用 camelCase
        var attBson = new BsonArray();
        var attJson = new List<Dictionary<string, object?>>();
        foreach (var a in attachments)
        {
            attBson.Add(new BsonDocument
            {
                { "ItemId", a.ItemId },
                { "Count", a.Count },
            });
            attJson.Add(new Dictionary<string, object?>
            {
                { "itemId", a.ItemId },
                { "count", a.Count },
            });
        }

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
            { "Sender", sender },
            { "Attachments", attBson },
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
                { "sender", sender },
                { "attachments", attJson },
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
                    { "sender", sender },
                    { "attachments", attJson },
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

    // ================================================================
    // 商城：商品 CRUD（MongoDB）+ 上架/下架（Redis，游戏服可见）
    // ================================================================

    public async Task<(long Total, List<BsonDocument> Items)> QueryShopProducts(
        string? keyword, string? status, int skip, int limit)
    {
        var col = Col(_cfg.ShopProductCollection);
        var filters = new List<BsonDocument>();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            filters.Add(new BsonDocument("$or", new BsonArray
            {
                new BsonDocument("ProductId", new BsonDocument("$regex", keyword)),
                new BsonDocument("Name", new BsonDocument("$regex", keyword)),
            }));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            !status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            filters.Add(new BsonDocument("Published",
                status.Equals("published", StringComparison.OrdinalIgnoreCase)));
        }

        var filter = filters.Count == 0
            ? new BsonDocument()
            : new BsonDocument("$and", new BsonArray(filters));

        var total = await col.CountDocumentsAsync(filter);
        var cursor = await col.FindAsync(filter, new FindOptions<BsonDocument>
        {
            Skip = skip,
            Limit = limit,
            Sort = Builders<BsonDocument>.Sort.Descending("UpdateTime"),
        });
        return (total, await cursor.ToListAsync());
    }

    public async Task<BsonDocument?> GetShopProduct(string productId)
    {
        var cursor = await Col(_cfg.ShopProductCollection).FindAsync(
            new BsonDocument("ProductId", productId),
            new FindOptions<BsonDocument> { Limit = 1 });
        var items = await cursor.ToListAsync();
        return items.FirstOrDefault();
    }

    public async Task<(bool Ok, string Error, BsonDocument? Doc)> CreateShopProduct(ShopProductInput input)
    {
        var productId = string.IsNullOrWhiteSpace(input.ProductId)
            ? Guid.NewGuid().ToString("N")
            : input.ProductId.Trim();

        var (product, error) = BuildProduct(input, productId);
        if (product == null)
        {
            return (false, error, null);
        }

        var existing = await GetShopProduct(productId);
        if (existing != null)
        {
            return (false, "商品 id 已存在", null);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var doc = ProductToBson(product, now, now, false, string.Empty);
        await Col(_cfg.ShopProductCollection).InsertOneAsync(doc);
        return (true, string.Empty, doc);
    }

    public async Task<(bool Ok, string Error, string? Warning, BsonDocument? Doc)> UpdateShopProduct(
        string productId, ShopProductInput input)
    {
        var existing = await GetShopProduct(productId);
        if (existing == null)
        {
            return (false, "商品不存在", null, null);
        }

        var (product, error) = BuildProduct(input, productId);
        if (product == null)
        {
            return (false, error, null, null);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var createTime = DocLong(existing, "CreateTime", now);
        var published = DocBool(existing, "Published");
        var listingId = DocString(existing, "ListingId");
        string? warning = null;

        // 先保存配置；在售商品再尝试把新配置同步到线上 listing
        var doc = ProductToBson(product, createTime, now, published, listingId);
        await Col(_cfg.ShopProductCollection).ReplaceOneAsync(
            new BsonDocument("ProductId", productId), doc);

        if (published && !string.IsNullOrEmpty(listingId))
        {
            if (!product.Enabled)
            {
                // 禁用 = 不再允许售卖；能下就立刻下，已有出价的拍卖让管理员手动决定
                var cancelled = await _shop.CancelAsync(listingId, "admin", force: false, now);
                if (cancelled.Ok)
                {
                    published = false;
                    listingId = string.Empty;
                    warning = "商品已禁用并自动下架";
                }
                else if (cancelled.Code == "has_bids")
                {
                    warning = "商品已禁用，但当前拍卖已有出价，未能自动下架，请手动强制下架";
                }
                else
                {
                    published = false;
                    listingId = string.Empty;
                    warning = "商品已禁用，原商品已结束";
                }
            }
            else
            {
                var live = await _shop.GetListingAsync(listingId);
                if (live == null || live.Status != ListingStatuses.Active)
                {
                    published = false;
                    listingId = string.Empty;
                    warning = "该商品已结束，已标记为未上架，请重新上架";
                }
                else if (live.SaleMode == ListingMode.Auction && !string.IsNullOrEmpty(live.TopBidderId))
                {
                    warning = "拍卖已有出价，配置已保存但未同步到当前拍卖；如需修改请先强制下架";
                }
                else if (live.SaleMode == ListingMode.Fixed && live.SoldCount > 0)
                {
                    warning = "商品已有销量，配置已保存但未同步到当前在售商品；如需修改请先下架";
                }
                else
                {
                    // 拍卖需要换一个新的 listing（时长/起拍价都变了）；一口价原地覆盖
                    if (live.SaleMode == ListingMode.Auction)
                    {
                        await _shop.CancelAsync(listingId, "admin", force: true, now);
                        var republished = await _shop.PublishAsync(product, now);
                        if (republished.Ok && republished.Listing != null)
                        {
                            listingId = republished.Listing.ListingId;
                            published = true;
                        }
                        else
                        {
                            published = false;
                            listingId = string.Empty;
                            warning = "线上拍卖更新失败，商品已标记为未上架：" + ShopErrorText(republished.Code);
                        }
                    }
                    else
                    {
                        var republished = await _shop.PublishAsync(product, now, replace: true);
                        if (republished.Ok && republished.Listing != null)
                        {
                            listingId = republished.Listing.ListingId;
                            published = true;
                        }
                        else
                        {
                            warning = "线上商品更新失败：" + ShopErrorText(republished.Code);
                        }
                    }
                }
            }

            var sync = new BsonDocument
            {
                { "$set", new BsonDocument
                    {
                        { "Published", published },
                        { "ListingId", listingId },
                        { "UpdateTime", now },
                    }
                },
            };
            await Col(_cfg.ShopProductCollection).UpdateOneAsync(
                new BsonDocument("ProductId", productId), sync);
        }

        var updated = await GetShopProduct(productId);
        return (true, string.Empty, warning, updated);
    }

    public async Task<(bool Ok, string Error)> DeleteShopProduct(string productId, bool force)
    {
        var existing = await GetShopProduct(productId);
        if (existing == null)
        {
            return (false, "商品不存在");
        }

        if (DocBool(existing, "Published") || !string.IsNullOrEmpty(DocString(existing, "ListingId")))
        {
            var (ok, error, _) = await UnpublishShopProduct(productId, force);
            if (!ok)
            {
                return (false, error);
            }
        }

        await Col(_cfg.ShopProductCollection).DeleteOneAsync(
            new BsonDocument("ProductId", productId));
        return (true, string.Empty);
    }

    public async Task<(bool Ok, string Error, ShopListing? Listing)> PublishShopProduct(
        string productId, bool force)
    {
        var doc = await GetShopProduct(productId);
        if (doc == null)
        {
            return (false, "商品不存在", null);
        }

        var (product, error) = BsonToProduct(doc);
        if (product == null)
        {
            return (false, error, null);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var result = await _shop.PublishAsync(product, now, force);
        if (!result.Ok)
        {
            return (false, ShopErrorText(result.Code), null);
        }

        var update = new BsonDocument
        {
            { "$set", new BsonDocument
                {
                    { "Published", true },
                    { "ListingId", result.Listing!.ListingId },
                    { "UpdateTime", now },
                }
            },
        };
        await Col(_cfg.ShopProductCollection).UpdateOneAsync(
            new BsonDocument("ProductId", productId), update);
        return (true, string.Empty, result.Listing);
    }

    public async Task<(bool Ok, string Error, string? Warning)> UnpublishShopProduct(
        string productId, bool force)
    {
        var doc = await GetShopProduct(productId);
        if (doc == null)
        {
            return (false, "商品不存在", null);
        }

        var listingId = DocString(doc, "ListingId");
        string? warning = null;
        if (!string.IsNullOrEmpty(listingId))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var result = await _shop.CancelAsync(listingId, "admin", force, now);
            if (!result.Ok && result.Code is not ("not_active" or "not_found"))
            {
                return (false, ShopErrorText(result.Code), null);
            }

            if (!result.Ok)
            {
                warning = "该商品已经结束，仅清理了上架状态";
            }
        }

        var update = new BsonDocument
        {
            { "$set", new BsonDocument
                {
                    { "Published", false },
                    { "ListingId", string.Empty },
                    { "UpdateTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                }
            },
        };
        await Col(_cfg.ShopProductCollection).UpdateOneAsync(
            new BsonDocument("ProductId", productId), update);
        return (true, string.Empty, warning);
    }

    // ---------------- 线上 listing / 成交流水 ----------------

    public async Task<(int Total, List<ShopListing> Items)> QueryShopListings(
        string? source, string? status, int skip, int take)
    {
        ListingSource? src = source?.ToLowerInvariant() switch
        {
            "official" => ListingSource.Official,
            "player" => ListingSource.Player,
            _ => null,
        };
        var st = string.IsNullOrWhiteSpace(status) ||
                 status.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? null
            : status.ToLowerInvariant();

        return await _shop.QueryListingsAsync(new ShopListingQuery(
            Source: src,
            Status: st,
            IncludeInactive: true,
            Skip: skip,
            Take: take), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    public async Task<(bool Ok, string Error, ShopListing? Listing)> CancelShopListing(string listingId)
    {
        var live = await _shop.GetListingAsync(listingId);
        if (live == null)
        {
            return (false, "商品不存在", null);
        }

        var result = await _shop.CancelAsync(listingId, "admin", force: true,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        if (!result.Ok)
        {
            return (false, ShopErrorText(result.Code), null);
        }

        if (live.Source == ListingSource.Official && !string.IsNullOrEmpty(live.ProductId))
        {
            var update = new BsonDocument
            {
                { "$set", new BsonDocument
                    {
                        { "Published", false },
                        { "ListingId", string.Empty },
                        { "UpdateTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                    }
                },
            };
            await Col(_cfg.ShopProductCollection).UpdateOneAsync(
                new BsonDocument("ProductId", live.ProductId), update);
        }

        return (true, string.Empty, result.Listing);
    }

    public Task<(int Total, List<ShopOrder> Items)> QueryShopOrders(int skip, int take) =>
        _shop.QueryOrdersAsync(skip, take);

    // ---------------- 商城内部转换 ----------------

    private (ShopProduct? Product, string Error) BuildProduct(ShopProductInput input, string productId)
    {
        var name = input.Name?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return (null, "商品名称不能为空");
        }

        if (input.SaleMode is not (1 or 2))
        {
            return (null, "售卖模式只能是 1（一口价）或 2（限时拍卖）");
        }

        var rewards = (input.Rewards ?? Array.Empty<MailAttachmentInput>())
            .Where(a => !string.IsNullOrWhiteSpace(a.ItemId))
            .Select(a => new ShopItemGrant(a.ItemId.Trim(), Math.Max(0, a.Count)))
            .Where(a => a.Count > 0)
            .ToList();
        if (rewards.Count == 0)
        {
            return (null, "至少配置一个奖励物品");
        }

        var currency = string.IsNullOrWhiteSpace(input.CurrencyItemId)
            ? _cfg.Shop.CurrencyItemId
            : input.CurrencyItemId.Trim();
        if (string.IsNullOrWhiteSpace(currency))
        {
            return (null, "结算货币不能为空");
        }

        var mode = input.SaleMode == 2 ? ListingMode.Auction : ListingMode.Fixed;
        if (mode == ListingMode.Fixed && input.Price < 0)
        {
            return (null, "价格不能为负");
        }

        if (mode == ListingMode.Auction && (input.StartPrice <= 0 || input.MinIncrement <= 0))
        {
            return (null, "起拍价 / 最小加价必须大于 0");
        }

        if (mode == ListingMode.Fixed && (input.Stock < -1 || input.Stock == 0))
        {
            return (null, "一口价库存必须 > 0 或 = -1（不限）");
        }

        var product = new ShopProduct
        {
            ProductId = productId,
            Name = name,
            Description = input.Description ?? string.Empty,
            Icon = input.Icon ?? string.Empty,
            Category = input.Category ?? string.Empty,
            SaleMode = mode,
            CurrencyItemId = currency,
            Price = Math.Max(0, input.Price),
            StartPrice = Math.Max(0, input.StartPrice),
            MinIncrement = Math.Max(1, input.MinIncrement),
            AuctionDurationSeconds = Math.Max(0, input.AuctionDurationSeconds),
            AutoRestart = input.AutoRestart,
            Stock = input.Stock < 0 ? -1 : input.Stock,
            PerUserLimit = Math.Max(0, input.PerUserLimit),
            Enabled = input.Enabled,
            Sort = input.Sort,
            StartTime = Math.Max(0, input.StartTime),
            EndTime = Math.Max(0, input.EndTime),
            Items = rewards,
        };

        return (product, string.Empty);
    }

    private static BsonDocument ProductToBson(ShopProduct product, long createTime, long updateTime,
        bool published, string listingId)
    {
        var rewards = new BsonArray();
        foreach (var item in product.Items)
        {
            rewards.Add(new BsonDocument
            {
                { "ItemId", item.ItemId },
                { "Count", item.Count },
            });
        }

        return new BsonDocument
        {
            { "ProductId", product.ProductId },
            { "Name", product.Name },
            { "Description", product.Description },
            { "Icon", product.Icon },
            { "Category", product.Category },
            { "SaleMode", (int)product.SaleMode },
            { "CurrencyItemId", product.CurrencyItemId },
            { "Price", product.Price },
            { "StartPrice", product.StartPrice },
            { "MinIncrement", product.MinIncrement },
            { "AuctionDurationSeconds", product.AuctionDurationSeconds },
            { "AutoRestart", product.AutoRestart },
            { "Stock", product.Stock },
            { "PerUserLimit", product.PerUserLimit },
            { "Enabled", product.Enabled },
            { "Sort", product.Sort },
            { "StartTime", product.StartTime },
            { "EndTime", product.EndTime },
            { "Rewards", rewards },
            { "Published", published },
            { "ListingId", listingId },
            { "CreateTime", createTime },
            { "UpdateTime", updateTime },
        };
    }

    private static (ShopProduct? Product, string Error) BsonToProduct(BsonDocument doc)
    {
        var rewards = new List<ShopItemGrant>();
        if (doc.TryGetValue("Rewards", out var rewardsValue) && rewardsValue.IsBsonArray)
        {
            foreach (var entry in rewardsValue.AsBsonArray)
            {
                if (!entry.IsBsonDocument)
                {
                    continue;
                }

                var itemId = DocString(entry.AsBsonDocument, "ItemId");
                var count = DocLong(entry.AsBsonDocument, "Count");
                if (!string.IsNullOrWhiteSpace(itemId) && count > 0)
                {
                    rewards.Add(new ShopItemGrant(itemId, count));
                }
            }
        }

        var product = new ShopProduct
        {
            ProductId = DocString(doc, "ProductId"),
            Name = DocString(doc, "Name"),
            Description = DocString(doc, "Description"),
            Icon = DocString(doc, "Icon"),
            Category = DocString(doc, "Category"),
            SaleMode = DocInt(doc, "SaleMode") == 2 ? ListingMode.Auction : ListingMode.Fixed,
            CurrencyItemId = DocString(doc, "CurrencyItemId"),
            Price = DocLong(doc, "Price"),
            StartPrice = DocLong(doc, "StartPrice"),
            MinIncrement = Math.Max(1, DocLong(doc, "MinIncrement")),
            AuctionDurationSeconds = DocInt(doc, "AuctionDurationSeconds"),
            AutoRestart = DocBool(doc, "AutoRestart"),
            Stock = DocInt(doc, "Stock", -1),
            PerUserLimit = DocInt(doc, "PerUserLimit"),
            Enabled = DocBool(doc, "Enabled"),
            Sort = DocInt(doc, "Sort"),
            StartTime = DocLong(doc, "StartTime"),
            EndTime = DocLong(doc, "EndTime"),
            Items = rewards,
        };

        return string.IsNullOrWhiteSpace(product.ProductId) || product.Items.Count == 0
            ? (null, "商品配置缺少 ProductId 或奖励物品")
            : (product, string.Empty);
    }

    private static string DocString(BsonDocument doc, string field)
    {
        return doc.TryGetValue(field, out var value) && value.IsString ? value.AsString : string.Empty;
    }

    private static long DocLong(BsonDocument doc, string field, long fallback = 0)
    {
        if (!doc.TryGetValue(field, out var value))
        {
            return fallback;
        }

        return value.IsNumeric ? value.ToInt64() : fallback;
    }

    private static int DocInt(BsonDocument doc, string field, int fallback = 0)
    {
        if (!doc.TryGetValue(field, out var value))
        {
            return fallback;
        }

        return value.IsNumeric ? value.ToInt32() : fallback;
    }

    private static bool DocBool(BsonDocument doc, string field, bool fallback = false)
    {
        if (!doc.TryGetValue(field, out var value))
        {
            return fallback;
        }

        return value.IsBoolean ? value.AsBoolean : fallback;
    }

    private static string ShopErrorText(string code) => code switch
    {
        "already_active" => "商品已经在售，如需更新请选择强制覆盖",
        "has_bids" => "拍卖已有出价，强制下架会把最高出价退还给买家",
        "window_closed" => "商品已过停售时间",
        "disabled" => "商品未启用，请先勾选启用再上架",
        "not_active" => "商品已下架或已结束",
        "not_found" => "商品不存在",
        "invalid" => "商品配置不合法",
        _ => "操作失败：" + code,
    };

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
