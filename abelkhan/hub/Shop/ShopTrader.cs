using core;
using engine;
using hub.inventory;
using ListingMode = shop.ListingMode;
using ListingSource = shop.ListingSource;
using ListingStatuses = shop.ListingStatuses;
using ShopDelivery = shop.ShopDelivery;
using ShopGrant = shop.ShopItemGrant;
using ShopListing = shop.ShopListing;
using ShopListingQuery = shop.ShopListingQuery;
using ShopOrder = shop.ShopOrder;
using ShopSettings = shop.ShopSettings;
using ShopStore = shop.ShopStore;

namespace hub.shop;

/// <summary>领取结算的结果。</summary>
public sealed record ShopClaimOutcome(
    List<ShopItemGrant> Granted,
    string Message,
    long Remaining);

/// <summary>
/// 玩家侧的商城交易逻辑：把「背包（本地）」和「商品状态（Redis）」串起来。
///
/// 所有涉及钱的操作顺序都是：
///   1. 先扣玩家背包（本地原子）；
///   2. 再调 <see cref="ShopStore"/> 的 Lua 原子操作抢占商品；
///   3. 抢占失败就把钱退回背包（放不下则进待领取队列）。
/// 这样最坏情况是「扣了钱但没买到」会在秒级内自动退款，而不会出现白拿商品。
/// </summary>
public sealed class ShopTrader
{
    private readonly ShopStore _store;
    private readonly Func<string, long, IItem?> _createItem;

    public ShopTrader(ShopStore store, Func<string, long, IItem?> createItem)
    {
        _store = store;
        _createItem = createItem;
    }

    public ShopSettings Settings => _store.Settings;

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>Lua 用 double 表示数字，金额超过 2^53 会丢精度，这里统一卡一个上限。</summary>
    private const long MaxAmount = 90_000_000_000_000;

    // ================================================================
    // 查询
    // ================================================================

    public async Task<ClientShopListResult> ListAsync(string playerId, IBag bag, ClientShopList req)
    {
        var now = Now();

        // 进商城时顺手把待领取结算放进背包（背包满则保留，提示玩家整理后再领）
        var claim = await ClaimAsync(playerId, bag);

        var page = Math.Clamp(req.Page == 0 ? 1u : req.Page, 1u, 10_000u);
        var pageSize = req.PageSize == 0 ? 20u : Math.Clamp(req.PageSize, 1u, 100u);
        var source = req.Source switch
        {
            ShopSource.Official => (ListingSource?)ListingSource.Official,
            ShopSource.Player => ListingSource.Player,
            _ => null,
        };

        var (total, listings) = await _store.QueryListingsAsync(new ShopListingQuery(
            Category: string.IsNullOrWhiteSpace(req.Category) ? null : req.Category.Trim(),
            Source: source,
            SellerId: req.Mine ? playerId : null,
            Status: ListingStatuses.Active,
            IncludeInactive: false,
            Skip: (int)((page - 1) * pageSize),
            Take: (int)pageSize), now);

        var result = new ClientShopListResult
        {
            Total = (uint)total,
            Page = page,
            PageSize = pageSize,
            CurrencyItemId = Settings.CurrencyItemId,
            ServerTime = (ulong)now,
            PendingDeliveries = (uint)claim.Remaining,
        };

        foreach (var listing in listings)
        {
            result.Listings.Add(await ToInfoAsync(listing, playerId, now));
        }

        return result;
    }

    // ================================================================
    // 一口价购买
    // ================================================================

