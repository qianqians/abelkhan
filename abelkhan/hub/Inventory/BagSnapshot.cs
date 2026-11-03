namespace hub.inventory;

/// <summary>
/// 背包快照：容量 + 每个格位的物品（空位为 null）。用于存档、回滚和跨进程传输。
/// 快照会复制一份格位数组，之后原背包继续变动不会影响它。
/// </summary>
public sealed class BagSnapshot
{
    public BagSnapshot(int capacity, IReadOnlyList<IItem?> slots)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "背包容量必须大于 0");
        }

        ArgumentNullException.ThrowIfNull(slots);
        if (slots.Count > capacity)
        {
            throw new ArgumentException("格位数量不能超过容量", nameof(slots));
        }

        var copy = new IItem?[capacity];
        for (var i = 0; i < slots.Count; i++)
        {
            copy[i] = slots[i];
        }

        Capacity = capacity;
        Slots = Array.AsReadOnly(copy);
    }

    /// <summary>快照对应的背包容量。</summary>
    public int Capacity { get; }

    /// <summary>只读格位列表，长度为 <see cref="Capacity"/>。</summary>
    public IReadOnlyList<IItem?> Slots { get; }

    /// <summary>非空格位数量。</summary>
    public int OccupiedSlots
    {
        get
        {
            var n = 0;
            foreach (var item in Slots)
            {
                if (item != null)
                {
                    n++;
                }
            }

            return n;
        }
    }
}
