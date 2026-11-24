using System.Collections.Concurrent;
using chat;
using client;
using consts;
using core;
using engine;
using friend;
using Google.Protobuf;
using guild;
using hub;
using hub.inventory;
using leaderboard;
using MongoDB.Bson;
using quest;
using shop;
using ClientPlayer = client.Player;
using ClientServiceBase = client.Service;
using ClientSession = client.Client;
using HubClient = hub.Client;
using HubPlayerBase = hub.Player;
using HubServiceBase = hub.Service;

namespace ProtocolSmoke;

/// <summary>最小可用的 hub 侧 Player。Save 走内存快照（测试没有 Mongo）。</summary>
internal sealed class HubTestPlayer(string entityId, string entityType, string userId, RedisHandle redis,
    ConcurrentDictionary<string, HubClient> clients, ConcurrentDictionary<string, GateNetwork> gates)
    : HubPlayerBase(entityId, entityType, userId, redis, null!, clients, gates)
{
    /// <summary>用于验证迁移时「存档随实体走」的字段。</summary>
    public int Score;

    protected override string MongoDbName() => "protocol_smoke";

    protected override string MongoCollectionName() => "player";

    public override BsonDocument FullInfo() => new()
    {
        { "EntityId", EntityId },
        { "UserId", UserId },
        { "Score", Score },
    };

    public override BsonDocument ClientInfo() => FullInfo();

    /// <summary>测试没有 Mongo：Save 写成内存快照，LoadOfflinePlayerAsync 从它恢复。</summary>
    public override async void Save()
    {
        HarnessSnapshots.Snapshot(UserId, EntityId, Score);
        await Task.CompletedTask;
    }
}

/// <summary>跨节点的「存档」：模拟 Mongo 里玩家文档的持久化。</summary>
internal static class HarnessSnapshots
{
    private static readonly ConcurrentDictionary<string, (string EntityId, int Score)> Data = new();

    public static void Snapshot(string userId, string entityId, int score) => Data[userId] = (entityId, score);

    public static (string EntityId, int Score)? Load(string userId) =>
        Data.TryGetValue(userId, out var v) ? v : null;
}

/// <summary>最小可用的 hub 侧 Service。</summary>
internal sealed class HubTestService(ConcurrentDictionary<string, HubClient> clients, RedisHandle redis,
    ConcurrentDictionary<string, GateNetwork> gates) : HubServiceBase(clients)
{
    public string NextUserId = "u0";

    /// <summary>邮件领取条件；null = 不提供（协议会直接返回空列表）。</summary>
    public (int Level, int CreateDays)? MailCondition;

    // 跨程序集重写 protected internal 必须降级为 protected（C# 规则）
    protected override (int Level, int CreateDays)? GetMailCondition(string userId) => MailCondition;

    protected override HubPlayerBase CreateEntity(string gateName, string cliConnId, byte[] info)
        => new HubTestPlayer("e-" + NextUserId, "player", NextUserId, redis, clients, gates);

    /// <summary>测试用「使用物品」：直接消费，返回提示文本。</summary>
    protected override Task<Result<string, string>> UseBagItemAsync(HubPlayerBase player, IItem item, long count)
    {
        var used = player.Bag.Consume(item.ItemId, count);
        return Task.FromResult(used.Failed
            ? Result<string, string>.Err(used.Message)
            : Result<string, string>.Ok($"{item.ItemId} x{count}"));
    }

    /// <summary>登录参数就是 userId 的 UTF8 字节（测试约定），供顶号在创建前复用旧实体。</summary>
    protected override string? PeekUserId(byte[] info) =>
        info.Length == 0 ? null : System.Text.Encoding.UTF8.GetString(info);

    /// <summary>迁移重建：从内存快照恢复（等价于游戏从 Mongo 读档）。</summary>
    protected override Task<HubPlayerBase?> LoadOfflinePlayerAsync(string userId)
    {
        var snap = HarnessSnapshots.Load(userId);
        if (snap == null)
        {
            return Task.FromResult<HubPlayerBase?>(null);
        }

        var player = new HubTestPlayer(snap.Value.EntityId, "player", userId,
            GetRedis(), clients, gates)
        {
            Score = snap.Value.Score,
        };
        return Task.FromResult<HubPlayerBase?>(player);
    }

    private RedisHandle GetRedis() => redis;
}

