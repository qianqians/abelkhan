using System.Collections.Concurrent;
using chat;
using client;
using ClientPlayer = client.Player;
using ClientServiceBase = client.Service;
using ClientSession = client.Client;
using HubClient = hub.Client;
using HubPlayerBase = hub.Player;
using HubServiceBase = hub.Service;
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

namespace ProtocolSmoke;

/// <summary>最小可用的 hub 侧 Player（协议测试只走协议层，存档相关方法不会被调用）。</summary>
internal sealed class HubTestPlayer(string entityId, string entityType, string userId, RedisHandle redis,
    ConcurrentDictionary<string, HubClient> clients, ConcurrentDictionary<string, GateNetwork> gates)
    : HubPlayerBase(entityId, entityType, userId, redis, null!, clients, gates)
{
    protected override string MongoDbName() => "protocol_smoke";

    protected override string MongoCollectionName() => "player";

    public override BsonDocument FullInfo() => new() { { "EntityId", EntityId }, { "UserId", UserId } };

    public override BsonDocument ClientInfo() => FullInfo();
}

/// <summary>最小可用的 hub 侧 Service：所有子系统在 <see cref="HubHarness"/> 里注入。</summary>
internal sealed class HubTestService(ConcurrentDictionary<string, HubClient> clients, RedisHandle redis,
    ConcurrentDictionary<string, GateNetwork> gates) : HubServiceBase(clients)
{
    public string NextUserId = "u0";

    protected override HubPlayerBase CreateEntity(string gateName, string cliConnId, byte[] info)
        => new HubTestPlayer("e-" + NextUserId, "player", NextUserId, redis, clients, gates);
}

/// <summary>最小可用的客户端 Player（游戏侧会派生它）。</summary>
internal sealed class TestClientPlayer(string entityId, string entityType) : ClientPlayer(entityId, entityType);

/// <summary>最小可用的客户端 Service：把服务器的 entity_type 映射成 <see cref="TestClientPlayer"/>。</summary>
internal sealed class TestClientService : ClientServiceBase
{
    public override ClientPlayer CreatePlayerEntity(string entityId, string entityType, byte[] argv)
        => new TestClientPlayer(entityId, entityType);
}

/// <summary>
/// 进程内的「gate」：一边是客户端 SDK 的 <see cref="IClientNetwork"/>，另一边是 hub 的
/// <see cref="GateNetwork"/>，中间做与真实 gate 相同的报文转换。
///
/// 这样测试走的是**真实链路**：client.Player 的封装 → 客户端报文 → gate 转换 → hub 实体 handler
/// → Redis / 子系统 → 回包 → gate 转换 → 客户端分发 → 封装方法返回 Result。
/// </summary>
internal sealed class MiniGate(ProtocolHarness h, string gateId, string userId) : IClientNetwork
{
    private Action<byte[]>? _toClient;

    public string UserId { get; } = userId;

    public string ConnId { get; } = "conn-" + Guid.NewGuid().ToString("N")[..8];

    public string EntityId { get; private set; } = string.Empty;

    public HubPlayerBase? HubPlayer { get; private set; }

    public event Action? OnClose;

    public bool IsOpen { get; private set; }

    public void Start(Action<byte[]> onPacket)
    {
        _toClient = onPacket;
        IsOpen = true;
        h.RegisterGate(this);

        // 真实 gate 在 accept 之后下发 conn_id
        SendToClient(new WRpc().Notify(Consts.NotifyConnId, new NotifyConnID { ConnId = ConnId }));
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
                        await OnRequestService(ntf);
                        break;
                    case Consts.ClientRequestReconnect:
                        await OnRequestService(ntf); // 测试里重连等价于一次登录
                        break;
                    case Consts.GateForwardClientNotifyHub:
                    {
                        var fwd = new MessageParser<GateForwardClientNotifyHub>(() => new GateForwardClientNotifyHub())
                            .ParseFrom(ntf.Event.Content.ToByteArray());
                        await DispatchToHub(fwd.EntityId, string.Empty, fwd.Event.ProtoName, fwd.Event.Content);
                        break;
                    }
                    case Consts.VersionHandshake:
                    case Consts.AckReliabilityMsg:
                        break; // 版本 / 可靠消息 ack 在进程内没有意义
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
                else
                {
                    Console.WriteLine($"  (mini-gate) 未处理的 req:{req.Event.ProtoName}");
                }

                break;
            }
            case Msg.PayloadOneofCase.HeartBeats:
                break; // 心跳在进程内没有意义

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

    /// <summary>hub → gate 的帧，转换成客户端报文。</summary>
    internal void SendToClient(byte[] frame)
    {
        _toClient?.Invoke(frame);
    }

