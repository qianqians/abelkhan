using consts;
using core;
using engine;
using hub.inventory;

namespace hub;

/// <summary>
/// 玩家实体的背包协议（查询 / 移动 / 整理 / 丢弃 / 使用）与变更推送。
/// 背包数据在 <see cref="hub.inventory.IBag"/>，这里只做协议解析和结果拼装。
/// 使用 / 丢弃的具体规则由 <see cref="Service"/> 的钩子决定。
/// </summary>
public abstract partial class Player
{
    internal void RegisterBagRequests(Service service)
    {
        // 背包是玩家核心模块，不依赖外部服务，始终注册。
        // 变更推送覆盖邮件附件、商城结算、道具使用等所有会改动背包的路径。
        BagChanged += changes => _ = NotifyBagChangedAsync((ulong)Bag.Version, changes);

        RegisterRequest<ClientBagInfo, ClientBagInfoResult>(Consts.ClientBagInfo, _ =>
            Result<ClientBagInfoResult, string>.Ok(BuildBagInfo()));

        RegisterRequest<ClientBagMove, ClientBagMoveResult>(Consts.ClientBagMove, req =>
        {
            var moved = Bag.Move((int)req.FromSlot, (int)req.ToSlot);
            return moved.Failed
                ? Result<ClientBagMoveResult, string>.Err(moved.Message)
                : Result<ClientBagMoveResult, string>.Ok(new ClientBagMoveResult { Bag = BuildBagInfo() });
        });

        RegisterRequest<ClientBagSort, ClientBagSortResult>(Consts.ClientBagSort, _ =>
        {
            var sorted = Bag.Sort();
            return sorted.Failed
                ? Result<ClientBagSortResult, string>.Err(sorted.Message)
                : Result<ClientBagSortResult, string>.Ok(new ClientBagSortResult { Bag = BuildBagInfo() });
        });

        RegisterRequest<ClientBagDiscard, ClientBagDiscardResult>(Consts.ClientBagDiscard, req =>
        {
            var bag = Bag;
            if (req.Slot >= (uint)bag.Capacity)
            {
                return Result<ClientBagDiscardResult, string>.Err($"格位越界: {req.Slot}");
            }

            var item = bag[(int)req.Slot];
            if (item == null)
            {
                return Result<ClientBagDiscardResult, string>.Err($"格位 {req.Slot} 为空");
            }

            var count = req.Count <= 0 ? item.Count : req.Count;
            if (count > item.Count)
            {
                return Result<ClientBagDiscardResult, string>.Err(
                    $"格位 {req.Slot} 只有 {item.Count} 个，无法丢弃 {count}");
            }

            var denied = service.CheckDiscardBagItem(this, item, count);
            if (!string.IsNullOrEmpty(denied))
            {
                return Result<ClientBagDiscardResult, string>.Err(denied);
            }

            var removed = count >= item.Count
                ? bag.RemoveAt((int)req.Slot)
                : bag.RemoveAt((int)req.Slot, count);
            return removed.Failed
                ? Result<ClientBagDiscardResult, string>.Err(removed.Message)
                : Result<ClientBagDiscardResult, string>.Ok(new ClientBagDiscardResult { Bag = BuildBagInfo() });
        });

        RegisterRequest<ClientBagUse, ClientBagUseResult>(Consts.ClientBagUse, async req =>
        {
            var bag = Bag;
            if (req.Slot >= (uint)bag.Capacity)
            {
                return Result<ClientBagUseResult, string>.Err($"格位越界: {req.Slot}");
            }

            var item = bag[(int)req.Slot];
            if (item == null)
            {
                return Result<ClientBagUseResult, string>.Err($"格位 {req.Slot} 为空");
            }

            var count = req.Count <= 0 ? 1 : req.Count;
            if (count > item.Count)
            {
                return Result<ClientBagUseResult, string>.Err(
                    $"格位 {req.Slot} 只有 {item.Count} 个，无法使用 {count}");
            }

            var used = await service.UseBagItemAsync(this, item, count);
            if (used.IsErr)
            {
                return Result<ClientBagUseResult, string>.Err(used.Error);
            }

            return Result<ClientBagUseResult, string>.Ok(new ClientBagUseResult
            {
                Bag = BuildBagInfo(),
                Message = used.Value,
            });
        });
    }

    /// <summary>拼装完整背包视图（只包含非空格）。</summary>
    private ClientBagInfoResult BuildBagInfo()
    {
        var bag = Bag;
        var info = new ClientBagInfoResult
        {
            Capacity = (uint)bag.Capacity,
            OccupiedSlots = (uint)bag.OccupiedSlots,
            Version = (ulong)bag.Version,
        };

        for (var slot = 0; slot < bag.Capacity; slot++)
        {
            if (bag[slot] is { } item)
            {
                info.Slots.Add(ToBagSlot(slot, item));
            }
        }

        return info;
    }

    private async Task NotifyBagChangedAsync(ulong version, IReadOnlyList<BagSlotChange> changes)
    {
        // 离线（读档 / 离线补偿）改动没有连接可推，等客户端下次登录拉 ClientBagInfo 即可。
        if (!_clients.ContainsKey(UserId))
        {
            return;
        }

        var notify = new ClientBagChangedNotify { Version = version };
        foreach (var change in changes)
        {
            notify.Slots.Add(new ClientBagSlot
            {
                Slot = (uint)change.Slot,
                ItemId = change.ItemId ?? string.Empty,
                Count = change.Count,
            });
        }

        try
        {
            await Notify(UserId, Consts.ClientBagChangedNotify, notify);
        }
        catch (Exception e)
        {
            Log.Error("notify bag changed user:{0} err:{1}", UserId, e);
        }
    }

    private static ClientBagSlot ToBagSlot(int slot, IItem item) => new()
    {
        Slot = (uint)slot,
        ItemId = item.ItemId,
        Count = item.Count,
    };
}
