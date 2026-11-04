using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace shop;

public enum ListingMode
{
    Fixed = 1,
    Auction = 2,
}

public enum ListingSource
{
    Official = 1,
    Player = 2,
}

public static class ListingStatuses
{
    public const string Active = "active";
    public const string Sold = "sold";
    public const string Expired = "expired";
    public const string Cancelled = "cancelled";

    public static bool IsTerminal(string status) =>
        status is Sold or Expired or Cancelled;
}

/// <summary>物品/货币条目：item_id + 数量。数量必须大于 0。</summary>
public sealed class ShopItemGrant
{
    public string ItemId { get; set; } = string.Empty;
    public long Count { get; set; }

    public ShopItemGrant()
    {
    }

    public ShopItemGrant(string itemId, long count)
    {
        ItemId = itemId;
        Count = count;
    }

    public ShopItemGrant Clone() => new(ItemId, Count);
}

/// <summary>
/// admin 后台维护的商品配置。真正对玩家可见的是一份由它生成的 <see cref="ShopListing"/>。
/// </summary>
public sealed class ShopProduct
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ListingMode SaleMode { get; set; } = ListingMode.Fixed;
    public string CurrencyItemId { get; set; } = string.Empty;

    /// <summary>一口价（单份）。</summary>
    public long Price { get; set; }

    /// <summary>拍卖起拍价。</summary>
    public long StartPrice { get; set; }

    /// <summary>拍卖最小加价。</summary>
    public long MinIncrement { get; set; } = 1;

    /// <summary>拍卖（或自动续拍时）的时长，秒；0 = 使用服务端默认值。</summary>
    public int AuctionDurationSeconds { get; set; }

    /// <summary>拍卖结束后自动开下一场（直到 <see cref="EndTime"/> 或下架）。</summary>
    public bool AutoRestart { get; set; }

    /// <summary>一口价库存，-1 = 不限。</summary>
    public int Stock { get; set; } = -1;

    /// <summary>每人限购，0 = 不限。</summary>
    public int PerUserLimit { get; set; }

    public bool Enabled { get; set; } = true;
    public int Sort { get; set; }

    /// <summary>开售时间，0 = 上架即刻开售。</summary>
    public long StartTime { get; set; }

    /// <summary>停售时间，0 = 不限时。</summary>
    public long EndTime { get; set; }

    public List<ShopItemGrant> Items { get; set; } = new();
}

/// <summary>
/// 一条对玩家可见的售卖记录：官方商品上架后生成一条，玩家寄拍也生成一条。
/// 存储上是一个 Redis hash（字段名见 <see cref="ShopStore"/>）。
/// </summary>
public sealed class ShopListing
{
    public string ListingId { get; set; } = string.Empty;
    public ListingSource Source { get; set; } = ListingSource.Official;
    public string ProductId { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public ListingMode SaleMode { get; set; } = ListingMode.Fixed;
    public string CurrencyItemId { get; set; } = string.Empty;
    public long Price { get; set; }
    public long StartPrice { get; set; }
    public long MinIncrement { get; set; } = 1;
    public long CurrentPrice { get; set; }
    public string TopBidderId { get; set; } = string.Empty;
    public int Stock { get; set; } = 1;
    public int PerUserLimit { get; set; }
    public List<ShopItemGrant> Items { get; set; } = new();
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public string Status { get; set; } = ListingStatuses.Active;
    public bool AutoRestart { get; set; }
    public int AuctionDurationSeconds { get; set; }
    public long SaleEndTime { get; set; }
    public long CreateTime { get; set; }
    public long SoldCount { get; set; }
    public int Sort { get; set; }

    public bool IsActiveAt(long now) =>
        Status == ListingStatuses.Active && StartTime <= now && (EndTime == 0 || now < EndTime);

    public long MinBid()
    {
        if (TopBidderId.Length == 0)
        {
            return Math.Max(1, StartPrice);
        }

        return CurrentPrice + Math.Max(1, MinIncrement);
    }

    public ShopListing Clone() => new()
    {
        ListingId = ListingId,
        Source = Source,
        ProductId = ProductId,
        SellerId = SellerId,
        Title = Title,
        Description = Description,
        Icon = Icon,
        Category = Category,
        SaleMode = SaleMode,
        CurrencyItemId = CurrencyItemId,
        Price = Price,
        StartPrice = StartPrice,
        MinIncrement = MinIncrement,
        CurrentPrice = CurrentPrice,
        TopBidderId = TopBidderId,
        Stock = Stock,
        PerUserLimit = PerUserLimit,
        Items = Items.Select(i => i.Clone()).ToList(),
        StartTime = StartTime,
        EndTime = EndTime,
        Status = Status,
        AutoRestart = AutoRestart,
        AuctionDurationSeconds = AuctionDurationSeconds,
        SaleEndTime = SaleEndTime,
        CreateTime = CreateTime,
        SoldCount = SoldCount,
        Sort = Sort,
    };
}

/// <summary>
/// 玩家待领取的结算（拍卖中标物品、寄拍货款、被超价退还的货币、过期退回的物品）。
/// 玩家下一次请求商城 / 主动领取时由 <see cref="ShopStore.TakeDeliveriesAsync"/> 放进背包。
/// </summary>
public sealed class ShopDelivery
{
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>item / currency</summary>
    public string Kind { get; set; } = "item";

