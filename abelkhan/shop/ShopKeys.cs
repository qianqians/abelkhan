using consts;

namespace shop;

/// <summary>
/// shop 用到的 Redis key。所有 key 共用 <c>{shop}</c> hash tag，
/// Lua 脚本里可以安全地同时读写「商品 / 待领取 / 计数器」等多个 key。
/// </summary>
public sealed class ShopKeys
{
    private readonly string _prefix;

    public ShopKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Contains('{') || prefix.Contains('}'))
        {
            throw new ArgumentException("ShopSettings.KeyPrefix 不能包含 '{' 或 '}'（会破坏 hash tag）", nameof(prefix));
        }

        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    public string Listing(string listingId) =>
        _prefix + string.Format(Consts.ShopListingKey, listingId);

    public string Listings => _prefix + string.Format(Consts.ShopListingsKey);

    public string Active => _prefix + string.Format(Consts.ShopActiveKey);

    public string SellerListings(string playerId) =>
        _prefix + string.Format(Consts.ShopSellerListingsKey, playerId);

    /// <summary>Lua 里按卖家 id 拼 key 时使用的前缀（以 ':' 结尾）。</summary>
    public string SellerPrefix => _prefix + "shop:{shop}:seller:";

    public string Purchased(string listingId) =>
        _prefix + string.Format(Consts.ShopPurchasedKey, listingId);

    public string Delivery(string playerId) =>
        _prefix + string.Format(Consts.ShopDeliveryKey, playerId);

    /// <summary>Lua 里需要按玩家 id 拼 key 时使用的前缀（以 ':' 结尾）。</summary>
    public string DeliveryPrefix => _prefix + "shop:{shop}:delivery:";

    /// <summary>单个操作（orderId）的幂等标记：Redis 超时重试时避免重复发货。</summary>
    public string Job(string orderId) => _prefix + "shop:{shop}:job:" + orderId;

    public string Orders => _prefix + string.Format(Consts.ShopOrdersKey);
}
