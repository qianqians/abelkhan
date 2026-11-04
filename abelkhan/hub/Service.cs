using System.Collections.Concurrent;
using core;
using hub.inventory;
using leaderboard;
using shop;

namespace hub;

public abstract class Service(ConcurrentDictionary<string, Client> clients)
{
    private readonly Group _group = new();

    /// <summary>
    /// 全局邮件箱，由 <see cref="MainClass"/> 在启动时注入（与 hub 共用同一个 Redis）。
    /// 未启用邮件时保持为 null，客户端邮件请求会返回空列表。
    /// </summary>
    public GlobalMailBox? GlobalMailBox { get; set; }

    /// <summary>
    /// 商城存储（氪金商品 / 寄拍 / 限时拍卖）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册商城协议（客户端请求会得到 method not register 错误）。
    /// </summary>
    public ShopStore? Shop { get; set; }

    /// <summary>
    /// 排行榜存储（Redis ZSET）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册排行榜协议。
    /// </summary>
    public LeaderboardStore? Leaderboard { get; set; }

    /// <summary>物品配置表；创建出的玩家会继承该配置。为 null 时允许任意 item_id。</summary>
    public IItemCatalog? ItemCatalog { get; set; }

    /// <summary>新建玩家的背包容量。</summary>
    public int BagCapacity { get; set; } = Player.DefaultBagCapacity;

    protected abstract Player CreateEntity(string gateName, string cliConnId, byte[] info);

    /// <summary>
    /// 玩家创建后的配置钩子。默认注入物品配置 / 背包容量；
    /// 游戏可以重写来做读档、恢复背包等初始化（读档前先调用 base.ConfigurePlayer(player)，
    /// 或自行设置好 ItemCatalog / BagCapacity）。
    /// </summary>
    protected virtual void ConfigurePlayer(Player player)
    {
        player.ItemCatalog = ItemCatalog;
        player.BagCapacity = BagCapacity;
    }

    /// <summary>
    /// 游戏提供：该玩家当前的邮件领取条件。返回 null 表示该玩家暂不可领取邮件。
    /// level = 玩家等级，createDays = 建角天数。
    /// </summary>
    protected internal virtual (int Level, int CreateDays)? GetMailCondition(string userId) => null;

    /// <summary>
    /// 游戏提供：按 userId 批量解析排行榜条目的展示信息（昵称 / 头像）。
    /// 默认返回空字典（条目展示字段为空）；范围拉榜会一次批量解析区间内所有 member。
    /// </summary>
    protected internal virtual Task<IReadOnlyDictionary<string, LeaderboardDisplay>> ResolveLeaderboardDisplayAsync(
        IReadOnlyCollection<string> userIds) =>
        Task.FromResult<IReadOnlyDictionary<string, LeaderboardDisplay>>(
            new Dictionary<string, LeaderboardDisplay>());

    public virtual async Task<Player> EchoQueryServiceEntity(string gateName, string cliConnId, byte[] info)
    {
        var player = CreateEntity(gateName, cliConnId, info);
        // 先注入物品配置 / 背包容量，再注册邮件 / 商城协议（都要用到背包）
        ConfigurePlayer(player);
        // 给玩家实体挂上「客户端请求领取邮件」协议的处理
        player.RegisterMailRequest(this);
        // 商城：氪金商品购买、玩家寄拍、限时拍卖
        player.RegisterShopRequests(this);
        // 排行榜：打榜 / 拉榜区间 / 查名次 / 重置
        player.RegisterLeaderboardRequests(this);
        var cli = clients.AddOrUpdate(player.UserId, key => new Client(player.UserId, gateName, cliConnId), (key, old) => old);
        await player.CreateRemotePlayer(cli);
        await _group.CreateRemotePlayer(player);
        return player;
    }

    public virtual async Task<List<BaseEntity>?> EchoQueryServiceExt(List<(string, string, byte[])> infoData)
    {
        try
        {
            var lEntities = new List<BaseEntity>();
            foreach (var (gateName, cliConnId, info) in infoData)
            {
                lEntities.Add(await EchoQueryServiceEntity(gateName, cliConnId, info));
            }
            return lEntities;
        }
        catch (Exception ex)
        {
            Log.Error($"EchoQueryServiceExt err:{ex}");
        }
        return null;
    }
}