/// <summary>最小可用的客户端 Player / Service。</summary>
internal sealed class TestClientPlayer(string entityId, string entityType) : ClientPlayer(entityId, entityType);

internal sealed class TestClientService : ClientServiceBase
{
    public override ClientPlayer CreatePlayerEntity(string entityId, string entityType, byte[] argv)
        => new TestClientPlayer(entityId, entityType);
}

/// <summary>
/// 进程内的「gate」：一边是客户端 SDK 的 IClientNetwork，另一边按 entityId 的归属节点
/// 把帧转给对应 hub 的实体（等价于 gate 的 cli.RegisterNetwork 切换）。
/// </summary>
internal sealed class MiniGate(ProtocolHarness h, string gateId, string userId) : IClientNetwork
{
    private Action<byte[]>? _toClient;
    private readonly Dictionary<string, int> _owner = new();
    private readonly object _lock = new();

    public string UserId { get; } = userId;

    public string ConnId { get; } = "conn-" + Guid.NewGuid().ToString("N")[..8];

    /// <summary>本次登录走哪个 hub 节点（模拟客户端连到不同 gate/hub）。</summary>
    public int LoginNode { get; set; }

    public event Action? OnClose;

    public bool IsOpen { get; private set; }

    public void Start(Action<byte[]> onPacket)
    {
        _toClient = onPacket;
        IsOpen = true;
        h.RegisterGate(this);
        SendToClient(new WRpc().Notify(Consts.NotifyConnId, new NotifyConnID { ConnId = ConnId }));
    }

    public void SetOwner(string entityId, int nodeIndex)
    {
        lock (_lock)
        {
            _owner[entityId] = nodeIndex;
        }
    }

    public int OwnerOf(string entityId)
    {
        lock (_lock)
        {
            return _owner.TryGetValue(entityId, out var n) ? n : 0;
        }
    }

    public async Task Send(byte[] data)
    {
        var msg = Parse(data);
        switch (msg.PayloadCase)
        {
            case Msg.PayloadOneofCase.Notify:
            {
                var ntf = msg.Notify;
                switch (ntf.Event.ProtoName)
                {
                    case Consts.ClientRequestService:
                    case Consts.ClientRequestReconnect:
                    {
                        var argv = ntf.Event.ProtoName == Consts.ClientRequestReconnect
                            ? new MessageParser<ClientRequestReconnect>(() => new ClientRequestReconnect())
                                .ParseFrom(ntf.Event.Content.ToByteArray()).Argv.ToByteArray()
                            : new MessageParser<ClientRequestService>(() => new ClientRequestService())
                                .ParseFrom(ntf.Event.Content.ToByteArray()).Argv.ToByteArray();

                        var player = await h.LoginViaNodeAsync(LoginNode, UserId, argv, this);
                        SetOwner(player.EntityId, LoginNode);
                        break;
                    }
                    case Consts.GateForwardClientNotifyHub:
                    {
                        var fwd = new MessageParser<GateForwardClientNotifyHub>(() => new GateForwardClientNotifyHub())
                            .ParseFrom(ntf.Event.Content.ToByteArray());
                        await DispatchToHub(fwd.EntityId, string.Empty, fwd.Event.ProtoName, fwd.Event.Content);
                        break;
                    }
                    case Consts.VersionHandshake:
                    case Consts.AckReliabilityMsg:
                        break;
                    default:
                        Console.WriteLine($"  (mini-gate) 未处理的 notify:{ntf.Event.ProtoName}");
                        break;
                }

                break;
            }
            case Msg.PayloadOneofCase.Req:
            {
                var req = msg.Req;
                if (req.Event.ProtoName == Consts.GateForwardClientRequestHub)
                {
                    var fwd = new MessageParser<GateForwardClientRequestHub>(() => new GateForwardClientRequestHub())
                        .ParseFrom(req.Event.Content.ToByteArray());
                    await DispatchToHub(fwd.EntityId, req.MsgId, fwd.Event.ProtoName, fwd.Event.Content);
                }

                break;
            }
            case Msg.PayloadOneofCase.HeartBeats:
                break;
            default:
                Console.WriteLine($"  (mini-gate) 未处理的报文:{msg.PayloadCase}");
                break;
        }
    }