    public string ItemId { get; set; } = string.Empty;
    public long Count { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ListingId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public long CreateTime { get; set; }

    public ShopItemGrant ToGrant() => new(ItemId, Count);
}

/// <summary>成交 / 结算流水，存 Redis list 供 admin 查询。</summary>
public sealed class ShopOrder
{
    public string OrderId { get; set; } = string.Empty;

    /// <summary>buy / auction / expire / cancel</summary>
    public string Type { get; set; } = string.Empty;

    public string ListingId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string CurrencyItemId { get; set; } = string.Empty;
    public long Price { get; set; }
    public long Fee { get; set; }
    public List<ShopItemGrant> Items { get; set; } = new();
    public long CreateTime { get; set; }
}

/// <summary>一次到期结算的结果（写流水 / 官方拍卖自动续拍时使用）。</summary>
public sealed record ShopSettlement(
    ShopListing Listing,
    string Type,
    string WinnerId,
    long Price,
    long Fee);

/// <summary>商品/寄拍列表查询条件。</summary>
public sealed record ShopListingQuery(
    string? Category = null,
    ListingSource? Source = null,
    string? SellerId = null,
    string? Status = null,
    bool IncludeInactive = false,
    int Skip = 0,
    int Take = 50);

/// <summary>商城操作的统一返回：Ok=false 时 Code 是稳定的错误码，Message 给日志/调试用。</summary>
public sealed record ShopResult(bool Ok, string Code, string Message, ShopListing? Listing = null, long Amount = 0)
{
    public static ShopResult Success(ShopListing? listing = null, string message = "", long amount = 0) =>
        new(true, string.Empty, message, listing, amount);

    public static ShopResult Fail(string code, string message) => new(false, code, message);
}

internal static class ShopJson
{
    public static readonly JsonSerializerSettings CamelCase = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static string Items(IReadOnlyList<ShopItemGrant> items) =>
        JsonConvert.SerializeObject(items, CamelCase);

    public static List<ShopItemGrant> ParseItems(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<ShopItemGrant>();
        }

        try
        {
            return JsonConvert.DeserializeObject<List<ShopItemGrant>>(json) ?? new List<ShopItemGrant>();
        }
        catch
        {
            return new List<ShopItemGrant>();
        }
    }

    public static string Delivery(ShopDelivery delivery) =>
        JsonConvert.SerializeObject(delivery);

    public static ShopDelivery? ParseDelivery(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<ShopDelivery>(json);
        }
        catch
        {
            return null;
        }
    }

    public static JObject ParseLua(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new JObject();
        }

        try
        {
            return JObject.Parse(json);
        }
        catch
        {
            return new JObject();
        }
    }

    public static string Str(long value) => value.ToString(CultureInfo.InvariantCulture);

    public static long Long(string? value, long fallback = 0)
    {
        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : fallback;
    }

    public static int Int(string? value, int fallback = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : fallback;
    }

    public static bool Bool(string? value) => value is "1" or "true" or "True";
}