    internal void NotifyConnId()
    {
        SendToClient(new WRpc().Notify(Consts.NotifyConnId, new NotifyConnID { ConnId = ConnId }));
    }

    private async Task OnRequestService(Notify ntf)
    {
        var argv = ntf.Event.ProtoName == Consts.ClientRequestReconnect
            ? new MessageParser<ClientRequestReconnect>(() => new ClientRequestReconnect())
                .ParseFrom(ntf.Event.Content.ToByteArray()).Argv.ToByteArray()
            : new MessageParser<ClientRequestService>(() => new ClientRequestService())
                .ParseFrom(ntf.Event.Content.ToByteArray()).Argv.ToByteArray();

        HubPlayer = await h.LoginHubEntityAsync(this, UserId, argv);
        EntityId = HubPlayer.EntityId;
    }

    private async Task DispatchToHub(string entityId, string msgId, string method, ByteString content)
    {
        if (HubPlayer == null)
        {
            Console.WriteLine($"  (mini-gate) 实体还没创建，丢弃 {method}");
            return;
        }

        if (!string.Equals(entityId, EntityId, StringComparison.Ordinal))
        {
            Console.WriteLine($"  (mini-gate) 实体 id 不匹配 {entityId} != {EntityId}");
            return;
        }

        await HubPlayer.OnDoMsg(ConnId, msgId, h.Gate(ProtocolHarness.GateId), method, content);
    }

    private static Msg Parse(byte[] data) =>
        new MessageParser<Msg>(() => new Msg()).ParseFrom(data);
}

/// <summary>hub 侧的出站网络：抓住 hub 发给 gate 的帧做转换（等价于 gate 的 HubGeneralMsgHandle）。</summary>
internal sealed class HubToGateNetwork(ProtocolHarness h, string gateId) : INetwork
{
    public Task Send(byte[] data)
    {
        var gate = h.FindGateByHubUserId(gateId, data, out var msg);
        if (gate == null)
        {
            return Task.CompletedTask;
        }

        switch (msg.PayloadCase)
        {
            case Msg.PayloadOneofCase.Notify:
            {
                var ntf = msg.Notify;
                switch (ntf.Event.ProtoName)
                {
                    case Consts.HubCreatePlayerEntity:
                    {
                        var m = new MessageParser<HubCreatePlayerEntity>(() => new HubCreatePlayerEntity())
                            .ParseFrom(ntf.Event.Content.ToByteArray());
                        gate.SendToClient(new WRpc().Notify(Consts.CreatePlayerEntity, new CreatePlayerEntity
                        {
                            EntityId = m.EntityId,
                            EntityType = m.EntityType,
                            Argv = m.Argv,
                        }));
                        break;
                    }
                    case Consts.GateForwardHubNotifyClient:
                    {
                        var m = new MessageParser<GateForwardHubNotifyClient>(() => new GateForwardHubNotifyClient())
                            .ParseFrom(ntf.Event.Content.ToByteArray());
                        gate.SendToClient(new WRpc().Notify(Consts.HubNotifyClient, new HubNotifyClient
                        {
                            EntityId = m.EntityId,
                            Event = m.Event,
                        }));
                        break;
                    }
                    case Consts.GateForwardHubNotifyClientMq:
                    {
                        var m = new MessageParser<GateForwardHubNotifyClientMq>(
                                () => new GateForwardHubNotifyClientMq())
                            .ParseFrom(ntf.Event.Content.ToByteArray());
                        gate.SendToClient(new WRpc().Notify(Consts.HubNotifyClientMq, new HubNotifyClientMq
                        {
                            EntityId = m.EntityId,
                            Event = m.Event,
                            NeedAck = false,
                        }));
                        break;
                    }
                    case Consts.GateForwardHubCallGlobal:
                    case Consts.HubKickOffClient:
                        break;
                    default:
                        Console.WriteLine($"  (hub→gate) 未处理的 notify:{ntf.Event.ProtoName}");
                        break;
                }

                break;
            }
            case Msg.PayloadOneofCase.Rsp:
            {
                var rsp = msg.Rsp;
                if (rsp.Event.ProtoName == Consts.GateForwardHubResponseClient)
                {
                    var m = new MessageParser<GateForwardHubResponseClient>(() => new GateForwardHubResponseClient())
                        .ParseFrom(rsp.Event.Content.ToByteArray());
                    gate.SendToClient(new WRpc().Response(Consts.HubResponseClient, rsp.MsgId,
                        new HubResponseClient
                        {
                            ErrMsg = m.ErrMsg,
                            Content = m.Content,
                        }));
                }
                else
                {
                    Console.WriteLine($"  (hub→gate) 未处理的 rsp:{rsp.Event.ProtoName}");
                }

                break;
            }
            default:
                Console.WriteLine($"  (hub→gate) 未处理的报文:{msg.PayloadCase}");
                break;
        }

        return Task.CompletedTask;
    }

