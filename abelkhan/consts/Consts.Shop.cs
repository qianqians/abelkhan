namespace consts;

/// <summary>
/// 商城协议方法名 + Redis key（对应 proto/shop.proto）
/// </summary>
public partial class Consts
{
    // ---- 商城 / 寄拍 / 限时拍卖（对应 shop.proto 的 ClientShop* 消息） ----
    public const string ClientShopList = "client_shop_list";
    public const string ClientShopBuy = "client_shop_buy";
    public const string ClientShopBid = "client_shop_bid";
    public const string ClientShopConsign = "client_shop_consign";
    public const string ClientShopCancel = "client_shop_cancel";
    public const string ClientShopClaim = "client_shop_claim";

    // ---- shop / 寄拍 ----
    // 所有 shop key 都带 {shop} hash tag：Lua 脚本可以安全地跨多个 key 操作
    // （Redis Cluster 下这些 key 落在同一个 slot；单机 Redis 下 hash tag 只是普通字符）。
    // 注意：下面常量里的 {shop} 写成 {{shop}} 是为了 string.Format 转义，
    // 实际 key 里是 {shop}。通过 shop.ShopKeys 统一生成，不要手写 key。
    // 单条商品/寄拍的 hash key（{0} = listingId）
    public const string ShopListingKey = "shop:{{shop}}:listing:{0}";
    // 全部商品/寄拍 id 集合
    public const string ShopListingsKey = "shop:{{shop}}:listings";
    // 待结算（按 end_time 排序）的 zset，hub 扫描它结算过期拍卖
    public const string ShopActiveKey = "shop:{{shop}}:active";
    // 卖家名下的寄拍 id 集合（{0} = playerId）
    public const string ShopSellerListingsKey = "shop:{{shop}}:seller:{0}";
    // 单个 listing 的每人购买计数（{0} = listingId）
    public const string ShopPurchasedKey = "shop:{{shop}}:purchased:{0}";
    // 玩家待领取的结算物品/货币（{0} = playerId）
    public const string ShopDeliveryKey = "shop:{{shop}}:delivery:{0}";
    // 成交/结算记录（Redis list，admin 查询）
    public const string ShopOrdersKey = "shop:{{shop}}:orders";
}
