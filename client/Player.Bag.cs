using consts;
using engine;

namespace client;

/// <summary>
/// 背包协议（查询 / 移动 / 整理 / 丢弃 / 使用）与服务器推送的变更事件。
/// </summary>
public abstract partial class Player
{
    // ================================================================
    // 请求
    // ================================================================

    /// <summary>拉取完整背包（容量 / 已用格数 / 版本 / 非空格列表）。</summary>
    public Task<Result<ClientBagInfoResult, string>> RequestBagInfo()
    {
        return Request<ClientBagInfo, ClientBagInfoResult>(
            Consts.ClientBagInfo,
            new ClientBagInfo());
    }

    /// <summary>移动 / 交换格位：目标为空则移动，非空则交换。</summary>
    public Task<Result<ClientBagMoveResult, string>> MoveBagItem(uint fromSlot, uint toSlot)
    {
        return Request<ClientBagMove, ClientBagMoveResult>(
            Consts.ClientBagMove,
            new ClientBagMove { FromSlot = fromSlot, ToSlot = toSlot });
    }

    /// <summary>整理背包：压缩空格 + 按 item_id 排序。</summary>
    public Task<Result<ClientBagSortResult, string>> SortBag()
    {
        return Request<ClientBagSort, ClientBagSortResult>(
            Consts.ClientBagSort,
            new ClientBagSort());
    }

    /// <summary>丢弃物品。count &lt;= 0 表示丢弃整格。</summary>
    public Task<Result<ClientBagDiscardResult, string>> DiscardBagItem(uint slot, long count = 0)
    {
        return Request<ClientBagDiscard, ClientBagDiscardResult>(
            Consts.ClientBagDiscard,
            new ClientBagDiscard { Slot = slot, Count = count });
    }

    /// <summary>使用物品。count &lt;= 0 按 1 个处理；具体效果由服务端游戏钩子实现。</summary>
    public Task<Result<ClientBagUseResult, string>> UseBagItem(uint slot, long count = 1)
    {
        return Request<ClientBagUse, ClientBagUseResult>(
            Consts.ClientBagUse,
            new ClientBagUse { Slot = slot, Count = count });
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>
    /// 背包变更：只包含本次变化的格位（空格用 ItemId = ""、Count = 0 表示）。
    /// 可以按 Version 丢弃过期通知（例如并发请求导致的乱序）。
    /// </summary>
    public event Action<ClientBagChangedNotify>? BagChanged;

    private void RegisterBagNotifications()
    {
        TryRegisterNotify<ClientBagChangedNotify>(Consts.ClientBagChangedNotify,
            msg => BagChanged?.Invoke(msg));
    }
}