    public async Task<Result<ClientShopBuyResult, string>> BuyAsync(string playerId, IBag bag,
        string listingId, uint requestedCount)
    {
        var count = requestedCount == 0 ? 1 : requestedCount;
        if (count > Settings.MaxBuyCount)
        {
            return Result<ClientShopBuyResult, string>.Err($"一次最多购买 {Settings.MaxBuyCount} 份");
        }

        var now = Now();
        var listing = await _store.GetListingAsync(listingId);
        if (listing == null)
        {
            return Result<ClientShopBuyResult, string>.Err("商品不存在");
        }

        if (!listing.IsActiveAt(now))
        {
            return Result<ClientShopBuyResult, string>.Err("商品已下架或已结束");
        }

        if (listing.SaleMode != ListingMode.Fixed)
        {
            return Result<ClientShopBuyResult, string>.Err("该商品是拍卖，请出价");
        }

        if (listing.Price < 0 || listing.Price > MaxAmount)
        {
            return Result<ClientShopBuyResult, string>.Err("商品价格异常");
        }

        if (!string.IsNullOrEmpty(listing.SellerId) && listing.SellerId == playerId)
        {
            return Result<ClientShopBuyResult, string>.Err("不能购买自己寄拍的物品");
        }

        if (listing.Stock >= 0 && count > listing.Stock)
        {
            return Result<ClientShopBuyResult, string>.Err("库存不足");
        }

        long cost;
        try
        {
            cost = checked(listing.Price * count);
        }
        catch (OverflowException)
        {
            return Result<ClientShopBuyResult, string>.Err("价格溢出");
        }

        if (cost > 0)
        {
            var pay = bag.Consume(listing.CurrencyItemId, cost);
            if (pay.Failed)
            {
                return Result<ClientShopBuyResult, string>.Err(pay.Message);
            }
        }

        var orderId = Guid.NewGuid().ToString("N");
        var purchase = await _store.PurchaseAsync(listingId, playerId, (int)count, now, orderId,
            listing.Price, listing.CurrencyItemId);
        if (!purchase.Ok)
        {
            await RefundAsync(playerId, bag, listing.CurrencyItemId, cost);
            return Result<ClientShopBuyResult, string>.Err(MessageFor(purchase.Code));
        }

        var current = purchase.Listing ?? listing;
        var claim = await ClaimAsync(playerId, bag);
        var result = new ClientShopBuyResult
        {
            Listing = await ToInfoAsync(current, playerId, now),
            Message = claim.Message,
        };
        result.Delivered.AddRange(claim.Granted);

        await TryLogOrderAsync(new ShopOrder
        {
            OrderId = orderId,
            Type = "buy",
            ListingId = listingId,
            ProductId = current.ProductId,
            BuyerId = playerId,
            SellerId = current.SellerId,
            CurrencyItemId = current.CurrencyItemId,
            Price = cost,
            Fee = FeeOf(current, cost),
            Items = current.Items.Select(i => i.Clone()).ToList(),
            CreateTime = now,
        });

        return Result<ClientShopBuyResult, string>.Ok(result);
    }

    // ================================================================
    // 拍卖出价
    // ================================================================

    public async Task<Result<ClientShopBidResult, string>> BidAsync(string playerId, IBag bag,
        string listingId, long price)
    {
        var now = Now();
        var listing = await _store.GetListingAsync(listingId);
        if (listing == null)
        {
            return Result<ClientShopBidResult, string>.Err("商品不存在");
        }

        if (!listing.IsActiveAt(now))
        {
            return Result<ClientShopBidResult, string>.Err("拍卖已结束");
        }

        if (listing.SaleMode != ListingMode.Auction)
        {
            return Result<ClientShopBidResult, string>.Err("该商品是一口价，请直接购买");
        }

        if (!string.IsNullOrEmpty(listing.SellerId) && listing.SellerId == playerId)
        {
            return Result<ClientShopBidResult, string>.Err("不能给自己的寄拍出价");
        }

        var minimum = listing.MinBid();
        if (price < minimum)
        {
            return Result<ClientShopBidResult, string>.Err($"出价不能低于 {minimum}");
        }

        if (price > MaxAmount)
        {
            return Result<ClientShopBidResult, string>.Err("出价金额过大");
        }

        // 全额托管：自己顶价时，Lua 会把上一笔保证金退到待领取队列
        var escrow = price;
        if (escrow <= 0)
        {
            return Result<ClientShopBidResult, string>.Err("出价必须大于 0");
        }

        var pay = bag.Consume(listing.CurrencyItemId, escrow);
        if (pay.Failed)
        {
            return Result<ClientShopBidResult, string>.Err(pay.Message);
        }

        var orderId = Guid.NewGuid().ToString("N");
        var bid = await _store.PlaceBidAsync(listingId, playerId, price, now, orderId,
            listing.CurrencyItemId);
        if (!bid.Ok)
        {
            await RefundAsync(playerId, bag, listing.CurrencyItemId, escrow);
            return Result<ClientShopBidResult, string>.Err(MessageFor(bid.Code));
        }

        // 若上一口是自己，这里会把旧保证金领回背包
        await ClaimAsync(playerId, bag);

        var updated = bid.Listing ?? listing;
        return Result<ClientShopBidResult, string>.Ok(new ClientShopBidResult
        {
            Listing = await ToInfoAsync(updated, playerId, now),
            Required = price,
        });
    }

    // ================================================================
    // 玩家寄拍
    // ================================================================

    public async Task<Result<ClientShopConsignResult, string>> ConsignAsync(string playerId, IBag bag,
        ClientShopConsign req)
    {
        var itemId = req.ItemId?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(itemId))
        {
            return Result<ClientShopConsignResult, string>.Err("请选择要寄拍的物品");
        }

        if (req.Count == 0)
        {
            return Result<ClientShopConsignResult, string>.Err("寄拍数量必须大于 0");
        }

