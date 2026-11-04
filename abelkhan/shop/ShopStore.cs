using System.Globalization;
using core;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace shop;

/// <summary>
/// 商城 Redis 存储层。所有会改变商品状态的操作（购买 / 出价 / 取消 / 结算）都用 Lua
/// 在 Redis 内原子执行，保证多 hub 进程并发时不会超卖、重复结算或把同一件物品卖两次。
///
/// 物品的「发放 / 扣除」仍然发生在玩家自己的 hub 进程（背包不是共享的），
/// 由 <c>hub.shop.ShopTrader</c> 负责，本类只负责商品状态和「待领取」结算队列。
/// </summary>
public sealed class ShopStore
{
    /// <summary>Lua 用 double 表示金额，超过 2^53 会丢精度。</summary>
    private const long MaxAmount = 90_000_000_000_000;

    private readonly RedisHandle _redis;
    private readonly ShopSettings _settings;

    public ShopStore(RedisHandle redis, ShopSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new ShopKeys(settings.KeyPrefix);
    }

    public ShopSettings Settings => _settings;

    public ShopKeys Keys { get; }

    // ================================================================
    // 商品上架 / 寄拍创建
    // ================================================================

    /// <summary>
    /// 把 admin 后台的商品配置发布成一条对玩家可见的 listing。
    /// <paramref name="replace"/> = true 时允许覆盖仍在售的同一商品（拍卖有出价时拒绝，避免吞掉保证金）。
    /// </summary>
    public async Task<ShopResult> PublishAsync(ShopProduct product, long now, bool replace = false)
    {
        var invalid = ValidateProduct(product);
        if (invalid != null)
        {
            return invalid;
        }

        var mode = product.SaleMode;
        var start = Math.Max(now, product.StartTime > 0 ? product.StartTime : now);
        long end = 0;
        if (mode == ListingMode.Auction)
        {
            var duration = _settings.NormalizeDurationSeconds(product.AuctionDurationSeconds, auction: true);
            end = start + duration * 1000L;
            if (product.EndTime > 0)
            {
                end = Math.Min(end, product.EndTime);
            }

            if (end <= now || end <= start)
            {
                return ShopResult.Fail("window_closed", "商品已过停售时间");
            }
        }
        else
        {
            end = product.EndTime;
            if (end > 0 && (end <= now || end <= start))
            {
                return ShopResult.Fail("window_closed", "商品已过停售时间");
            }
        }

        var listingId = mode == ListingMode.Fixed
            ? product.ProductId
            : $"{product.ProductId}:a:{start}:{Random.Shared.Next(1000, 10000)}";

        var existing = await ReadListingAsync(listingId);
        if (existing != null && existing.Status == ListingStatuses.Active)
        {
            if (!replace)
            {
                return ShopResult.Fail("already_active", "该商品已在售");
            }

            if (existing.TopBidderId.Length > 0)
            {
                return ShopResult.Fail("has_bids", "当前拍卖已有出价，不能直接覆盖，请先下架");
            }
        }

        var listing = new ShopListing
        {
            ListingId = listingId,
            Source = ListingSource.Official,
            ProductId = product.ProductId,
            SellerId = string.Empty,
            Title = product.Name,
            Description = product.Description,
            Icon = product.Icon,
            Category = product.Category,
            SaleMode = mode,
            CurrencyItemId = string.IsNullOrWhiteSpace(product.CurrencyItemId)
                ? _settings.CurrencyItemId
                : product.CurrencyItemId,
            Price = Math.Max(0, product.Price),
            StartPrice = Math.Max(0, product.StartPrice),
            MinIncrement = Math.Max(1, product.MinIncrement),
            CurrentPrice = 0,
            TopBidderId = string.Empty,
            Stock = mode == ListingMode.Fixed ? NormalizeStock(product.Stock) : 1,
            PerUserLimit = Math.Max(0, product.PerUserLimit),
            Items = product.Items.Select(i => i.Clone()).ToList(),
            StartTime = start,
            EndTime = end,
            Status = ListingStatuses.Active,
            AutoRestart = product.AutoRestart,
            AuctionDurationSeconds = product.AuctionDurationSeconds,
            SaleEndTime = product.EndTime,
            CreateTime = now,
            SoldCount = 0,
            Sort = product.Sort,
        };

        await WriteListingAsync(listing);
        await IndexListingAsync(listing);
        if (end == 0)
        {
            // 之前可能是限时商品，改成不限时后要把旧的到期扫描项清掉
            await _redis.Execute("shop.zrem_active",
                db => db.SortedSetRemoveAsync(Keys.Active, listingId));
        }

        if (mode == ListingMode.Fixed)
        {
            // 重新上架 = 重置限购计数
            await _redis.Execute("shop.del_purchased",
                db => db.KeyDeleteAsync(Keys.Purchased(listingId)));
        }

        return ShopResult.Success(listing);
    }

