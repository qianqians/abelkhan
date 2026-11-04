namespace shop;

/// <summary>
/// 商城运行参数。hub 与 admin 必须使用同一份配置（尤其是 <see cref="KeyPrefix"/>），
/// admin 上架的商品才能被游戏服读到。
/// </summary>
public sealed class ShopSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离，不能包含 '{' / '}'。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>氪金商城默认货币（背包里的 item_id）。官方商品没填货币时使用。</summary>
    public string CurrencyItemId { get; set; } = "diamond";

    /// <summary>玩家寄拍默认货币；为空时复用 <see cref="CurrencyItemId"/>。</summary>
    public string MarketCurrencyItemId { get; set; } = string.Empty;

    /// <summary>玩家寄拍成交税（百分比，0-90）。税会从卖家货款里扣除。</summary>
    public int SaleTaxPercent { get; set; } = 5;

    /// <summary>单个玩家同时最多寄拍多少件。</summary>
    public int MaxListingsPerPlayer { get; set; } = 8;

    /// <summary>寄拍时长下限（秒）。</summary>
    public int MinListingDurationSeconds { get; set; } = 300;

    /// <summary>寄拍时长上限（秒），同时限制拍卖时长。</summary>
    public int MaxListingDurationSeconds { get; set; } = 7 * 24 * 3600;

    /// <summary>一口价寄拍默认时长（秒）。</summary>
    public int DefaultListingDurationSeconds { get; set; } = 24 * 3600;

    /// <summary>拍卖默认时长（秒）。</summary>
    public int DefaultAuctionDurationSeconds { get; set; } = 24 * 3600;

    /// <summary>一次购买的最大份数。</summary>
    public int MaxBuyCount { get; set; } = 99;

    public string MarketCurrency =>
        string.IsNullOrWhiteSpace(MarketCurrencyItemId) ? CurrencyItemId : MarketCurrencyItemId;

    public int NormalizedTaxPercent => Math.Clamp(SaleTaxPercent, 0, 90);

    /// <summary>把请求里的时长夹到配置允许的范围内；0 表示使用默认值。</summary>
    public int NormalizeDurationSeconds(int seconds, bool auction)
    {
        var fallback = auction ? DefaultAuctionDurationSeconds : DefaultListingDurationSeconds;
        if (seconds <= 0)
        {
            seconds = fallback;
        }

        var min = Math.Max(1, MinListingDurationSeconds);
        var max = Math.Max(min, MaxListingDurationSeconds);
        return Math.Clamp(seconds, min, max);
    }
}