    public void OnReceive(Action<byte[]> onReceive)
    {
    }

    public Task Close() => Task.CompletedTask;
}

/// <summary>把 hub 侧的一切准备好（Redis + 各子系统），并负责客户端登录与推送泵。</summary>
internal sealed class ProtocolHarness : IAsyncDisposable
{
    public const string GateId = "gate-smoke";

    private readonly string _prefix;
    private readonly List<MiniGate> _gates = new();
    private readonly ConcurrentDictionary<string, MiniGate> _gateByUser = new();

    public RedisHandle Redis { get; }

    public ConcurrentDictionary<string, HubClient> Clients { get; } = new();

    public ConcurrentDictionary<string, GateNetwork> Gates { get; } = new();

    public HubTestService Service { get; }

    public FriendStore FriendStore { get; }

    public ItemCatalog ItemCatalog { get; } = new();

    public QuestCatalog QuestCatalog { get; } = new();

    public GuildTaskCatalog GuildTaskCatalog { get; } = new();

    public QuestService QuestService { get; }

    private ProtocolHarness(string redisUrl)
    {
        _prefix = "proto-smoke-" + Guid.NewGuid().ToString("N") + ":";
        Redis = new RedisHandle(redisUrl, "");

        Gates[GateId] = new GateNetwork(new HubToGateNetwork(this, GateId));
        Service = new HubTestService(Clients, Redis, Gates);

        Service.GlobalMailBox = new GlobalMailBox(Redis, _prefix + "mail_global", _prefix + "mail_claimed");
        Service.ItemCatalog = ItemCatalog;
        ItemCatalog.Register(new ItemDefinition("diamond", 0));
        ItemCatalog.Register(new ItemDefinition("gold", 0));
        ItemCatalog.Register(new ItemDefinition("potion", 99));

        Service.Shop = new ShopStore(Redis, new ShopSettings
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

        Service.Leaderboard = new LeaderboardStore(Redis, new LeaderboardSettings
        {
            KeyPrefix = _prefix,
            MaxEntries = 0,
        });

        FriendStore = new FriendStore(Redis, new FriendSettings
        {
            KeyPrefix = _prefix,
            MaxFriends = 5,
            MaxPendingRequests = 10,
            ReserveTimeoutMs = 3000,
        });
        Service.Friend = new FriendService(FriendStore, Redis, "hub-smoke");

        var chatStore = new ChatStore(Redis, new ChatSettings { KeyPrefix = _prefix });
        var chatService = new ChatService(chatStore, Redis)
        {
            ResolveOnlineEntityId = async userId =>
            {
                var presence = await FriendStore.GetPresenceAsync(new[] { userId });
                return presence.TryGetValue(userId, out var where) ? where.EntityId : null;
            },
        };
        Service.Chat = chatService;

        // 等价于 MainClass：订阅跨 hub 频道广播（少了这一步，频道消息谁都收不到——
        // 发送方也走订阅，只有一条分发路径）
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
        Service.Guild = guildService;

        QuestService = new QuestService(
            new QuestStore(Redis, new QuestSettings { KeyPrefix = _prefix, MaxActiveQuests = 10 }), Redis);
        Service.Quest = QuestService;

        QuestCatalog.RegisterRange(QuestFixtures.Quests);
        GuildTaskCatalog.RegisterRange(GuildFixtures.Tasks);
        Service.QuestCatalog = QuestCatalog;
        Service.GuildTaskCatalog = GuildTaskCatalog;
        QuestService.Catalog = QuestCatalog;
        guildService.Catalog = GuildTaskCatalog;
        Service.Chat.GuildScopeResolver ??= player => Service.Guild.ScopeOf(player.UserId);
    }

    public static ProtocolHarness Create(string redisUrl) => new(redisUrl);

    internal GateNetwork Gate(string gateId) => Gates[gateId];

    internal void RegisterGate(MiniGate gate)
    {
        lock (_gates)
        {
            _gates.Add(gate);
        }
    }

    /// <summary>按帧里带的路由信息找回对应的 gate（等价于真实 gate 按 conn_id / user_id 找连接）。</summary>
    internal MiniGate? FindGateByHubUserId(string gateId, byte[] frame, out Msg msg)
    {
        msg = new MessageParser<Msg>(() => new Msg()).ParseFrom(frame);

        // 建实体 / 可靠队列：按 user_id 找
        string? userId = msg.PayloadCase switch
        {
            Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.HubCreatePlayerEntity =>
                new MessageParser<HubCreatePlayerEntity>(() => new HubCreatePlayerEntity())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray()).UserId,
            Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.GateForwardHubNotifyClientMq =>
                new MessageParser<GateForwardHubNotifyClientMq>(() => new GateForwardHubNotifyClientMq())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray()).UserId,
            _ => null,
        };

