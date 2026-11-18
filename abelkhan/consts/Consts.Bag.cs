namespace consts;

/// <summary>
/// 背包协议方法名（对应 proto/bag.proto）
/// </summary>
public partial class Consts
{
    // ---- 背包（对应 bag.proto 的 ClientBag* 消息） ----
    public const string ClientBagInfo = "client_bag_info";
    public const string ClientBagMove = "client_bag_move";
    public const string ClientBagSort = "client_bag_sort";
    public const string ClientBagDiscard = "client_bag_discard";
    public const string ClientBagUse = "client_bag_use";

    // 背包变更通知（hub -> 客户端）
    public const string ClientBagChangedNotify = "client_bag_changed_notify";
}
