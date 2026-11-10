using System.Collections.Concurrent;
using chat;
using core;
using engine;
using friend;
using guild;
using hub.inventory;
using leaderboard;
using quest;
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

    /// <summary>
    /// 好友系统（Redis 权威 + 跨服确认）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册好友协议。
    /// </summary>
    public FriendService? Friend { get; set; }

    /// <summary>
    /// 聊天系统（频道跨 hub 扇出 + 点对点）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册聊天协议。
    /// </summary>
    public ChatService? Chat { get; set; }

    /// <summary>
    /// 帮会系统（创建 / 成员 / 职位 / 任务）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册帮会协议。
    /// </summary>
    public GuildService? Guild { get; set; }

    /// <summary>
    /// 任务系统（事件驱动 + 三条件 + 任务链）。由 <see cref="MainClass"/> 在启动时注入。
    /// 为 null 时不注册任务协议。
    /// </summary>
    public QuestService? Quest { get; set; }

    /// <summary>
    /// 任务目录（游戏在注册 service 前把配置注册进来）。
    /// </summary>
    public IQuestCatalog QuestCatalog { get; set; } = global::quest.QuestCatalog.Empty;

    /// <summary>
    /// 帮会任务目录（游戏在注册 service 前把默认任务注册进来）。
    /// 新建帮会时会自动发布 <c>GuildSettings.DefaultTaskIds</c> 里存在的那些。
    /// </summary>
    public IGuildTaskCatalog GuildTaskCatalog { get; set; } = global::guild.GuildTaskCatalog.Empty;

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

    /// <summary>
    /// 游戏提供：按 userId 批量解析好友条目的展示信息（昵称 / 头像）。
    /// 默认返回空字典（条目展示字段为空）。
    /// </summary>
    protected internal virtual Task<IReadOnlyDictionary<string, FriendDisplay>> ResolveFriendDisplayAsync(
        IReadOnlyCollection<string> userIds) =>
        Task.FromResult<IReadOnlyDictionary<string, FriendDisplay>>(
            new Dictionary<string, FriendDisplay>());

    /// <summary>
    /// 游戏提供：按 userId 批量解析聊天消息的展示信息（昵称 / 头像）。
    /// 默认返回空字典（消息里只有 userId）。
    /// </summary>
    protected internal virtual Task<IReadOnlyDictionary<string, ChatDisplay>> ResolveChatDisplayAsync(
        IReadOnlyCollection<string> userIds) =>
        Task.FromResult<IReadOnlyDictionary<string, ChatDisplay>>(new Dictionary<string, ChatDisplay>());

    /// <summary>
    /// 游戏提供：按 userId 批量解析帮会成员列表的展示信息（昵称 / 头像）。
    /// </summary>
    protected internal virtual Task<IReadOnlyDictionary<string, GuildDisplay>> ResolveGuildDisplayAsync(
        IReadOnlyCollection<string> userIds) =>
        Task.FromResult<IReadOnlyDictionary<string, GuildDisplay>>(new Dictionary<string, GuildDisplay>());

    /// <summary>
    /// 游戏提供：创建帮会的额外校验（等级 / 花费 / 冷却）。返回非空字符串 = 拒绝原因。
    /// </summary>
    protected internal virtual string? CheckGuildCreate(Player player, string name) => null;

    /// <summary>
    /// 游戏提供：使用背包物品。返回 Ok(提示文本) 表示使用成功；Err(原因) 表示拒绝。
    /// 物品消耗 / 效果由实现决定（通常调用 <c>player.Bag.Consume(...)</c>），
    /// 框架只负责校验格位、数量并把使用后的背包视图返回给客户端。
    /// 默认拒绝（未接入物品效果的框架不应让客户端"用掉"物品）。
    /// </summary>
    protected internal virtual Task<Result<string, string>> UseBagItemAsync(Player player, IItem item, long count) =>
        Task.FromResult(Result<string, string>.Err("该物品不能使用"));

    /// <summary>
    /// 游戏提供：丢弃前的额外校验（绑定 / 任务道具 / 不可丢弃等）。
    /// 返回非空字符串 = 拒绝原因；默认允许丢弃。
    /// </summary>
    protected internal virtual string? CheckDiscardBagItem(Player player, IItem item, long count) => null;

    /// <summary>
    /// 游戏提供：任务完成时发奖 / 记日志。奖励标识来自任务定义的 RewardKey，框架不解析。
    /// </summary>
    protected internal virtual Task OnQuestCompletedAsync(QuestCompletion completion) => Task.CompletedTask;

    /// <summary>
    /// 游戏提供：完成门槛的实时判定（事件状态不满足时才会问，例如「背包里要有 5 个药水」）。
    /// </summary>
    protected internal virtual bool CheckQuestCondition(Player player, QuestDefinition definition,
        QuestCondition condition) => false;

    /// <summary>
    /// 游戏提供：帮会任务完成时发奖 / 记日志。
    /// 可能在成员离线时触发，所以只给 id（走邮件或直接发奖都行）。
    /// </summary>
    protected internal virtual Task OnGuildTaskCompletedAsync(GuildTaskCompletion completion) =>
        Task.CompletedTask;

    /// <summary>
    /// 游戏提供：把好友关系写进玩家存档（镜像）。
    /// 关系本身以 Redis 为权威，这里只负责同步存档；读 <see cref="Player.Friends"/> /
    /// <see cref="Player.PendingFriendReserve"/> 后把它写进自己的存档字段并调用 Save()。
    /// 默认空实现（不存镜像也能正常工作）。
    /// </summary>
    protected internal virtual Task SyncFriendMirrorAsync(Player player) => Task.CompletedTask;

    /// <summary>
    /// 游戏提供：按 userId 从存档加载一个「无客户端连接」的玩家实例，用于给离线玩家的存档
    /// 同步好友镜像（加好友时对方不在线）。返回 null 表示不需要 / 无法加载。
    /// 实现必须与登录读档保持一致，调用方已持有该玩家的用户级 Redis 锁。
    /// </summary>
    protected internal virtual Task<Player?> LoadOfflinePlayerAsync(string userId) =>
        Task.FromResult<Player?>(null);

    public virtual async Task<Player> EchoQueryServiceEntity(string gateName, string cliConnId, byte[] info)
    {
        var player = CreateEntity(gateName, cliConnId, info);
        // 先注入物品配置 / 背包容量，再注册背包 / 邮件 / 商城协议（都要用到背包）
        ConfigurePlayer(player);
        // 背包：查询 / 移动 / 整理 / 丢弃 / 使用 + 变更推送
        player.RegisterBagRequests(this);
        // 给玩家实体挂上「客户端请求领取邮件」协议的处理
        player.RegisterMailRequest(this);
        // 商城：氪金商品购买、玩家寄拍、限时拍卖
        player.RegisterShopRequests(this);
        // 排行榜：打榜 / 拉榜区间 / 查名次 / 重置
        player.RegisterLeaderboardRequests(this);
        // 好友：申请 / 同意 / 拒绝 / 删除 / 列表
        player.RegisterFriendRequests(this);
        // 聊天：频道列表 / 发言 / 历史 / 私聊
        player.RegisterChatRequests(this);
        // 帮会：创建 / 成员 / 职位 / 任务
        player.RegisterGuildRequests(this);
        // 任务：列表 / 接取 / 提交完成
        player.RegisterQuestRequests(this);
        var cli = clients.AddOrUpdate(player.UserId, key => new Client(player.UserId, gateName, cliConnId), (key, old) => old);
        await player.CreateRemotePlayer(cli);
        await _group.CreateRemotePlayer(player);

        // 上线事件：刷新路由、推待处理好友申请、通知在线好友
        if (Friend != null)
        {
            try
            {
                await Friend.OnOnlineAsync(player);
            }
            catch (Exception ex)
            {
                Log.Error($"friend online user:{player.UserId} err:{ex}");
            }
        }

        // 帮会：必须在聊天之前——聊天的「帮会频道」作用域由帮会系统回答
        if (Guild != null)
        {
            try
            {
                await Guild.OnOnlineAsync(player);
            }
            catch (Exception ex)
            {
                Log.Error($"guild online user:{player.UserId} err:{ex}");
            }
        }

        // 任务：上线时跑一次触发判定（等级 / 场景这类事件可能离线期间变了）
        if (Quest != null)
        {
            try
            {
                await Quest.OnOnlineAsync(player);
            }
            catch (Exception ex)
            {
                Log.Error($"quest online user:{player.UserId} err:{ex}");
            }
        }

        // 聊天：解析玩家的频道作用域（场景 / 帮会 / 门派），建本地成员索引
        if (Chat != null)
        {
            try
            {
                await Chat.OnOnlineAsync(player);
            }
            catch (Exception ex)
            {
                Log.Error($"chat online user:{player.UserId} err:{ex}");
            }
        }

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