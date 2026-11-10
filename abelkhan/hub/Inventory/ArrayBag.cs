using System.Collections.ObjectModel;
using System.Diagnostics;

namespace hub.inventory;

/// <summary>
/// 基于固定长度数组的背包实现。
///
/// 设计要点：
/// 1. 每个格位只存一个 <see cref="IItem"/> 引用，物品本身不可变；
/// 2. 添加时先填已有同类堆叠，再找空格，空间不足整体回滚（不会出现"加了一半"）；
/// 3. 消费时先校验总数，再按格位顺序扣除；
/// 4. 整理时压缩空格并按 item_id 稳定排序；
/// 5. 非线程安全，默认由 hub 的 per-entity Actor 串行访问。
/// </summary>
public sealed class ArrayBag : IBag
{
    private readonly IItem?[] _slots;
    private readonly IItemCatalog? _catalog;
    private readonly ReadOnlyCollection<IItem?> _slotsView;

    private long _version;

    /// <param name="capacity">格位数量，必须大于 0。</param>
    /// <param name="catalog">
    /// 物品配置。非空时 <see cref="Add(string,long)"/> 会校验 item_id 并补全堆叠上限；
    /// 为 null 时允许任意 item_id，按无上限堆叠处理。
    /// </param>
    public ArrayBag(int capacity, IItemCatalog? catalog = null)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "背包容量必须大于 0");
        }

        _slots = new IItem?[capacity];
        _catalog = catalog;
        _slotsView = Array.AsReadOnly(_slots);
    }

    public int Capacity => _slots.Length;

    public int OccupiedSlots
    {
        get
        {
            var n = 0;
            foreach (var item in _slots)
            {
                if (item != null)
                {
                    n++;
                }
            }

            return n;
        }
    }

    public long Version => _version;

    public IItem? this[int slot]
    {
        get
        {
            if (!IsValidSlot(slot))
            {
                throw new ArgumentOutOfRangeException(nameof(slot));
            }

            return _slots[slot];
        }
    }

    public IReadOnlyList<IItem?> Slots => _slotsView;

    public event Action<IReadOnlyList<BagSlotChange>>? Changed;

    public long GetItemCount(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        long total = 0;
        foreach (var item in _slots)
        {
            if (item != null && item.ItemId == itemId)
            {
                total += item.Count;
            }
        }

        return total;
    }

    public bool Contains(string itemId, long count = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) || count <= 0)
        {
            return false;
        }

        return GetItemCount(itemId) >= count;
    }

    public BagSnapshot Snapshot() => new BagSnapshot(Capacity, (IItem?[])_slots.Clone());

    public BagResult Add(IItem item)
    {
        var invalid = ValidateItem(item);
        if (invalid.HasValue)
        {
            return invalid.Value;
        }

        var snapshot = (IItem?[])_slots.Clone();
        var changes = new List<BagSlotChange>();
        long added;
        try
        {
            added = AddCore(item!, changes);
        }
        catch
        {
            Restore(snapshot);
            throw;
        }

        if (added < item!.Count)
        {
            Restore(snapshot);
            return BagResult.Fail(BagError.NotEnoughSpace,
                $"背包空间不足: 需要 {item.Count}，只能放下 {added}");
        }

        Commit(changes);
        return BagResult.Success(added);
    }

    public BagResult Add(string itemId, long count)
    {
        if (!TryCreateItem(itemId, count, out var item, out var error))
        {
            return error;
        }

        return Add(item);
    }

    public BagResult AddRange(IEnumerable<IItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var list = items as IReadOnlyCollection<IItem> ?? items.ToList();
        if (list.Count == 0)
        {
            return BagResult.Success();
        }

        long required = 0;
        foreach (var item in list)
        {
            var invalid = ValidateItem(item);
            if (invalid.HasValue)
            {
                return invalid.Value;
            }

            required += item.Count;
        }

        var snapshot = (IItem?[])_slots.Clone();
        var changes = new List<BagSlotChange>();
        long added = 0;
        try
        {
            foreach (var item in list)
            {
                added += AddCore(item, changes);
            }
        }
        catch
        {
            Restore(snapshot);
            throw;
        }

        if (added < required)
        {
            Restore(snapshot);
            return BagResult.Fail(BagError.NotEnoughSpace,
                $"背包空间不足: 需要 {required}，只能放下 {added}");
        }

        Commit(changes);
        return BagResult.Success(added);
    }

    public BagResult AddPartial(IItem item, out long remaining)
    {
        remaining = 0;
        var invalid = ValidateItem(item);
        if (invalid.HasValue)
        {
            return invalid.Value;
        }

        var snapshot = (IItem?[])_slots.Clone();
        var changes = new List<BagSlotChange>();
        long added;
        try
        {
            added = AddCore(item!, changes);
        }
        catch
        {
            Restore(snapshot);
            throw;
        }

        remaining = item!.Count - added;
        Commit(changes);
        return BagResult.Success(added);
    }

    public BagResult Consume(string itemId, long count)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return BagResult.Fail(BagError.InvalidArgument, "item_id 不能为空");
        }

        if (count <= 0)
        {
            return BagResult.Fail(BagError.InvalidCount, "消费数量必须大于 0");
        }

        var owned = GetItemCount(itemId);
        if (owned < count)
        {
            return BagResult.Fail(BagError.NotEnoughItems,
                $"物品不足: {itemId} 需要 {count}，当前 {owned}");
        }

        var snapshot = (IItem?[])_slots.Clone();
        var changes = new List<BagSlotChange>();
        var remaining = count;
        try
        {
            for (var i = 0; i < _slots.Length && remaining > 0; i++)
            {
                var current = _slots[i];
                if (current == null || current.ItemId != itemId)
                {
                    continue;
                }

                var take = Math.Min(current.Count, remaining);
                var left = current.Count - take;
                _slots[i] = left > 0 ? current.WithCount(left) : null;
                changes.Add(new BagSlotChange(i, _slots[i]?.ItemId, left));
                remaining -= take;
            }
        }
        catch
        {
            Restore(snapshot);
            throw;
        }

        Commit(changes);
        return BagResult.Success(count);
    }

    public BagResult RemoveAt(int slot)
    {
        if (!IsValidSlot(slot))
        {
            return BagResult.Fail(BagError.SlotOutOfRange, $"格位越界: {slot}");
        }

        var item = _slots[slot];
        if (item == null)
        {
            return BagResult.Fail(BagError.SlotEmpty, $"格位 {slot} 为空");
        }

        _slots[slot] = null;
        Commit(new List<BagSlotChange> { new BagSlotChange(slot, null, 0) });
        return BagResult.Success(item.Count);
    }

    public BagResult RemoveAt(int slot, long count)
    {
        if (!IsValidSlot(slot))
        {
            return BagResult.Fail(BagError.SlotOutOfRange, $"格位越界: {slot}");
        }

        if (count <= 0)
        {
            return BagResult.Fail(BagError.InvalidCount, "删除数量必须大于 0");
        }

        var item = _slots[slot];
        if (item == null)
        {
            return BagResult.Fail(BagError.SlotEmpty, $"格位 {slot} 为空");
        }

        if (count > item.Count)
        {
            return BagResult.Fail(BagError.NotEnoughItems,
                $"格位 {slot} 只有 {item.Count}，无法删除 {count}");
        }

        var left = item.Count - count;
        _slots[slot] = left > 0 ? item.WithCount(left) : null;
        Commit(new List<BagSlotChange> { new BagSlotChange(slot, _slots[slot]?.ItemId, left) });
        return BagResult.Success(count);
    }

    public BagResult Delete(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return BagResult.Fail(BagError.InvalidArgument, "item_id 不能为空");
        }

        var changes = new List<BagSlotChange>();
        long removed = 0;
        for (var i = 0; i < _slots.Length; i++)
        {
            var item = _slots[i];
            if (item == null || item.ItemId != itemId)
            {
                continue;
            }

            removed += item.Count;
            _slots[i] = null;
            changes.Add(new BagSlotChange(i, null, 0));
        }

        Commit(changes);
        return BagResult.Success(removed);
    }

    public BagResult Sort()
    {
        var before = (IItem?[])_slots.Clone();
        var sorted = _slots
            .Where(static item => item != null)
            .OrderBy(static item => item!.ItemId, StringComparer.Ordinal)
            .ToList();

        for (var i = 0; i < _slots.Length; i++)
        {
            _slots[i] = i < sorted.Count ? sorted[i] : null;
        }

        Commit(Diff(before));
        return BagResult.Success();
    }

    public BagResult Move(int fromSlot, int toSlot)
    {
        if (!IsValidSlot(fromSlot))
        {
            return BagResult.Fail(BagError.SlotOutOfRange, $"格位越界: {fromSlot}");
        }

        if (!IsValidSlot(toSlot))
        {
            return BagResult.Fail(BagError.SlotOutOfRange, $"格位越界: {toSlot}");
        }

        if (fromSlot == toSlot)
        {
            return BagResult.Success();
        }

        var source = _slots[fromSlot];
        if (source == null)
        {
            return BagResult.Fail(BagError.SlotEmpty, $"格位 {fromSlot} 为空");
        }

        var target = _slots[toSlot];
        _slots[fromSlot] = target;
        _slots[toSlot] = source;
        Commit(new List<BagSlotChange>
        {
            new(fromSlot, target?.ItemId, target?.Count ?? 0),
            new(toSlot, source.ItemId, source.Count),
        });
        return BagResult.Success();
    }

    public BagResult Clear()
    {
        var before = (IItem?[])_slots.Clone();
        long removed = 0;
        for (var i = 0; i < _slots.Length; i++)
        {
            removed += _slots[i]?.Count ?? 0;
            _slots[i] = null;
        }

        Commit(Diff(before));
        return BagResult.Success(removed);
    }

    public BagResult Load(BagSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.Capacity > Capacity)
        {
            return BagResult.Fail(BagError.InvalidSnapshot,
                $"快照容量 {snapshot.Capacity} 超过当前背包容量 {Capacity}");
        }

        var next = new IItem?[Capacity];
        for (var i = 0; i < snapshot.Capacity; i++)
        {
            var item = i < snapshot.Slots.Count ? snapshot.Slots[i] : null;
            if (item == null)
            {
                continue;
            }

            var invalid = ValidateItem(item);
            if (invalid.HasValue)
            {
                return BagResult.Fail(BagError.InvalidSnapshot,
                    $"快照格位 {i} 非法: {invalid.Value.Message}");
            }

            next[i] = item;
        }

        var before = (IItem?[])_slots.Clone();
        Array.Copy(next, _slots, _slots.Length);
        Commit(Diff(before));
        return BagResult.Success();
    }

    // ---------------- 内部实现 ----------------

    private static BagResult? ValidateItem(IItem? item)
    {
        if (item == null)
        {
            return BagResult.Fail(BagError.InvalidArgument, "物品不能为 null");
        }

        if (string.IsNullOrWhiteSpace(item.ItemId))
        {
            return BagResult.Fail(BagError.InvalidArgument, "item_id 不能为空");
        }

        if (item.Count <= 0)
        {
            return BagResult.Fail(BagError.InvalidCount, "物品数量必须大于 0");
        }

        return null;
    }

    private bool TryCreateItem(string itemId, long count, out IItem item, out BagResult error)
    {
        item = null!;
        error = default;

        if (string.IsNullOrWhiteSpace(itemId))
        {
            error = BagResult.Fail(BagError.InvalidArgument, "item_id 不能为空");
            return false;
        }

        if (count <= 0)
        {
            error = BagResult.Fail(BagError.InvalidCount, "物品数量必须大于 0");
            return false;
        }

        if (_catalog == null)
        {
            item = new ItemStack(itemId, count);
            return true;
        }

        if (!_catalog.TryGet(itemId, out var definition))
        {
            error = BagResult.Fail(BagError.UnknownItem, $"未知 item_id: {itemId}");
            return false;
        }

        item = definition.CreateStack(count);
        return true;
    }

    /// <summary>把一份物品放进背包，返回实际放入的数量。调用方负责在失败时回滚。</summary>
    private long AddCore(IItem item, List<BagSlotChange> changes)
    {
        var remaining = item.Count;
        var maxStack = item.Stackable
            ? (item.MaxStack <= 0 ? ItemStack.Unlimited : item.MaxStack)
            : 1;

        // 1) 先补满已有同类堆叠，避免每次添加都产生新的半格。
        for (var i = 0; i < _slots.Length && remaining > 0; i++)
        {
            var current = _slots[i];
            if (current == null || !CanMerge(current, item))
            {
                continue;
            }

            var space = maxStack - current.Count;
            if (space <= 0)
            {
                continue;
            }

            var moved = Math.Min(space, remaining);
            var updated = current.WithCount(current.Count + moved);
            _slots[i] = updated;
            changes.Add(new BagSlotChange(i, updated.ItemId, updated.Count));
            remaining -= moved;
        }

        // 2) 再使用空格。
        for (var i = 0; i < _slots.Length && remaining > 0; i++)
        {
            if (_slots[i] != null)
            {
                continue;
            }

            var chunk = Math.Min((long)maxStack, remaining);
            var stack = item.WithCount(chunk);
            _slots[i] = stack;
            changes.Add(new BagSlotChange(i, stack.ItemId, stack.Count));
            remaining -= chunk;
        }

        return item.Count - remaining;
    }

    private static bool CanMerge(IItem existing, IItem incoming) =>
        existing.Stackable &&
        incoming.Stackable &&
        existing.ItemId == incoming.ItemId &&
        existing.MaxStack == incoming.MaxStack &&
        existing.Type == incoming.Type;

    private bool IsValidSlot(int slot) => (uint)slot < (uint)_slots.Length;

    private void Restore(IItem?[] snapshot) => Array.Copy(snapshot, _slots, _slots.Length);

    private List<BagSlotChange> Diff(IItem?[] before)
    {
        var changes = new List<BagSlotChange>();
        for (var i = 0; i < _slots.Length; i++)
        {
            var previous = before[i];
            var current = _slots[i];
            if (SameItem(previous, current))
            {
                continue;
            }

            changes.Add(new BagSlotChange(i, current?.ItemId, current?.Count ?? 0));
        }

        return changes;
    }

    private static bool SameItem(IItem? a, IItem? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        return a.ItemId == b.ItemId &&
               a.Count == b.Count &&
               a.MaxStack == b.MaxStack &&
               a.Type == b.Type;
    }

    private void Commit(List<BagSlotChange> changes)
    {
        if (changes.Count == 0)
        {
            return;
        }

        _version++;

        // 事件是"通知"：订阅方抛异常不能把已经提交的背包操作变成调用方眼里的失败，
        // 否则邮件领取会因此回滚认领标记，导致物品重复发放。
        try
        {
            Changed?.Invoke(changes.AsReadOnly());
        }
        catch (Exception e)
        {
            Debug.WriteLine($"ArrayBag.Changed handler failed: {e}");
        }
    }
}
