namespace hub.inventory;

/// <summary>
/// 物品大类。背包只根据它决定能否堆叠；具体业务含义由游戏侧解释。
/// </summary>
public enum ItemType : byte
{
    /// <summary>可堆叠的普通道具 / 货币。</summary>
    Stackable = 0,

    /// <summary>装备：默认不可堆叠，一个格子放一件。</summary>
    Equipment = 1,

    /// <summary>任务道具：默认不可堆叠，避免任务进度按数量误判。</summary>
    Quest = 2,
}