        var count = (long)req.Count;
        if (count > 9999)
        {
            return Result<ClientShopConsignResult, string>.Err("单次寄拍数量过多");
        }

        if (bag.GetItemCount(itemId) < count)
        {
            return Result<ClientShopConsignResult, string>.Err("背包里的物品数量不足");
        }

        ListingMode mode;
        long price;
        long startPrice;
        long minIncrement;
        switch (req.SaleMode)
        {
            case ShopSaleMode.Fixed:
                mode = ListingMode.Fixed;
                if (req.Price <= 0 || req.Price > MaxAmount)
                {
                    return Result<ClientShopConsignResult, string>.Err("一口价必须大于 0 且不能超过上限");
                }

                price = req.Price;
                startPrice = 0;
                minIncrement = 1;
                break;
            case ShopSaleMode.Auction:
                mode = ListingMode.Auction;
                if (req.StartPrice <= 0 || req.StartPrice > MaxAmount)
                {
                    return Result<ClientShopConsignResult, string>.Err("起拍价必须大于 0 且不能超过上限");
                }

                price = 0;
                startPrice = req.StartPrice;
                minIncrement = req.MinIncrement <= 0 ? 1 : Math.Min(req.MinIncrement, MaxAmount);
                break;
            default:
                return Result<ClientShopConsignResult, string>.Err("未知售卖模式");
        }

        var now = Now();
        var requestedDuration = (int)Math.Min(req.DurationSeconds, (uint)int.MaxValue);
        var duration = Settings.NormalizeDurationSeconds(requestedDuration, mode == ListingMode.Auction);

        var escrow = bag.Consume(itemId, count);
        if (escrow.Failed)
        {
            return Result<ClientShopConsignResult, string>.Err(escrow.Message);
        }

        var listing = new ShopListing
        {
            SellerId = playerId,
            Title = Clamp(string.IsNullOrWhiteSpace(req.Title) ? itemId : req.Title.Trim(), 80),
            Description = Clamp(req.Description ?? string.Empty, 500),
            Category = "player",
            SaleMode = mode,
            CurrencyItemId = Settings.MarketCurrency,
            Price = price,
            StartPrice = startPrice,
            MinIncrement = minIncrement,
            Items = new List<ShopGrant> { new(itemId, count) },
            StartTime = now,
            EndTime = now + duration * 1000L,
            AuctionDurationSeconds = mode == ListingMode.Auction ? duration : 0,
        };

        var created = await _store.CreatePlayerListingAsync(listing, now);
        if (!created.Ok)
        {
            await RefundAsync(playerId, bag, itemId, count);
            return Result<ClientShopConsignResult, string>.Err(MessageFor(created.Code));
        }