    public Task Close()
    {
        if (!IsOpen)
        {
            return Task.CompletedTask;
        }

        IsOpen = false;
        OnClose?.Invoke();
        return Task.CompletedTask;
    }

    internal void SendToClient(byte[] frame) => _toClient?.Invoke(frame);

    private async Task DispatchToHub(string entityId, string msgId, string method, ByteString content)
    {
        var nodeIndex = OwnerOf(entityId);
        var player = h.Node(nodeIndex).PlayerByEntity(entityId);
        if (player == null)
        {
            // 等价于 gate 直连找不到 → 走 hub 的墓碑转发回调
            await h.Node(nodeIndex).Coordinator.ForwardMissingEntityAsync(entityId, ConnId, msgId, method,
                content.ToByteArray());
            return;
        }

        await player.OnDoMsg(ConnId, msgId, null, method, content);
    }

    private static Msg Parse(byte[] data) => new MessageParser<Msg>(() => new Msg()).ParseFrom(data);
}

/// <summary>hub → gate 方向的帧转换（等价于 gate 的 HubGeneralMsgHandle），带节点标记。</summary>
internal sealed class HubToGateNetwork(ProtocolHarness h, int nodeIndex) : INetwork
{
    public Task Send(byte[] data)
    {
        var msg = new MessageParser<Msg>(() => new Msg()).ParseFrom(data);
        MiniGate? gate;
        switch (msg.PayloadCase)
        {
            case Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.HubCreatePlayerEntity:
            {
                var m = new MessageParser<HubCreatePlayerEntity>(() => new HubCreatePlayerEntity())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray());
                gate = h.FindGateByConnId(m.ConnId);
                if (gate != null)
                {
                    // gate 在这里做 entity → hub 连接的重注册（迁移接管就是靠它）
                    gate.SetOwner(m.EntityId, nodeIndex);
                    gate.SendToClient(new WRpc().Notify(Consts.CreatePlayerEntity, new CreatePlayerEntity
                    {
                        EntityId = m.EntityId,
                        EntityType = m.EntityType,
                        Argv = m.Argv,
                    }));
                }

                return Task.CompletedTask;
            }
            case Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.GateForwardHubNotifyClient:
            {
                var m = new MessageParser<GateForwardHubNotifyClient>(() => new GateForwardHubNotifyClient())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray());
                gate = h.FindGateByConnId(m.ConnId);
                gate?.SendToClient(new WRpc().Notify(Consts.HubNotifyClient, new HubNotifyClient
                {
                    EntityId = m.EntityId,
                    Event = m.Event,
                }));
                return Task.CompletedTask;
            }
            case Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.GateForwardHubNotifyClientMq:
            {
                var m = new MessageParser<GateForwardHubNotifyClientMq>(() => new GateForwardHubNotifyClientMq())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray());
                gate = h.FindGateByUserId(m.UserId);
                gate?.SendToClient(new WRpc().Notify(Consts.HubNotifyClientMq, new HubNotifyClientMq
                {
                    EntityId = m.EntityId,
                    Event = m.Event,
                    NeedAck = false,
                }));
                return Task.CompletedTask;
            }
            case Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.HubKickOffClient:
            {
                var m = new MessageParser<HubKickOffClient>(() => new HubKickOffClient())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray());
                gate = h.FindGateByConnId(m.ConnId);
                gate?.SendToClient(new WRpc().Notify(Consts.KickOff, new KickOff { PromptInfo = m.PromptInfo }));
                return Task.CompletedTask;
            }
            case Msg.PayloadOneofCase.Rsp when msg.Rsp.Event.ProtoName == Consts.GateForwardHubResponseClient:
            {
                var m = new MessageParser<GateForwardHubResponseClient>(() => new GateForwardHubResponseClient())
                    .ParseFrom(msg.Rsp.Event.Content.ToByteArray());
                gate = h.FindGateByConnId(m.ConnId);
                gate?.SendToClient(new WRpc().Response(Consts.HubResponseClient, msg.Rsp.MsgId,
                    new HubResponseClient { ErrMsg = m.ErrMsg, Content = m.Content }));
                return Task.CompletedTask;
            }
            default:
                return Task.CompletedTask;
        }
    }

    public void OnReceive(Action<byte[]> onReceive)
    {
    }

    public Task Close() => Task.CompletedTask;
}