        if (!string.IsNullOrEmpty(userId))
        {
            return _gateByUser.TryGetValue(userId, out var byUser) ? byUser : null;
        }

        // 其余帧带 conn_id：按连接找
        string? connId = msg.PayloadCase switch
        {
            Msg.PayloadOneofCase.Notify when msg.Notify.Event.ProtoName == Consts.GateForwardHubNotifyClient =>
                new MessageParser<GateForwardHubNotifyClient>(() => new GateForwardHubNotifyClient())
                    .ParseFrom(msg.Notify.Event.Content.ToByteArray()).ConnId,
            Msg.PayloadOneofCase.Rsp when msg.Rsp.Event.ProtoName == Consts.GateForwardHubResponseClient =>
                new MessageParser<GateForwardHubResponseClient>(() => new GateForwardHubResponseClient())
                    .ParseFrom(msg.Rsp.Event.Content.ToByteArray()).ConnId,
            _ => null,
        };

        if (string.IsNullOrEmpty(connId))
        {
            return null;
        }

        lock (_gates)
        {
            return _gates.FirstOrDefault(g => g.ConnId == connId);
        }
    }

    /// <summary>hub 侧创建玩家实体（真实登录路径）。</summary>
    internal Task<HubPlayerBase> LoginHubEntityAsync(MiniGate gate, string userId, byte[] argv)
    {
        Service.NextUserId = userId;
        return Service.EchoQueryServiceEntity(GateId, gate.ConnId, argv);
    }

    /// <summary>
    /// 模拟真实 gate 的 presence 维护 + 推送泵：
    /// gate 会周期刷新 presence，并把 <c>entity_{userId}_client_mq</c> 里的消息投给客户端。
    /// </summary>
    public async Task PumpAsync(int rounds = 3, int delayMs = 30)
    {
        for (var i = 0; i < rounds; i++)
        {
            foreach (var (userId, gate) in _gateByUser)
            {
                if (gate.HubPlayer != null)
                {
                    await FriendStore.SetPresenceAsync(userId, GateId, gate.EntityId);
                }

                var popTask = Redis.PopList(string.Format(Consts.EntityClientMq, userId), 16);
                var list = popTask == null ? null : await popTask;
                if (list == null)
                {
                    continue;
                }


                foreach (var frame in list)
                {
                    // 和真实 gate 一样：先把 hub 的帧翻译成客户端报文
                    await Gate(ProtocolHarness.GateId).Send(frame);
                }
            }

            await Task.Delay(delayMs);
        }
    }

    /// <summary>客户端登录：建立 gate、接上 client SDK、等玩家实体就绪。</summary>
    /// <param name="postToMainThread">
    /// 模拟引擎的主线程调度器（Stride/Godot 接入时要传的 ClientConfig.PostToMainThread）；
    /// null = SDK 保持线程池行为。
    /// </param>
    public async Task<TestClient> LoginAsync(string userId, Action<Action>? postToMainThread = null)
    {
        var gate = new MiniGate(this, GateId, userId);
        _gateByUser[userId] = gate;

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
        }, clientService, _ => gate);

        var connected = await session.Connect();
        if (!connected)
        {
            throw new InvalidOperationException("client connect failed");
        }

        // 等 CreatePlayerEntity 下发并创建客户端 Player
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
        return new TestClient(session, gate);
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
/// 模拟引擎主线程：所有回调排进队列，由 <see cref="Pump"/> 在同一个线程上 drain。
/// 用来验证 <c>ClientConfig.PostToMainThread</c> 的语义（线程归属 + 保序）。
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

        // 等线程自报 id
        var deadline = Environment.TickCount64 + 3000;
        while (MainThreadId == 0 && Environment.TickCount64 < deadline)
        {
            Thread.Sleep(5);
        }
    }

    public int MainThreadId { get; private set; }

    /// <summary>把委托排到「主线程」</summary>
    public void Post(Action action) => _queue.Add(action);

    /// <summary>等队列跑空（简单起见：排一个哨兵再等它执行完）</summary>
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

/// <summary>一个已登录的测试客户端：暴露 client SDK 的 Player 与 gate 信息，并收集推送事件。</summary>
internal sealed class TestClient(ClientSession session, MiniGate gate)
{
    public ClientSession Session { get; } = session;

    public ClientPlayer Player => Session.Player!;

    public MiniGate Gate { get; } = gate;

    public string UserId => Gate.UserId;

    public HubPlayerBase HubPlayer => Gate.HubPlayer!;

    /// <summary>等一个条件成立（客户端事件是异步分发的，断言前用这个等）。</summary>
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
