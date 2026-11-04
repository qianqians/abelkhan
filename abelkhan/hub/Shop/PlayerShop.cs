using consts;
using engine;

namespace hub;

/// <summary>
/// 玩家实体的商城协议处理（氪金商品 + 玩家寄拍 + 限时拍卖）。
/// 具体交易逻辑在 <see cref="hub.shop.ShopTrader"/>，这里只做协议解析和结果拼装。
/// </summary>
public abstract partial class Player
{
    internal void RegisterShopRequests(Service service)
    {
        var store = service.Shop;
        if (store == null)
        {
            return;
        }

        var trader = new hub.shop.ShopTrader(store, CreateItem);

        RegisterRequest<ClientShopList, ClientShopListResult>(Consts.ClientShopList, async req =>
            Result<ClientShopListResult, string>.Ok(await trader.ListAsync(UserId, Bag, req)));

        RegisterRequest<ClientShopBuy, ClientShopBuyResult>(Consts.ClientShopBuy,
            req => trader.BuyAsync(UserId, Bag, req.ListingId, req.Count));

        RegisterRequest<ClientShopBid, ClientShopBidResult>(Consts.ClientShopBid,
            req => trader.BidAsync(UserId, Bag, req.ListingId, req.Price));

        RegisterRequest<ClientShopConsign, ClientShopConsignResult>(Consts.ClientShopConsign,
            req => trader.ConsignAsync(UserId, Bag, req));

        RegisterRequest<ClientShopCancel, ClientShopCancelResult>(Consts.ClientShopCancel,
            req => trader.CancelAsync(UserId, Bag, req.ListingId));

        RegisterRequest<ClientShopClaim, ClientShopClaimResult>(Consts.ClientShopClaim, async _ =>
        {
            var claim = await trader.ClaimAsync(UserId, Bag);
            var result = new ClientShopClaimResult { Message = claim.Message };
            result.Granted.AddRange(claim.Granted);
            return Result<ClientShopClaimResult, string>.Ok(result);
        });
    }
}