/// <summary>一个 hub 节点：自己的 clients / entities / service / coordinator。</summary>
internal sealed class HubNode
{
    public required string HubId { get; init; }
    public required HubTestService Service { get; init; }
    public required SessionCoordinator Coordinator { get; init; }
    public required HubGlobalEvent GlobalEvent { get; init; }
    public required ConcurrentDictionary<string, HubClient> Clients { get; init; }
    public required ConcurrentDictionary<string, GateNetwork> Gates { get; init; }
    public required ConcurrentDictionary<string, BaseEntity> Entities { get; init; }
    public required FriendService FriendService { get; init; }
    public required ChatService ChatService { get; init; }
    public required GuildService GuildService { get; init; }

    public HubPlayerBase? PlayerByEntity(string entityId) =>
        Entities.TryGetValue(entityId, out var e) ? e as HubPlayerBase : null;

    public HubPlayerBase? PlayerByUser(string userId) =>
        Coordinator.TryGetLivePlayer(userId, out var p) ? p : null;
}

/// <summary>把 hub 侧的一切准备好（Redis + 各子系统 + N 个 hub 节点），并负责客户端登录与推送泵。</summary>
internal sealed class ProtocolHarness : IAsyncDisposable
{
    public const string GateId = "gate-smoke";

    private readonly string _prefix;
    private readonly List<MiniGate> _gates = new();
    private readonly ConcurrentDictionary<string, MiniGate> _currentGateByUser = new();
    private readonly List<HubNode> _nodes = new();

    public RedisHandle Redis { get; }

    public QuestService QuestService { get; private set; } = null!;

    public FriendStore FriendStore { get; private set; } = null!;

    /// <summary>节点 0 的好友服务（兼容旧测试写法）。</summary>
    public FriendService FriendService => Node(0).FriendService;

    public ItemCatalog ItemCatalog { get; } = new();

    public QuestCatalog QuestCatalog { get; } = new();

    public GuildTaskCatalog GuildTaskCatalog { get; } = new();

    public string MailListKey { get; private set; } = "";

    /// <summary>节点 0 的 service（兼容旧测试写法）。</summary>
    public HubTestService Service => Node(0).Service;

    private ProtocolHarness(string redisUrl)
    {
        _prefix = "proto-smoke-" + Guid.NewGuid().ToString("N") + ":";
        core.Metrics.Init(new core.MonitoringOptions
        {
            ServiceName = "protocol-smoke",
            NodeId = "smoke-node",
            SlowLogThresholdMs = 60_000,
        });
        Redis = new RedisHandle(redisUrl, "");
    }

    public static ProtocolHarness Create(string redisUrl, int nodeCount = 2)
    {
        var h = new ProtocolHarness(redisUrl);
        h.InitSubsystems();
        for (var i = 0; i < nodeCount; i++)
        {
            h.AddNode("hub-" + i);
        }

        return h;
    }

    public HubNode Node(int index) => _nodes[index];

    public int NodeCount => _nodes.Count;

    private void InitSubsystems()
    {
        MailListKey = _prefix + "mail_global";

        FriendStore = new FriendStore(Redis, new FriendSettings
        {
            KeyPrefix = _prefix,
            MaxFriends = 5,
            MaxPendingRequests = 10,
            ReserveTimeoutMs = 3000,
        });
    }