        return Result<ClientShopConsignResult, string>.Ok(new ClientShopConsignResult
        {
            Listing = await ToInfoAsync(created.Listing!, playerId, now),
        });
    }

    public async Task<Result<ClientShopCancelResult, string>> CancelAsync(string playerId, IBag bag,
        string listingId)
    {
        var now = Now();
        var cancelled = await _store.CancelAsync(listingId, playerId, force: false, now);
        if (!cancelled.Ok)
        {
            return Result<ClientShopCancelResult, string>.Err(MessageFor(cancelled.Code));
        }

        await ClaimAsync(playerId, bag);
        return Result<ClientShopCancelResult, string>.Ok(new ClientShopCancelResult
        {
            Listing = await ToInfoAsync(cancelled.Listing!, playerId, now),
        });
    }

    // ================================================================
    // 待领取结算
    // ================================================================

    public async Task<ShopClaimOutcome> ClaimAsync(string playerId, IBag bag)
    {
        var deliveries = await _store.PeekDeliveriesAsync(playerId);
        var granted = new List<ShopItemGrant>();
        var removed = new List<string>();
        var message = string.Empty;
        var hasInvalid = false;

        foreach (var delivery in deliveries)
        {
            IItem? item;
            try
            {
                item = _createItem(delivery.ItemId, delivery.Count);
            }
            catch (Exception e)
            {
                hasInvalid = true;
                Log.Error("shop claim user:{0} item:{1} create failed: {2}", playerId, delivery.ItemId, e);
                continue; // 配置问题：保留在待领取里，修好配置后还能领
            }

            if (item == null)
            {
                hasInvalid = true;
                Log.Error("shop claim user:{0} unknown item:{1}", playerId, delivery.ItemId);
                continue;
            }

            var added = bag.Add(item);
            if (added.Failed)
            {
                message = "背包空间不足，剩余结算物品暂存在商城，整理后可重新领取";
                break;
            }

            granted.Add(new ShopItemGrant { ItemId = delivery.ItemId, Count = delivery.Count });
            removed.Add(delivery.DeliveryId);
        }

        if (removed.Count > 0)
        {
            await _store.RemoveDeliveriesAsync(playerId, removed);
        }

        if (hasInvalid && message.Length == 0)
        {
            message = "部分结算物品配置异常，请联系管理员";
        }

        var remaining = await _store.PendingDeliveryCountAsync(playerId);
        return new ShopClaimOutcome(granted, message, remaining);
    }

    public Task<long> PendingDeliveriesAsync(string playerId) =>
        _store.PendingDeliveryCountAsync(playerId);

    // ================================================================
    // 内部
    // ================================================================

    private async Task RefundAsync(string playerId, IBag bag, string itemId, long count)
    {
        if (count <= 0)
        {
            return;
        }

        var added = bag.Add(itemId, count);
        if (added.Failed)
        {
            // 背包放不下时进待领取队列，保证钱不会凭空消失
            await _store.PushDeliveryAsync(playerId, new ShopDelivery
            {
                DeliveryId = Guid.NewGuid().ToString("N"),
                Kind = "currency",
                ItemId = itemId,
                Count = count,
                Reason = "shop_refund",
                CreateTime = Now(),
            });
        }
    }

    private async Task TryLogOrderAsync(ShopOrder order)
    {
        try
        {
            await _store.AppendOrderAsync(order);
        }
        catch (Exception e)
        {
            Log.Error("shop append order failed: {0}", e);
        }
    }

    private long FeeOf(ShopListing listing, long gross)
    {
        if (listing.Source != ListingSource.Player || gross <= 0)
        {
            return 0;
        }

        // 与 Lua 保持一致：向下取整
        return gross * Settings.NormalizedTaxPercent / 100;
    }

    private async Task<ShopListingInfo> ToInfoAsync(ShopListing listing, string playerId, long now)
    {
        var status = listing.Status;
        if (status == ListingStatuses.Active)
        {
            if (listing.StartTime > now)
            {
                status = "pending";
            }
            else if (listing.EndTime > 0 && now >= listing.EndTime)
            {
                status = "expired";
            }
        }

        var info = new ShopListingInfo
        {
            ListingId = listing.ListingId,
            Source = listing.Source == ListingSource.Player ? ShopSource.Player : ShopSource.Official,
            SellerId = listing.SellerId,
            ProductId = listing.ProductId,
            Title = listing.Title,
            Description = listing.Description,
            Icon = listing.Icon,
            Category = listing.Category,
            SaleMode = listing.SaleMode == ListingMode.Auction ? ShopSaleMode.Auction : ShopSaleMode.Fixed,
            CurrencyItemId = listing.CurrencyItemId,
            Price = listing.Price,
            StartPrice = listing.StartPrice,
            MinIncrement = listing.MinIncrement,
            CurrentPrice = listing.CurrentPrice,
            TopBidderId = listing.TopBidderId,
            Stock = listing.Stock,
            StartTime = (ulong)Math.Max(0, listing.StartTime),
            EndTime = (ulong)Math.Max(0, listing.EndTime),
            Status = status,
            PerUserLimit = listing.PerUserLimit,
            ServerTime = (ulong)now,
        };

        foreach (var item in listing.Items)
        {
            info.Items.Add(new ShopItemGrant { ItemId = item.ItemId, Count = item.Count });
        }

        if (listing.PerUserLimit > 0 && !string.IsNullOrEmpty(playerId))
        {
            info.MyPurchased = await _store.GetPurchasedCountAsync(listing.ListingId, playerId);
        }

        return info;
    }

    private static string Clamp(string value, int max) =>
        value.Length <= max ? value : value[..max];

    private static string MessageFor(string code) => code switch
    {
        "not_found" => "商品不存在",
        "not_active" => "商品已下架或已售出",
        "expired" => "商品已结束",
        "not_started" => "商品尚未开售",
        "self" => "不能交易自己寄拍的物品",
        "sold_out" => "库存不足",
        "limit" => "已达到限购数量",
        "not_fixed" => "该商品不是一口价",
        "not_auction" => "该商品不是拍卖",
        "price_changed" => "商品价格刚刚发生变化，请刷新后重试",
        "currency_changed" => "商品结算货币刚刚发生变化，请刷新后重试",
        "too_low" => "出价低于最低出价",
        "has_bids" => "已有出价，不能取消",
        "forbidden" => "无权操作该商品",
        "listing_limit" => "同时寄拍的数量已达上限",
        "window_closed" => "商品已过停售时间",
        "disabled" => "商品未启用",
        "invalid" => "参数不合法",
        _ => "服务器繁忙，请稍后再试",
    };
}