    /// <summary>创建玩家寄拍。调用方负责先把物品从卖家背包扣除（见 hub.shop.ShopTrader）。</summary>
    public async Task<ShopResult> CreatePlayerListingAsync(ShopListing listing, long now)
    {
        if (string.IsNullOrWhiteSpace(listing.SellerId))
        {
            return ShopResult.Fail("invalid", "卖家不能为空");
        }

        if (listing.Items.Count == 0 || listing.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemId) || i.Count <= 0))
        {
            return ShopResult.Fail("invalid", "寄拍物品配置非法");
        }

        if (listing.SaleMode == ListingMode.Fixed &&
            (listing.Price < 0 || listing.Price > MaxAmount))
        {
            return ShopResult.Fail("invalid", "价格必须在 0 到上限之间");
        }

        if (listing.SaleMode == ListingMode.Auction &&
            (listing.StartPrice <= 0 || listing.MinIncrement <= 0 ||
             listing.StartPrice > MaxAmount || listing.MinIncrement > MaxAmount))
        {
            return ShopResult.Fail("invalid", "起拍价 / 最小加价必须大于 0 且不超过上限");
        }

        if (listing.SaleMode is not (ListingMode.Fixed or ListingMode.Auction))
        {
            return ShopResult.Fail("invalid", "未知售卖模式");
        }

        if (string.IsNullOrWhiteSpace(listing.CurrencyItemId))
        {
            return ShopResult.Fail("invalid", "结算货币不能为空");
        }

        if (listing.EndTime <= now)
        {
            return ShopResult.Fail("invalid", "寄拍结束时间必须晚于当前时间");
        }

        if (_settings.MaxListingsPerPlayer > 0)
        {
            var count = await _redis.Execute("shop.scard_seller",
                db => db.SetLengthAsync(Keys.SellerListings(listing.SellerId)));
            if (count >= _settings.MaxListingsPerPlayer)
            {
                return ShopResult.Fail("listing_limit",
                    $"最多同时寄拍 {_settings.MaxListingsPerPlayer} 件");
            }
        }

        listing.ListingId = Guid.NewGuid().ToString("N");
        listing.Source = ListingSource.Player;
        listing.ProductId = string.Empty;
        listing.Status = ListingStatuses.Active;
        listing.CurrentPrice = 0;
        listing.TopBidderId = string.Empty;
        listing.SoldCount = 0;
        listing.CreateTime = now;
        listing.SaleEndTime = 0;
        listing.Sort = 0;
        // 玩家寄拍一条 listing 只对应一份物品，卖完 / 到期就结束
        listing.Stock = 1;
        listing.PerUserLimit = 0;

        await WriteListingAsync(listing);
        await IndexListingAsync(listing);

        return ShopResult.Success(listing);
    }

    // ================================================================
    // 查询
    // ================================================================

    public async Task<ShopListing?> GetListingAsync(string listingId)
    {
        if (string.IsNullOrWhiteSpace(listingId))
        {
            return null;
        }

        return await ReadListingAsync(listingId);
    }

    public async Task<(int Total, List<ShopListing> Items)> QueryListingsAsync(
        ShopListingQuery query, long now)
    {
        var ids = await _redis.Execute("shop.smembers",
            db => db.SetMembersAsync(Keys.Listings));

        var all = new List<ShopListing>(ids.Length);
        foreach (var id in ids)
        {
            var listing = await ReadListingAsync(id.ToString());
            if (listing == null)
            {
                await _redis.Execute("shop.srem_stale",
                    db => db.SetRemoveAsync(Keys.Listings, id));
                continue;
            }

            if (!query.IncludeInactive && !listing.IsActiveAt(now))
            {
                continue;
            }

            // admin 选「在售中」时，即便查全部状态也不要混进已到期未结算的
            if (query.Status == ListingStatuses.Active && !listing.IsActiveAt(now))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(query.Status) &&
                !string.Equals(listing.Status, query.Status, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(query.Category) &&
                !string.Equals(listing.Category, query.Category, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (query.Source.HasValue && listing.Source != query.Source.Value)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(query.SellerId) && listing.SellerId != query.SellerId)
            {
                continue;
            }

            all.Add(listing);
        }

        all.Sort((a, b) =>
        {
            var bySort = b.Sort.CompareTo(a.Sort);
            return bySort != 0 ? bySort : b.CreateTime.CompareTo(a.CreateTime);
        });

        var total = all.Count;
        var skip = Math.Max(0, query.Skip);
        var take = query.Take <= 0 ? 50 : query.Take;
        var items = all.Skip(skip).Take(take).ToList();
        return (total, items);
    }

    /// <summary>当前玩家在某个 listing 上已购买的数量（限购展示用）。</summary>
    public async Task<int> GetPurchasedCountAsync(string listingId, string playerId)
    {
        if (string.IsNullOrEmpty(listingId) || string.IsNullOrEmpty(playerId))
        {
            return 0;
        }

        var value = await _redis.Execute("shop.hget_purchased",
            db => db.HashGetAsync(Keys.Purchased(listingId), playerId));
        return (int)ShopJson.Long(value.ToString());
    }

    // ================================================================
    // 购买 / 出价 / 取消 / 结算（Lua 原子）
    // ================================================================

    public async Task<ShopResult> PurchaseAsync(string listingId, string buyerId, int count,
        long now, string orderId, long expectedPrice, string expectedCurrency)
    {
        if (count <= 0)
        {
            return ShopResult.Fail("invalid", "购买数量必须大于 0");
        }

        try
        {
            var result = await _redis.Execute("shop.purchase", db => db.ScriptEvaluateAsync(
                PurchaseScript,
                new RedisKey[]
                {
                    Keys.Listing(listingId), Keys.Purchased(listingId), Keys.Active, Keys.Job(orderId),
                },
                new RedisValue[]
                {
                    listingId, buyerId, Math.Max(1, count), now, orderId,
                    Keys.DeliveryPrefix, _settings.NormalizedTaxPercent,
                    expectedPrice, expectedCurrency, Keys.SellerPrefix,
                }));

            var obj = ShopJson.ParseLua(result.ToString());
            if (!(obj.Value<bool?>("ok") ?? false))
            {
                var code = obj.Value<string>("code") ?? "failed";
                return ShopResult.Fail(code, code);
            }

            var listing = await ReadListingAsync(listingId);
            return ShopResult.Success(listing);
        }
        catch (Exception e)
        {
            Log.Error("shop purchase listing:{0} buyer:{1} err:{2}", listingId, buyerId, e);
            return ShopResult.Fail("internal", e.Message);
        }
    }

    public async Task<ShopResult> PlaceBidAsync(string listingId, string bidderId, long price,
        long now, string orderId, string expectedCurrency)
    {
        if (price <= 0)
        {
            return ShopResult.Fail("invalid", "出价必须大于 0");
        }

        try
        {
            var result = await _redis.Execute("shop.bid", db => db.ScriptEvaluateAsync(
                BidScript,
                new RedisKey[] { Keys.Listing(listingId), Keys.Job(orderId) },
                new RedisValue[]
                {
                    listingId, bidderId, price, now, orderId, Keys.DeliveryPrefix,
                    expectedCurrency,
                }));

            var obj = ShopJson.ParseLua(result.ToString());
            if (!(obj.Value<bool?>("ok") ?? false))
            {
                var code = obj.Value<string>("code") ?? "failed";
                return ShopResult.Fail(code, code);
            }

            var listing = await ReadListingAsync(listingId);
            return ShopResult.Success(listing);
        }
        catch (Exception e)
        {
            Log.Error("shop bid listing:{0} bidder:{1} err:{2}", listingId, bidderId, e);
            return ShopResult.Fail("internal", e.Message);
        }
    }

    public async Task<ShopResult> CancelAsync(string listingId, string actorId, bool force, long now)
    {
        try
        {
            var result = await _redis.Execute("shop.cancel", db => db.ScriptEvaluateAsync(
                CancelScript,
                new RedisKey[] { Keys.Listing(listingId), Keys.Active },
                new RedisValue[]
                {
                    listingId, actorId, force ? "1" : "0", now, Keys.DeliveryPrefix,
                    _settings.NormalizedTaxPercent, Keys.SellerPrefix,
                }));

            var obj = ShopJson.ParseLua(result.ToString());
            if (!(obj.Value<bool?>("ok") ?? false))
            {
                var code = obj.Value<string>("code") ?? "failed";
                return ShopResult.Fail(code, code);
            }

            var listing = await ReadListingAsync(listingId);
            return ShopResult.Success(listing);
        }
        catch (Exception e)
        {
            Log.Error("shop cancel listing:{0} actor:{1} err:{2}", listingId, actorId, e);
            return ShopResult.Fail("internal", e.Message);
        }
    }

    /// <summary>
    /// 扫描到期的商品并结算。返回本次真正结算掉的 listing（用于写流水 / 官方拍卖自动续拍）。
    /// 多个 hub 进程同时扫描也没关系：Lua 里 status 只会从 active 成功切换一次。
    /// </summary>
    public async Task<List<ShopSettlement>> SettleExpiredAsync(int limit, long now)
    {
        var settled = new List<ShopSettlement>();
        if (limit <= 0)
        {
            return settled;
        }

        var ids = await _redis.Execute("shop.zrange_active", db => db.SortedSetRangeByScoreAsync(
            Keys.Active, double.NegativeInfinity, now,
            Exclude.None, Order.Ascending, 0, limit));

        foreach (var idValue in ids)
        {
            var id = idValue.ToString();
            try
            {
                var result = await _redis.Execute("shop.settle", db => db.ScriptEvaluateAsync(
                    SettleScript,
                    new RedisKey[] { Keys.Listing(id), Keys.Active },
                    new RedisValue[]
                    {
                        id, now, Keys.DeliveryPrefix, _settings.NormalizedTaxPercent,
                        Keys.SellerPrefix,
                    }));

                var obj = ShopJson.ParseLua(result.ToString());
                if (!(obj.Value<bool?>("ok") ?? false))
                {
                    continue;
                }

                var listing = await ReadListingAsync(id);
                if (listing == null)
                {
                    continue;
                }

                var type = obj.Value<string>("settled") ?? "expired";
                var price = obj.Value<long?>("price") ?? 0;
                var fee = obj.Value<long?>("fee") ?? 0;
                var settlement = new ShopSettlement(listing, type,
                    obj.Value<string>("winner") ?? string.Empty, price, fee);
                settled.Add(settlement);

                await AppendOrderAsync(new ShopOrder
                {
                    OrderId = $"settle:{id}",
                    Type = type == "sold" ? "auction" : "expire",
                    ListingId = id,
                    ProductId = listing.ProductId,
                    BuyerId = settlement.WinnerId,
                    SellerId = listing.SellerId,
                    CurrencyItemId = listing.CurrencyItemId,
                    Price = price,
                    Fee = fee,
                    Items = listing.Items.Select(i => i.Clone()).ToList(),
                    CreateTime = now,
                });

                // 官方拍卖自动续拍：只有真正抢到结算的进程会走到这里
                if (type == "sold" && listing.Source == ListingSource.Official &&
                    listing.AutoRestart && listing.SaleMode == ListingMode.Auction)
                {
                    await RestartAuctionAsync(listing, now);
                }
            }
            catch (Exception e)
            {
                Log.Error("shop settle listing:{0} err:{1}", id, e);
            }
        }

        return settled;
    }

    /// <summary>官方拍卖成交后，按原配置再开一场（新 listing id），直到商品停售时间。</summary>
    public async Task<ShopResult> RestartAuctionAsync(ShopListing previous, long now)
    {
        if (previous.Source != ListingSource.Official ||
            previous.SaleMode != ListingMode.Auction ||
            !previous.AutoRestart)
        {
            return ShopResult.Fail("not_restartable", "该商品不支持自动续拍");
        }

        if (previous.SaleEndTime > 0 && now >= previous.SaleEndTime)
        {
            return ShopResult.Fail("window_closed", "商品已过停售时间");
        }

        var duration = _settings.NormalizeDurationSeconds(previous.AuctionDurationSeconds, auction: true);
        var end = now + duration * 1000L;
        if (previous.SaleEndTime > 0)
        {
            end = Math.Min(end, previous.SaleEndTime);
        }

        if (end <= now)
        {
            return ShopResult.Fail("window_closed", "商品已过停售时间");
        }

        var next = previous.Clone();
        next.ListingId = $"{previous.ProductId}:a:{now}:{Random.Shared.Next(1000, 10000)}";
        next.StartTime = now;
        next.EndTime = end;
        next.CurrentPrice = 0;
        next.TopBidderId = string.Empty;
        next.Status = ListingStatuses.Active;
        next.CreateTime = now;
        next.SoldCount = 0;
        next.Stock = 1;

        await WriteListingAsync(next);
        await IndexListingAsync(next);
        return ShopResult.Success(next);
    }

    // ================================================================
    // 待领取结算
    // ================================================================

    public async Task<List<ShopDelivery>> PeekDeliveriesAsync(string playerId)
    {
        var entries = await _redis.Execute("shop.hgetall_delivery",
            db => db.HashGetAllAsync(Keys.Delivery(playerId)));

        var result = new List<ShopDelivery>(entries.Length);
        foreach (var entry in entries)
        {
            var delivery = ShopJson.ParseDelivery(entry.Value.ToString());
            if (delivery == null || string.IsNullOrWhiteSpace(delivery.ItemId) || delivery.Count <= 0)
            {
                continue;
            }

            if (string.IsNullOrEmpty(delivery.DeliveryId))
            {
                delivery.DeliveryId = entry.Name.ToString();
            }

            result.Add(delivery);
        }

        result.Sort((a, b) =>
        {
            var byTime = a.CreateTime.CompareTo(b.CreateTime);
            return byTime != 0 ? byTime : string.CompareOrdinal(a.DeliveryId, b.DeliveryId);
        });
        return result;
    }

    public async Task RemoveDeliveriesAsync(string playerId, IEnumerable<string> deliveryIds)
    {
        var fields = deliveryIds
            .Where(id => !string.IsNullOrEmpty(id))
            .Select(id => (RedisValue)id)
            .ToArray();
        if (fields.Length == 0)
        {
            return;
        }

        await _redis.Execute("shop.hdel_delivery",
            db => db.HashDeleteAsync(Keys.Delivery(playerId), fields));
    }

    public async Task<long> PendingDeliveryCountAsync(string playerId)
    {
        return await _redis.Execute("shop.hlen_delivery",
            db => db.HashLengthAsync(Keys.Delivery(playerId)));
    }

    /// <summary>把一条结算直接放进玩家待领取队列（背包满 / 退款失败时的兜底）。</summary>
    public async Task PushDeliveryAsync(string playerId, ShopDelivery delivery)
    {
        if (string.IsNullOrEmpty(delivery.DeliveryId))
        {
            delivery.DeliveryId = Guid.NewGuid().ToString("N");
        }

        await _redis.Execute("shop.hset_delivery",
            db => db.HashSetAsync(Keys.Delivery(playerId), delivery.DeliveryId,
                ShopJson.Delivery(delivery)));
    }

    // ================================================================
    // 成交流水
    // ================================================================

    public async Task AppendOrderAsync(ShopOrder order)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(order);
        await _redis.Execute("shop.push_order",
            db => db.ListRightPushAsync(Keys.Orders, json));
        await _redis.Execute("shop.trim_orders",
            db => db.ListTrimAsync(Keys.Orders, -5000, -1));
    }

    public async Task<(int Total, List<ShopOrder> Items)> QueryOrdersAsync(int skip, int take)
    {
        var values = await _redis.Execute("shop.orders",
            db => db.ListRangeAsync(Keys.Orders, 0, -1));
        var all = new List<ShopOrder>(values.Length);
        for (var i = values.Length - 1; i >= 0; i--)
        {
            try
            {
                var order = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopOrder>(values[i].ToString());
                if (order != null)
                {
                    all.Add(order);
                }
            }
            catch
            {
                // 单条坏数据不影响整个列表
            }
        }

        var total = all.Count;
        var items = all.Skip(Math.Max(0, skip)).Take(take <= 0 ? 20 : take).ToList();
        return (total, items);
    }

    // ================================================================
    // 内部：hash 读写
    // ================================================================

    private async Task<ShopListing?> ReadListingAsync(string listingId)
    {
        var entries = await _redis.Execute("shop.hgetall_listing",
            db => db.HashGetAllAsync(Keys.Listing(listingId)));
        if (entries.Length == 0)
        {
            return null;
        }

        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in entries)
        {
            map[entry.Name.ToString()] = entry.Value.ToString();
        }

        return new ShopListing
        {
            ListingId = map.GetValueOrDefault("listingId", listingId),
            Source = (ListingSource)ShopJson.Int(map.GetValueOrDefault("source"), 1),
            ProductId = map.GetValueOrDefault("productId", string.Empty),
            SellerId = map.GetValueOrDefault("sellerId", string.Empty),
            Title = map.GetValueOrDefault("title", string.Empty),
            Description = map.GetValueOrDefault("description", string.Empty),
            Icon = map.GetValueOrDefault("icon", string.Empty),
            Category = map.GetValueOrDefault("category", string.Empty),
            SaleMode = (ListingMode)ShopJson.Int(map.GetValueOrDefault("saleMode"), 1),
            CurrencyItemId = map.GetValueOrDefault("currencyItemId", string.Empty),
            Price = ShopJson.Long(map.GetValueOrDefault("price")),
            StartPrice = ShopJson.Long(map.GetValueOrDefault("startPrice")),
            MinIncrement = Math.Max(1, ShopJson.Long(map.GetValueOrDefault("minIncrement"), 1)),
            CurrentPrice = ShopJson.Long(map.GetValueOrDefault("currentPrice")),
            TopBidderId = map.GetValueOrDefault("topBidderId", string.Empty),
            Stock = ShopJson.Int(map.GetValueOrDefault("stock"), 1),
            PerUserLimit = ShopJson.Int(map.GetValueOrDefault("perUserLimit")),
            Items = ShopJson.ParseItems(map.GetValueOrDefault("items")),
            StartTime = ShopJson.Long(map.GetValueOrDefault("startTime")),
            EndTime = ShopJson.Long(map.GetValueOrDefault("endTime")),
            Status = map.GetValueOrDefault("status", ListingStatuses.Active),
            AutoRestart = ShopJson.Bool(map.GetValueOrDefault("autoRestart")),
            AuctionDurationSeconds = ShopJson.Int(map.GetValueOrDefault("auctionDurationSeconds")),
            SaleEndTime = ShopJson.Long(map.GetValueOrDefault("saleEndTime")),
            CreateTime = ShopJson.Long(map.GetValueOrDefault("createTime")),
            SoldCount = ShopJson.Long(map.GetValueOrDefault("soldCount")),
            Sort = ShopJson.Int(map.GetValueOrDefault("sort")),
        };
    }

    private async Task WriteListingAsync(ShopListing listing)
    {
        var entries = new HashEntry[]
        {
            new("listingId", listing.ListingId),
            new("source", ((int)listing.Source).ToString(CultureInfo.InvariantCulture)),
            new("productId", listing.ProductId),
            new("sellerId", listing.SellerId),
            new("title", listing.Title),
            new("description", listing.Description),
            new("icon", listing.Icon),
            new("category", listing.Category),
            new("saleMode", ((int)listing.SaleMode).ToString(CultureInfo.InvariantCulture)),
            new("currencyItemId", listing.CurrencyItemId),
            new("price", ShopJson.Str(listing.Price)),
            new("startPrice", ShopJson.Str(listing.StartPrice)),
            new("minIncrement", ShopJson.Str(Math.Max(1, listing.MinIncrement))),
            new("currentPrice", ShopJson.Str(listing.CurrentPrice)),
            new("topBidderId", listing.TopBidderId),
            new("stock", ShopJson.Str(listing.Stock)),
            new("perUserLimit", ShopJson.Str(listing.PerUserLimit)),
            new("items", ShopJson.Items(listing.Items)),
            new("startTime", ShopJson.Str(listing.StartTime)),
            new("endTime", ShopJson.Str(listing.EndTime)),
            new("status", listing.Status),
            new("autoRestart", listing.AutoRestart ? "1" : "0"),
            new("auctionDurationSeconds", ShopJson.Str(listing.AuctionDurationSeconds)),
            new("saleEndTime", ShopJson.Str(listing.SaleEndTime)),
            new("createTime", ShopJson.Str(listing.CreateTime)),
            new("soldCount", ShopJson.Str(listing.SoldCount)),
            new("sort", ShopJson.Str(listing.Sort)),
        };

        await _redis.Execute("shop.hset_listing",
            db => db.HashSetAsync(Keys.Listing(listing.ListingId), entries));
    }

    private async Task IndexListingAsync(ShopListing listing)
    {
        await _redis.Execute("shop.sadd_listing",
            db => db.SetAddAsync(Keys.Listings, listing.ListingId));
        if (listing.Source == ListingSource.Player)
        {
            await _redis.Execute("shop.sadd_seller",
                db => db.SetAddAsync(Keys.SellerListings(listing.SellerId), listing.ListingId));
        }

        if (listing.EndTime > 0)
        {
            await _redis.Execute("shop.zadd_active",
                db => db.SortedSetAddAsync(Keys.Active, listing.ListingId, listing.EndTime));
        }
    }

    // ================================================================
    // 参数校验
    // ================================================================

    private ShopResult? ValidateProduct(ShopProduct product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductId))
        {
            return ShopResult.Fail("invalid", "商品 id 不能为空");
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            return ShopResult.Fail("invalid", "商品名称不能为空");
        }

        if (!product.Enabled)
        {
            return ShopResult.Fail("disabled", "商品未启用，不能上架");
        }

        if (product.Items.Count == 0 ||
            product.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemId) || i.Count <= 0))
        {
            return ShopResult.Fail("invalid", "商品奖励物品不能为空，数量必须大于 0");
        }

        if (string.IsNullOrWhiteSpace(product.CurrencyItemId) &&
            string.IsNullOrWhiteSpace(_settings.CurrencyItemId))
        {
            return ShopResult.Fail("invalid", "结算货币不能为空");
        }

        if (product.SaleMode == ListingMode.Fixed)
        {
            if (product.Price < 0 || product.Price > MaxAmount)
            {
                return ShopResult.Fail("invalid", "价格必须在 0 到上限之间");
            }

            if (product.Stock < -1 || product.Stock == 0)
            {
                return ShopResult.Fail("invalid", "库存必须 > 0 或 = -1（不限）");
            }
        }
        else if (product.SaleMode == ListingMode.Auction)
        {
            if (product.StartPrice <= 0 || product.MinIncrement <= 0 ||
                product.StartPrice > MaxAmount || product.MinIncrement > MaxAmount)
            {
                return ShopResult.Fail("invalid", "起拍价 / 最小加价必须大于 0 且不超过上限");
            }
        }
        else
        {
            return ShopResult.Fail("invalid", "未知售卖模式");
        }

        return null;
    }

    private static int NormalizeStock(int stock) => stock < 0 ? -1 : stock;

    // ================================================================
    // Lua：所有脚本返回 cjson 编码的单个字符串，避免依赖 Redis 多值返回的绑定细节
    // ================================================================

    private const string PurchaseScript = """
        local key = KEYS[1]
        local purchasedKey = KEYS[2]
        local activeKey = KEYS[3]
        local listingId = ARGV[1]
        local buyerId = ARGV[2]
        local count = tonumber(ARGV[3])
        local now = tonumber(ARGV[4])
        local orderId = ARGV[5]
        local deliveryPrefix = ARGV[6]
        local taxPercent = tonumber(ARGV[7])
        local expectedPrice = tonumber(ARGV[8])
        local expectedCurrency = ARGV[9]
        local sellerPrefix = ARGV[10]

        -- 幂等检查必须放在所有「可能因第一次执行而改变」的校验之前：
        -- 重试时商品可能已售罄 / 改价 / 下架，但仍应返回第一次的成功结果
        if redis.call('EXISTS', KEYS[4]) == 1 then
          return cjson.encode({ ok = true, duplicate = true })
        end
        if redis.call('EXISTS', key) == 0 then
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('HGET', key, 'status') ~= 'active' then
          return cjson.encode({ ok = false, code = 'not_active' })
        end
        if tonumber(redis.call('HGET', key, 'saleMode') or '0') ~= 1 then
          return cjson.encode({ ok = false, code = 'not_fixed' })
        end
        local startTime = tonumber(redis.call('HGET', key, 'startTime') or '0')
        if startTime > now then
          return cjson.encode({ ok = false, code = 'not_started' })
        end
        local endTime = tonumber(redis.call('HGET', key, 'endTime') or '0')
        if endTime > 0 and now >= endTime then
          return cjson.encode({ ok = false, code = 'expired' })
        end
        local seller = redis.call('HGET', key, 'sellerId') or ''
        local source = tonumber(redis.call('HGET', key, 'source') or '1')
        if seller ~= '' and seller == buyerId then
          return cjson.encode({ ok = false, code = 'self' })
        end
        -- 客户端扣款用的是读取时的价格 / 货币；管理员可能在这期间改了商品，必须拒绝并退款
        local price = tonumber(redis.call('HGET', key, 'price') or '0')
        local currency = redis.call('HGET', key, 'currencyItemId') or ''
        if price < 0 then
          return cjson.encode({ ok = false, code = 'invalid' })
        end
        if price ~= expectedPrice then
          return cjson.encode({ ok = false, code = 'price_changed' })
        end
        if currency ~= expectedCurrency then
          return cjson.encode({ ok = false, code = 'currency_changed' })
        end
        local stock = tonumber(redis.call('HGET', key, 'stock') or '-1')
        if stock >= 0 and stock < count then
          return cjson.encode({ ok = false, code = 'sold_out' })
        end
        local perUser = tonumber(redis.call('HGET', key, 'perUserLimit') or '0')
        if perUser > 0 then
          local bought = tonumber(redis.call('HGET', purchasedKey, buyerId) or '0')
          if bought + count > perUser then
            return cjson.encode({ ok = false, code = 'limit' })
          end
        end
        if not redis.call('SET', KEYS[4], '1', 'NX', 'EX', 86400) then
          return cjson.encode({ ok = true, duplicate = true })
        end
        if perUser > 0 then
          redis.call('HINCRBY', purchasedKey, buyerId, count)
        end
        if stock >= 0 then
          stock = stock - count
          redis.call('HSET', key, 'stock', stock)
          if stock <= 0 then
            redis.call('HSET', key, 'status', 'sold')
            redis.call('ZREM', activeKey, listingId)
            if source == 2 and seller ~= '' then
              redis.call('SREM', sellerPrefix .. seller, listingId)
            end
          end
        end
        redis.call('HINCRBY', key, 'soldCount', count)
        local items = cjson.decode(redis.call('HGET', key, 'items') or '[]')
        local buyerKey = deliveryPrefix .. buyerId
        local index = 0
        for _, item in ipairs(items) do
          index = index + 1
          local delivery = {
            deliveryId = orderId .. ':buyer:' .. index,
            kind = 'item',
            itemId = item.itemId,
            count = tonumber(item.count) * count,
            reason = 'shop_buy',
            listingId = listingId,
            orderId = orderId,
            createTime = now,
          }
          redis.call('HSET', buyerKey, delivery.deliveryId, cjson.encode(delivery))
        end
        local fee = 0
        if source == 2 then
          local gross = price * count
          fee = math.floor(gross * taxPercent / 100)
          local net = gross - fee
          if net > 0 then
            local delivery = {
              deliveryId = orderId .. ':seller',
              kind = 'currency',
              itemId = currency,
              count = net,
              reason = 'sale_proceeds',
              listingId = listingId,
              orderId = orderId,
              createTime = now,
            }
            redis.call('HSET', deliveryPrefix .. seller, delivery.deliveryId, cjson.encode(delivery))
          end
        end
        return cjson.encode({ ok = true, stock = stock, seller = seller, currency = currency, fee = fee })
        """;

    private const string BidScript = """
        local key = KEYS[1]
        local listingId = ARGV[1]
        local bidderId = ARGV[2]
        local price = tonumber(ARGV[3])
        local now = tonumber(ARGV[4])
        local orderId = ARGV[5]
        local deliveryPrefix = ARGV[6]
        local expectedCurrency = ARGV[7]

        -- 幂等检查在最低出价校验之前：第一次出价成功后，重试的旧价格会被判 too_low
        if redis.call('EXISTS', KEYS[2]) == 1 then
          return cjson.encode({ ok = true, duplicate = true })
        end
        if redis.call('EXISTS', key) == 0 then
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('HGET', key, 'status') ~= 'active' then
          return cjson.encode({ ok = false, code = 'not_active' })
        end
        if tonumber(redis.call('HGET', key, 'saleMode') or '0') ~= 2 then
          return cjson.encode({ ok = false, code = 'not_auction' })
        end
        local startTime = tonumber(redis.call('HGET', key, 'startTime') or '0')
        if startTime > now then
          return cjson.encode({ ok = false, code = 'not_started' })
        end
        local endTime = tonumber(redis.call('HGET', key, 'endTime') or '0')
        if endTime > 0 and now >= endTime then
          return cjson.encode({ ok = false, code = 'expired' })
        end
        local seller = redis.call('HGET', key, 'sellerId') or ''
        if seller ~= '' and seller == bidderId then
          return cjson.encode({ ok = false, code = 'self' })
        end
        local currency = redis.call('HGET', key, 'currencyItemId') or ''
        if currency ~= expectedCurrency then
          return cjson.encode({ ok = false, code = 'currency_changed' })
        end
        local current = tonumber(redis.call('HGET', key, 'currentPrice') or '0')
        local top = redis.call('HGET', key, 'topBidderId') or ''
        local minIncrement = tonumber(redis.call('HGET', key, 'minIncrement') or '1')
        local startPrice = tonumber(redis.call('HGET', key, 'startPrice') or '1')
        local required
        if top == '' then
          required = startPrice
        else
          required = current + minIncrement
        end
        if price < required then
          return cjson.encode({ ok = false, code = 'too_low', required = required })
        end
        -- 幂等：同一个 orderId 的重试不会重复出价 / 退款
        if not redis.call('SET', KEYS[2], '1', 'NX', 'EX', 86400) then
          return cjson.encode({ ok = true, duplicate = true })
        end
        if top ~= '' and current > 0 then
          local delivery = {
            deliveryId = orderId .. ':outbid:' .. top,
            kind = 'currency',
            itemId = currency,
            count = current,
            reason = 'outbid_refund',
            listingId = listingId,
            orderId = orderId,
            createTime = now,
          }
          redis.call('HSET', deliveryPrefix .. top, delivery.deliveryId, cjson.encode(delivery))
        end
        redis.call('HSET', key, 'currentPrice', price, 'topBidderId', bidderId, 'lastBidTime', now)
        redis.call('HINCRBY', key, 'bidCount', 1)
        return cjson.encode({ ok = true, previous = current, previousTop = top, required = required })
        """;

    private const string CancelScript = """
        local key = KEYS[1]
        local activeKey = KEYS[2]
        local listingId = ARGV[1]
        local actorId = ARGV[2]
        local force = ARGV[3] == '1'
        local now = tonumber(ARGV[4])
        local deliveryPrefix = ARGV[5]
        local taxPercent = tonumber(ARGV[6])
        local sellerPrefix = ARGV[7]

        if redis.call('EXISTS', key) == 0 then
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('HGET', key, 'status') ~= 'active' then
          return cjson.encode({ ok = false, code = 'not_active' })
        end
        local source = tonumber(redis.call('HGET', key, 'source') or '1')
        local seller = redis.call('HGET', key, 'sellerId') or ''
        local mode = tonumber(redis.call('HGET', key, 'saleMode') or '1')
        local top = redis.call('HGET', key, 'topBidderId') or ''
        local current = tonumber(redis.call('HGET', key, 'currentPrice') or '0')
        if not force then
          if source == 2 and seller ~= actorId then
            return cjson.encode({ ok = false, code = 'forbidden' })
          end
          if mode == 2 and top ~= '' then
            return cjson.encode({ ok = false, code = 'has_bids' })
          end
        end
        redis.call('HSET', key, 'status', 'cancelled')
        redis.call('ZREM', activeKey, listingId)
        if source == 2 and seller ~= '' then
          redis.call('SREM', sellerPrefix .. seller, listingId)
        end
        local currency = redis.call('HGET', key, 'currencyItemId') or ''
        if top ~= '' and current > 0 then
          local delivery = {
            deliveryId = 'cancel:' .. listingId .. ':refund',
            kind = 'currency',
            itemId = currency,
            count = current,
            reason = 'listing_cancelled_refund',
            listingId = listingId,
            orderId = 'cancel:' .. listingId,
            createTime = now,
          }
          redis.call('HSET', deliveryPrefix .. top, delivery.deliveryId, cjson.encode(delivery))
        end
        if source == 2 and seller ~= '' then
          local stock = 1
          if mode == 1 then
            stock = tonumber(redis.call('HGET', key, 'stock') or '1')
            if stock < 0 then
              stock = 1
            end
          end
          if stock > 0 then
            local items = cjson.decode(redis.call('HGET', key, 'items') or '[]')
            local index = 0
            for _, item in ipairs(items) do
              index = index + 1
              local delivery = {
                deliveryId = 'cancel:' .. listingId .. ':return:' .. index,
                kind = 'item',
                itemId = item.itemId,
                count = tonumber(item.count) * stock,
                reason = 'listing_cancelled',
                listingId = listingId,
                orderId = 'cancel:' .. listingId,
                createTime = now,
              }
              redis.call('HSET', deliveryPrefix .. seller, delivery.deliveryId, cjson.encode(delivery))
            end
          end
        end
        return cjson.encode({ ok = true })
        """;

    private const string SettleScript = """
        local key = KEYS[1]
        local activeKey = KEYS[2]
        local listingId = ARGV[1]
        local now = tonumber(ARGV[2])
        local deliveryPrefix = ARGV[3]
        local taxPercent = tonumber(ARGV[4])
        local sellerPrefix = ARGV[5]

        if redis.call('EXISTS', key) == 0 then
          redis.call('ZREM', activeKey, listingId)
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('HGET', key, 'status') ~= 'active' then
          redis.call('ZREM', activeKey, listingId)
          return cjson.encode({ ok = false, code = 'not_active' })
        end
        local endTime = tonumber(redis.call('HGET', key, 'endTime') or '0')
        if endTime == 0 or now < endTime then
          return cjson.encode({ ok = false, code = 'not_due' })
        end
        local source = tonumber(redis.call('HGET', key, 'source') or '1')
        local seller = redis.call('HGET', key, 'sellerId') or ''
        local mode = tonumber(redis.call('HGET', key, 'saleMode') or '1')
        local top = redis.call('HGET', key, 'topBidderId') or ''
        local current = tonumber(redis.call('HGET', key, 'currentPrice') or '0')
        local currency = redis.call('HGET', key, 'currencyItemId') or ''
        local items = cjson.decode(redis.call('HGET', key, 'items') or '[]')
        local orderId = 'settle:' .. listingId
        local fee = 0

        if top ~= '' then
          redis.call('HSET', key, 'status', 'sold')
          redis.call('ZREM', activeKey, listingId)
          if source == 2 and seller ~= '' then
            redis.call('SREM', sellerPrefix .. seller, listingId)
          end
          local index = 0
          for _, item in ipairs(items) do
            index = index + 1
            local delivery = {
              deliveryId = orderId .. ':winner:' .. index,
              kind = 'item',
              itemId = item.itemId,
              count = tonumber(item.count),
              reason = 'auction_won',
              listingId = listingId,
              orderId = orderId,
              createTime = now,
            }
            redis.call('HSET', deliveryPrefix .. top, delivery.deliveryId, cjson.encode(delivery))
          end
          if source == 2 then
            fee = math.floor(current * taxPercent / 100)
            local net = current - fee
            if net > 0 then
              local delivery = {
                deliveryId = orderId .. ':seller',
                kind = 'currency',
                itemId = currency,
                count = net,
                reason = 'auction_proceeds',
                listingId = listingId,
                orderId = orderId,
                createTime = now,
              }
              redis.call('HSET', deliveryPrefix .. seller, delivery.deliveryId, cjson.encode(delivery))
            end
          end
          return cjson.encode({ ok = true, settled = 'sold', winner = top, source = source,
                                seller = seller, price = current, fee = fee })
        end

        redis.call('HSET', key, 'status', 'expired')
        redis.call('ZREM', activeKey, listingId)
        if source == 2 and seller ~= '' then
          redis.call('SREM', sellerPrefix .. seller, listingId)
          local stock = 1
          if mode == 1 then
            stock = tonumber(redis.call('HGET', key, 'stock') or '0')
            if stock < 0 then
              stock = 0
            end
          end
          if stock > 0 then
            local index = 0
            for _, item in ipairs(items) do
              index = index + 1
              local delivery = {
                deliveryId = orderId .. ':return:' .. index,
                kind = 'item',
                itemId = item.itemId,
                count = tonumber(item.count) * stock,
                reason = 'listing_expired',
                listingId = listingId,
                orderId = orderId,
                createTime = now,
              }
              redis.call('HSET', deliveryPrefix .. seller, delivery.deliveryId, cjson.encode(delivery))
            end
          end
        end
        return cjson.encode({ ok = true, settled = 'expired', winner = '', price = 0, fee = 0 })
        """;
}