    private HubNode AddNode(string hubId)
    {
        var clients = new ConcurrentDictionary<string, HubClient>();
        var gates = new ConcurrentDictionary<string, GateNetwork>();
        var entities = new ConcurrentDictionary<string, BaseEntity>();

        gates[GateId] = new GateNetwork(new HubToGateNetwork(this, _nodes.Count));

        var service = new HubTestService(clients, Redis, gates);
        var globalEvent = new HubGlobalEvent(new HubConfig { HubId = hubId }, Redis);
        var coordinator = new SessionCoordinator(Redis, hubId, clients, gates, entities,
            () => new[] { service }, globalEvent, _ => { });

        service.GlobalMailBox = new GlobalMailBox(Redis, MailListKey, _prefix + "mail_claimed", 0);
        service.ItemCatalog = ItemCatalog;
        ItemCatalog.Register(new ItemDefinition("diamond", 0));
        ItemCatalog.Register(new ItemDefinition("gold", 0));
        ItemCatalog.Register(new ItemDefinition("potion", 99));

        service.Shop = new ShopStore(Redis, new ShopSettings
        {
            KeyPrefix = _prefix,
            CurrencyItemId = "diamond",
            MarketCurrencyItemId = "gold",
            MinListingDurationSeconds = 1,
            MaxListingDurationSeconds = 3600,
            DefaultListingDurationSeconds = 60,
            DefaultAuctionDurationSeconds = 60,
            MaxListingsPerPlayer = 4,
        });

        service.Leaderboard = new LeaderboardStore(Redis, new LeaderboardSettings
        {
            KeyPrefix = _prefix,
            MaxEntries = 0,
        });

        var friendService = new FriendService(FriendStore, Redis, hubId);
        service.Friend = friendService;

        var chatStore = new ChatStore(Redis, new ChatSettings { KeyPrefix = _prefix });
        var chatService = new ChatService(chatStore, Redis)
        {
            ResolveOnlineEntityId = async userId =>
            {
                var presence = await FriendStore.GetPresenceAsync(new[] { userId });
                return presence.TryGetValue(userId, out var where) ? where.EntityId : null;
            },
        };
        service.Chat = chatService;
        chatStore.Subscribe(payload => _ = Task.Run(async () =>
        {
            try
            {
                await chatService.OnBroadcastAsync(payload);
            }
            catch (Exception e)
            {
                Console.WriteLine("  (harness) chat broadcast err:" + e.Message);
            }
        }));

        var guildService = new GuildService(new GuildStore(Redis, new GuildSettings
        {
            KeyPrefix = _prefix,
            MaxMembers = 5,
            MaxOfficers = 2,
            MaxApplications = 5,
            MaxInvites = 5,
        }), Redis)
        {
            ResolveOnlineEntityIds = async userIds =>
            {
                var result = new Dictionary<string, string>();
                var presence = await FriendStore.GetPresenceAsync(userIds);
                foreach (var (userId, where) in presence)
                {
                    result[userId] = where.EntityId;
                }

                return result;
            },
        };
        service.Guild = guildService;

        QuestService = new QuestService(
            new QuestStore(Redis, new QuestSettings { KeyPrefix = _prefix, MaxActiveQuests = 10 }), Redis);
        service.Quest = QuestService;

        QuestCatalog.RegisterRange(QuestFixtures.Quests);
        GuildTaskCatalog.RegisterRange(GuildFixtures.Tasks);
        service.QuestCatalog = QuestCatalog;
        service.GuildTaskCatalog = GuildTaskCatalog;
        QuestService.Catalog = QuestCatalog;
        guildService.Catalog = GuildTaskCatalog;
        chatService.GuildScopeResolver ??= player => guildService.ScopeOf(player.UserId);

        var node = new HubNode
        {
            HubId = hubId,
            Service = service,
            Coordinator = coordinator,
            GlobalEvent = globalEvent,
            Clients = clients,
            Gates = gates,
            Entities = entities,
            FriendService = friendService,
            ChatService = chatService,
            GuildService = guildService,
        };
        _nodes.Add(node);

        // hub_hub_{hubId}_mq 消费循环（等价于 MainClass.StartRedisMsg 的那一段）
        _ = Task.Run(async () =>
        {
            var parser = new MessageParser<HubHubNotifyEntity>(() => new HubHubNotifyEntity());
            while (true)
            {
                try
                {
                    var task = Redis.PopList(string.Format(Consts.HubHubMq, hubId), 8);
                    var frames = task == null ? null : await task;
                    if (frames == null || frames.Count == 0)
                    {
                        await Task.Delay(5);
                        continue;
                    }

                    foreach (var frame in frames)
                    {
                        await coordinator.DispatchHubFrameAsync(parser.ParseFrom(frame));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("  (harness) hub mq err:" + e.Message);
                    await Task.Delay(20);
                }
            }
        });

        return node;
    }

    internal void RegisterGate(MiniGate gate)
    {
        lock (_gates)
        {
            _gates.Add(gate);
        }

        _currentGateByUser[gate.UserId] = gate;
    }

    internal MiniGate? FindGateByConnId(string connId)
    {
        lock (_gates)
        {
            return _gates.FirstOrDefault(g => g.ConnId == connId);
        }
    }

    internal MiniGate? FindGateByUserId(string userId) =>
        _currentGateByUser.TryGetValue(userId, out var g) ? g : null;

    /// <summary>登录走指定 hub 节点（真实路径：coordinator.OnLoginAsync）。</summary>
    internal async Task<HubPlayerBase> LoginViaNodeAsync(int nodeIndex, string userId, byte[] argv, MiniGate gate)
    {
        var node = _nodes[nodeIndex];
        node.Service.NextUserId = userId;
        var player = await node.Coordinator.OnLoginAsync(node.Service, GateId, gate.ConnId, argv);
        return player!;
    }

    /// <summary>客户端登录：建 MiniGate + client SDK，等玩家实体就绪。</summary>
    public async Task<TestClient> LoginAsync(string userId, int nodeIndex = 0,
        Action<Action>? postToMainThread = null)
    {
        var gate = new MiniGate(this, GateId, userId) { LoginNode = nodeIndex };

        var clientService = new TestClientService();
        var session = new ClientSession(new ClientConfig
        {
            PostToMainThread = postToMainThread,
            ServiceName = "test_service",
            RequestTimeoutMs = 5000,
            AutoReconnect = false,
            ResyncTimeoutMs = 0,
            MinVersion = 0,
            MaxVersion = 0,
            Argv = System.Text.Encoding.UTF8.GetBytes(userId),
        }, clientService, _ => gate);

        var connected = await session.Connect();
        if (!connected)
        {
            throw new InvalidOperationException("client connect failed");
        }

        var deadline = Environment.TickCount64 + 5000;
        while (session.Player == null && Environment.TickCount64 < deadline)
        {
            await Task.Delay(20);
        }

        if (session.Player == null)
        {
            throw new InvalidOperationException($"client player not created for {userId}");
        }

        await PumpAsync();
        return new TestClient(this, session, gate, nodeIndex);
    }

    /// <summary>模拟真实 gate 的 presence 维护 + 客户端队列投递泵。</summary>
    public async Task PumpAsync(int rounds = 3, int delayMs = 30)
    {
        for (var i = 0; i < rounds; i++)
        {
            foreach (var (userId, gate) in _currentGateByUser.ToList())
            {
                foreach (var node in _nodes)
                {
                    var player = node.PlayerByUser(userId);
                    if (player != null)
                    {
                        await FriendStore.SetPresenceAsync(userId, GateId, player.EntityId);
                    }
                }

                var popTask = Redis.PopList(string.Format(Consts.EntityClientMq, userId), 16);
                var list = popTask == null ? null : await popTask;
                if (list == null)
                {
                    continue;
                }

                foreach (var frame in list)
                {
                    await Gate(GateId).Send(frame);
                }
            }

            await Task.Delay(delayMs);
        }
    }

    internal GateNetwork Gate(string gateId) => _nodes[0].Gates[gateId];

    /// <summary>按 Prometheus 文本格式导出一份指标（等价于抓 /metrics）。</summary>
    public static async Task<string> ScrapeMetricsAsync()
    {
        using var buffer = new MemoryStream();
        await core.Metrics.ExportAsync(buffer);
        return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>往全局邮件 list 里投一封邮件（字段名与 admin 写入的一致）。</summary>
    public Task PushGlobalMailAsync(string mailId, string title,
        (string ItemId, int Count)[] attachments, int minLevel = 0)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            mailId,
            title,
            content = "内容 " + title,
            sender = "GM",
            attachments = attachments.Select(a => new { itemId = a.ItemId, count = a.Count }).ToArray(),
            createTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            expireSeconds = 0,
            popUp = false,
            validDays = 0,
            minLevel,
            maxLevel = 0,
            minCreateDays = 0,
            maxCreateDays = 0,
        });

