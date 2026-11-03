namespace hub.inventory;

/// <summary>
/// 背包读写接口。实现必须保证每个操作是原子的：要么完整生效，要么背包保持原样。
///
/// 线程模型：背包本身不加锁，默认由 hub 的 per-entity Actor 串行访问；
/// 如果游戏在 Actor 之外共享同一个背包，需要自己加锁。
/// </summary>
public interface IBag : IReadOnlyBag
{
    /// <summary>
    /// 添加一份物品。空间不足时整体失败（不会部分放入）。
    /// 该重载信任调用方传入的 <paramref name="item"/>；需要按配置校验 item_id / 堆叠上限时用
    /// <see cref="Add(string,long)"/>。
    /// </summary>
    BagResult Add(IItem item);

    /// <summary>按 item_id + 数量添加。存在 catalog 时未知 item_id 会失败。</summary>
    BagResult Add(string itemId, long count);

    /// <summary>批量添加，全部成功才提交；任意一项放不下则全部回滚。</summary>
    BagResult AddRange(IEnumerable<IItem> items);

    /// <summary>
    /// 尽可能多地放入，返回实际放入数量；<paramref name="remaining"/> 为放不下的剩余数量。
    /// 适合邮件 / 掉落这类允许溢出的场景。
    /// </summary>
    BagResult AddPartial(IItem item, out long remaining);

    /// <summary>消费指定 item_id 的数量；总数不足时失败，背包不变。</summary>
    BagResult Consume(string itemId, long count);

    /// <summary>删除整个格位（装备卸下 / 丢弃 / GM 删除）。</summary>
    BagResult RemoveAt(int slot);

    /// <summary>删除某个格位中的部分数量；数量超过该格堆叠时失败。</summary>
    BagResult RemoveAt(int slot, long count);

    /// <summary>删除某个 item_id 的所有堆叠（跨格位）。返回删除的总数量；不存在时成功且 Affected = 0。</summary>
    BagResult Delete(string itemId);

    /// <summary>整理背包：压缩空格位，并按 item_id 做稳定升序排序（空位在末尾）。</summary>
    BagResult Sort();

    /// <summary>清空背包（GM / 删档）。</summary>
    BagResult Clear();

    /// <summary>用快照覆盖当前背包。快照非法或容量超过当前背包时失败，背包不变。</summary>
    BagResult Load(BagSnapshot snapshot);

    /// <summary>成功变更后触发；参数是本次发生变化的格位及其最终状态。</summary>
    event Action<IReadOnlyList<BagSlotChange>>? Changed;
}
