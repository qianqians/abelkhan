using consts;
using engine;

namespace client;

/// <summary>
/// 商城协议（氪金商品购买 / 玩家寄拍 / 限时拍卖）。
/// </summary>
public abstract partial class Player
{
    // ---------------- 商城（氪金商品 / 玩家寄拍 / 限时拍卖） ----------------

    /// <summary>浏览商城。category 为空 = 全部分类；mine = 只看自己寄拍的。</summary>
    public Task<Result<ClientShopListResult, string>> RequestShopList(
        string category = "", ShopSource source = ShopSource.Unknown, bool mine = false,
        uint page = 1, uint pageSize = 20)
    {
        return Request<ClientShopList, ClientShopListResult>(
            Consts.ClientShopList,
            new ClientShopList
            {
                Category = category,
                Source = source,
                Mine = mine,
                Page = page,
                PageSize = pageSize,
            });
    }

    /// <summary>一口价购买。count = 0 按 1 份处理。</summary>
    public Task<Result<ClientShopBuyResult, string>> BuyShopItem(string listingId, uint count = 1)
    {
        return Request<ClientShopBuy, ClientShopBuyResult>(
            Consts.ClientShopBuy,
            new ClientShopBuy { ListingId = listingId, Count = count });
    }

    /// <summary>拍卖出价（总额）。自己顶价时服务端只补差价，旧保证金会退回。</summary>
    public Task<Result<ClientShopBidResult, string>> BidShopItem(string listingId, long price)
    {
        return Request<ClientShopBid, ClientShopBidResult>(
            Consts.ClientShopBid,
            new ClientShopBid { ListingId = listingId, Price = price });
    }

    /// <summary>把背包物品寄拍：一口价或限时拍卖。durationSeconds = 0 使用服务端默认时长。</summary>
    public Task<Result<ClientShopConsignResult, string>> ConsignShopItem(string itemId, uint count,
        ShopSaleMode saleMode, long price = 0, long startPrice = 0, long minIncrement = 0,
        uint durationSeconds = 0, string title = "", string description = "")
    {
        return Request<ClientShopConsign, ClientShopConsignResult>(
            Consts.ClientShopConsign,
            new ClientShopConsign
            {
                ItemId = itemId,
                Count = count,
                SaleMode = saleMode,
                Price = price,
                StartPrice = startPrice,
                MinIncrement = minIncrement,
                DurationSeconds = durationSeconds,
                Title = title,
                Description = description,
            });
    }

    /// <summary>取消自己还没出价的寄拍（一口价 / 拍卖均可）。</summary>
    public Task<Result<ClientShopCancelResult, string>> CancelShopConsign(string listingId)
    {
        return Request<ClientShopCancel, ClientShopCancelResult>(
            Consts.ClientShopCancel,
            new ClientShopCancel { ListingId = listingId });
    }

    /// <summary>领取待领取的结算物品 / 货款 / 退款（进商城时也会自动尝试领取）。</summary>
    public Task<Result<ClientShopClaimResult, string>> ClaimShopDeliveries()
    {
        return Request<ClientShopClaim, ClientShopClaimResult>(
            Consts.ClientShopClaim,
            new ClientShopClaim());
    }
}