        return Redis.PushList(MailListKey, System.Text.Encoding.UTF8.GetBytes(json));
    }

    public static void GiveItem(HubPlayerBase player, string itemId, long count)
    {
        var result = player.Bag.Add(itemId, count);
        if (result.Failed)
        {
            throw new InvalidOperationException("准备背包失败: " + result.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var key in await Redis.GetKeys(_prefix + "*"))
        {
            Redis.DelData(key);
        }
    }

    public static class QuestFixtures
    {
        public static readonly QuestDefinition[] Quests =
        [
            new("q_auto", "自动任务",
                AutoAccept: true,
                TriggerConditions: [new QuestCondition("level", "", QuestConditionMode.Max, 1)],
                ProgressConditions: [new QuestCondition("kill", "goblin", QuestConditionMode.Count, 2)],
                CompletionConditions: [new QuestCondition("talk", "npc", QuestConditionMode.Flag)],
                NextQuestIds: ["q_next"],
                RewardKey: "rw_auto"),
            new("q_next", "后继任务", RequireQuests: ["q_auto"]),
            new("q_manual", "手动任务",
                AutoAccept: false,
                TriggerConditions: [new QuestCondition("level", "", QuestConditionMode.Max, 1)],
                ProgressConditions: [new QuestCondition("collect", "potion", QuestConditionMode.Count, 1)]),
        ];
    }

    public static class GuildFixtures
    {
        public static readonly GuildTaskDefinition[] Tasks =
        [
            new("g_task_daily", "帮会日常", Target: 3),
            new("g_task_boss", "帮会首领", Target: 1, PerMember: false, OfficerCanPublish: true),
        ];
    }
}

/// <summary>
/// 模拟引擎主线程：所有回调排进队列，由同一个线程 drain。
/// 用来验证 ClientConfig.PostToMainThread 的语义（线程归属 + 保序）。
/// </summary>
internal sealed class FakeMainThread : IDisposable
{
    private readonly BlockingCollection<Action> _queue = new();
    private readonly Thread _thread;
    private volatile bool _running = true;

    public FakeMainThread()
    {
        _thread = new Thread(() =>
        {
            MainThreadId = Environment.CurrentManagedThreadId;
            foreach (var action in _queue.GetConsumingEnumerable())
            {
                if (!_running)
                {
                    break;
                }

                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Console.WriteLine("  (fake-main) callback err:" + e.Message);
                }
            }
        })
        { IsBackground = true, Name = "fake-main-thread" };
        _thread.Start();

        var deadline = Environment.TickCount64 + 3000;
        while (MainThreadId == 0 && Environment.TickCount64 < deadline)
        {
            Thread.Sleep(5);
        }
    }

    public int MainThreadId { get; private set; }

    public void Post(Action action) => _queue.Add(action);

    public async Task PumpAsync(int delayMs = 60)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Post(() => done.TrySetResult());
        await Task.WhenAny(done.Task, Task.Delay(3000));
        await Task.Delay(delayMs);
    }

    public void Dispose()
    {
        _running = false;
        _queue.CompleteAdding();
        _thread.Join(1000);
        _queue.Dispose();
    }
}

/// <summary>一个已登录的测试客户端。</summary>
internal sealed class TestClient(ProtocolHarness harness, ClientSession session, MiniGate gate, int nodeIndex)
{
    public ProtocolHarness Harness { get; } = harness;

    public ClientSession Session { get; } = session;

    public ClientPlayer Player => Session.Player!;

    public MiniGate Gate { get; } = gate;

    public int NodeIndex { get; } = nodeIndex;

    public string UserId => Gate.UserId;

    /// <summary>本客户端当前归属 hub 节点上的服务端 Player（测试子类，带 Score 等字段）。</summary>
    public HubTestPlayer HubPlayer =>
        (HubTestPlayer)Harness.Node(Gate.OwnerOf(Player.EntityId)).PlayerByUser(UserId)!;

    public static async Task<bool> WaitForAsync(Func<bool> condition, int timeoutMs = 3000)
    {
        var deadline = Environment.TickCount64 + timeoutMs;
        while (!condition())
        {
            if (Environment.TickCount64 > deadline)
            {
                return false;
            }

            await Task.Delay(20);
        }

        return true;
    }
}
