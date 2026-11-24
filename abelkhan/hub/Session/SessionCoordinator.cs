using System.Collections.Concurrent;
using System.Diagnostics;
using consts;
using core;
using engine;
using Google.Protobuf;

namespace hub;

/// <summary>
/// 顶号（登录挤占）与玩家实体跨 hub 迁移的编排层。
///
/// 从 MainClass 抽出来是为了可注入、可测试：harness 可以用同一份 Redis 起两个 coordinator
/// 模拟两个 hub，真跑跨 hub 的顶号与迁移。
///
/// 依赖全部是 MainClass 已有的集合，本类不拥有它们：
///   clients / gates / entities / services / globalEvent / enqueueMq。
/// </summary>
public sealed class SessionCoordinator
{
    private readonly RedisHandle _redis;
    private readonly string _hubId;
    private readonly ConcurrentDictionary<string, Client> _clients;
    private readonly ConcurrentDictionary<string, GateNetwork> _gates;
    private readonly ConcurrentDictionary<string, BaseEntity> _entities;
    private readonly Func<IEnumerable<Service>> _services;
    private readonly HubGlobalEvent? _globalEvent;
    private readonly Action<string> _enqueueMq;

    public SessionRegistry Sessions { get; }

    private readonly ConcurrentDictionary<string, string> _entityIdByUser = new();
    private readonly ConcurrentDictionary<string, long> _sessionSeq = new();
    private readonly ConcurrentDictionary<string, byte> _sessionHandlerRegistered = new();
    private readonly ConcurrentDictionary<string, MigrationState> _migrations = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _migrateQuiet = new();
    private readonly ConcurrentDictionary<string, string> _tombstones = new();
    private readonly ConcurrentDictionary<string, int> _tombForwardCount = new();

    public SessionCoordinator(RedisHandle redis, string hubId,
        ConcurrentDictionary<string, Client> clients,
        ConcurrentDictionary<string, GateNetwork> gates,
        ConcurrentDictionary<string, BaseEntity> entities,
        Func<IEnumerable<Service>> services,
        HubGlobalEvent? globalEvent,
        Action<string> enqueueMq,
        int sessionTtlSeconds = 90)
    {
        _redis = redis;
        _hubId = hubId;
        _clients = clients;
        _gates = gates;
        _entities = entities;
        _services = services;
        _globalEvent = globalEvent;
        _enqueueMq = enqueueMq;
        Sessions = new SessionRegistry(redis, hubId, sessionTtlSeconds);
    }

    public string HubId => _hubId;

    // ================================================================
    // 顶号
    // ================================================================

    /// <summary>
    /// 登录入口：能复用就复用（顶号），不能复用才创建；随后占会话记录并踢旧会话。
    /// </summary>
    public async Task<Player?> OnLoginAsync(Service service, string gateName, string connId, byte[] data)
    {
        var player = await LoginOrReuseAsync(service, gateName, connId, data);
        if (player == null)
        {
            return null;
        }

        var (prev, seq) = await Sessions.ClaimAsync(player.UserId, gateName, connId, player.EntityId);
        _sessionSeq[player.UserId] = seq;
        RegisterSessionReplacedHandler(player);

        if (prev != null && prev.ConnId != connId)
        {
            await ReplaceOldSessionAsync(prev, seq, player);
        }

        return player;
    }

    private async Task<Player?> LoginOrReuseAsync(Service service, string gateName, string connId, byte[] data)
    {
        var peeked = service.PeekLoginUserId(data);
        if (peeked != null && TryGetLivePlayer(peeked, out var reused))
        {
            await service.RebindAsync(reused!, gateName, connId);
            return reused;
        }

        var created = await service.EchoQueryServiceEntity(gateName, connId, data);
        if (created is not Player player)
        {
            return null;
        }

        if (TryGetLivePlayer(player.UserId, out var existing) && !ReferenceEquals(existing, player))
        {
            // 兜底去重（PeekUserId 未实现时）：丢弃刚创建的，复用旧的
            await service.DiscardAsync(player);
            await service.RebindAsync(existing!, gateName, connId);
            return existing;
        }

        RegisterEntity(player);
        return player;
    }

    /// <summary>登记实体到本 hub 的各张表（创建 / 迁移重建共用）。</summary>
    public void RegisterEntity(Player player)
    {
        _entities.TryAdd(player.EntityId, player);
        _globalEvent?.OnEntityCreate(player);
        _enqueueMq(player.EntityId);
        _entityIdByUser[player.UserId] = player.EntityId;
    }

    public bool TryGetLivePlayer(string userId, out Player? player)
    {
        player = null;
        if (!_entityIdByUser.TryGetValue(userId, out var entityId))
        {
            return false;
        }

        if (!_entities.TryGetValue(entityId, out var entity) || entity is not Player p)
        {
            return false;
        }

        player = p;
        return true;
    }

    private async Task ReplaceOldSessionAsync(PlayerSession prev, long newSeq, Player current)
    {
        if (prev.HubId == _hubId)
        {
            await KickConnAsync(prev.GateName, prev.ConnId, "账号在其他设备登录");
            if (prev.EntityId != current.EntityId &&
                _entities.TryGetValue(prev.EntityId, out var oldEntity) && oldEntity is Player oldPlayer)
            {
                await RetirePlayerAsync(oldPlayer, "replaced_local");
            }
        }
        else
        {
            // EntityId 带上旧实体：HubGlobalEvent 按它路由到旧 hub 的 player 实体
            await SendHubCommandAsync(prev.HubId, Consts.HubSessionReplaced, new HubSessionReplaced
            {
                UserId = current.UserId,
                EntityId = prev.EntityId,
                Seq = newSeq,
                Reason = "replaced_by_new_login",
            }, prev.EntityId);
        }
    }

    /// <summary>给实体挂「被顶号」处理：踢自己的旧连接 + 保存 + 摘除。幂等。</summary>
    private void RegisterSessionReplacedHandler(Player player)
    {
        if (!_sessionHandlerRegistered.TryAdd(player.EntityId, 0))
        {
            return;
        }

        player.RegisterHubNotify<HubSessionReplaced>(Consts.HubSessionReplaced, async (_, msg) =>
        {
            if (!_sessionSeq.TryGetValue(msg.UserId, out var local) || msg.Seq <= local)
            {
                return; // 迟到的旧消息，忽略
            }

            _sessionSeq[msg.UserId] = msg.Seq;
            if (_clients.TryGetValue(msg.UserId, out var cli))
            {
                await KickConnAsync(cli.GateName, cli.ConnId, "账号在其他设备登录");
            }

            if (_entities.TryGetValue(msg.EntityId, out var entity) && entity is Player p)
            {
                await RetirePlayerAsync(p, msg.Reason);
            }
        });
    }

    /// <summary>被挤掉 / 交出：游戏钩子 → 保存一次 → 摘除（clients / group / _entities / 跨 hub 路由）。</summary>
    public async Task RetirePlayerAsync(Player player, string reason)
    {
        foreach (var svc in _services())
        {
            try
            {
                await svc.OnSessionReplacedHookAsync(player, reason);
            }
            catch (Exception e)
            {
                Log.Error("session replaced hook err:{0}", e);
            }
        }

        try
        {
            player.Save();
        }
        catch (Exception e)
        {
            Log.Error("retire save err:{0}", e);
        }

        _entities.TryRemove(player.EntityId, out _);
        _globalEvent?.OnEntityDestroy(player);
        _entityIdByUser.TryRemove(player.UserId, out _);
        _sessionHandlerRegistered.TryRemove(player.EntityId, out _);

        foreach (var svc in _services())
        {
            await svc.DetachPlayerAsync(player.UserId, player);
        }
    }

    private async Task KickConnAsync(string gateName, string connId, string prompt)
    {
        if (string.IsNullOrEmpty(connId) || !_gates.TryGetValue(gateName, out var gate))
        {
            return;
        }

        var rpc = new WRpc();
        await gate.Send(rpc.Notify(Consts.HubKickOffClient, new HubKickOffClient
        {
            ConnId = connId,
            PromptInfo = prompt,
        }));
    }

    /// <summary>在线续期会话记录（seq 不匹配自动跳过，旧 hub 不会给新会话续命）。</summary>
    public async Task RefreshSessionsAsync()
    {
        foreach (var userId in _clients.Keys.ToList())
        {
            if (_sessionSeq.TryGetValue(userId, out var seq))
            {
                await Sessions.RefreshAsync(userId, seq);
            }
        }
    }

    // ================================================================
    // hub 级命令分发
    // ================================================================

    public async Task DispatchHubFrameAsync(HubHubNotifyEntity ev)
    {
        switch (ev.Event.ProtoName)
        {
            case Consts.HubMigrateRequest:
                await OnHubMigrateRequestAsync(ev);
                break;
            case Consts.HubMigrateForward:
                OnHubMigrateForward(ev);
                break;
            case Consts.HubMigrateQuiet:
                OnHubMigrateQuiet(ev);
                break;
            case Consts.HubMigrateResult:
                OnHubMigrateResult(ev);
                break;
            default:
                _globalEvent?.OnEntityEvent(ev.SrcHubId, ev.EntityId, ev.Event);
                break;
        }
    }

    private Task SendHubCommandAsync<T>(string hubId, string method, T msg, string entityId = "")
        where T : IMessage<T>, new()
    {
        var rpc = new WRpc();
        var frame = new HubHubNotifyEntity
        {
            SrcHubId = _hubId,
            EntityId = entityId,
            Event = new CallRpc { ProtoName = method, Content = msg.ToByteString() },
        };
        return _redis.PushList(string.Format(Consts.HubHubMq, hubId), frame.ToByteArray());
    }

    // ================================================================
    // 迁移
    // ================================================================

    /// <summary>
    /// 迁移入口（触发无关）：把玩家实体从本 hub 搬到 targetHubId。
    /// 六阶段：切割 → 落地 → 重建 → 转尾 → 接管 → 收尾，详见 docs/session-migration.md。
    /// </summary>
    public async Task<MigrationResult> MigrateAsync(string userId, string targetHubId,
        MigrateOptions? options = null)
    {
        var opt = options ?? new MigrateOptions();
        var started = Stopwatch.GetTimestamp();
        if (targetHubId == _hubId)
        {
            return new MigrationResult(false, MigrationCodes.SameHub);
        }

        if (!TryGetLivePlayer(userId, out var player) || player == null)
        {
            return new MigrationResult(false, MigrationCodes.NotFound);
        }

        var entityId = player.EntityId;
        var state = new MigrationState { UserId = userId, EntityId = entityId, TargetHub = targetHubId };
        if (!_migrations.TryAdd(entityId, state))
        {
            return new MigrationResult(false, MigrationCodes.AlreadyMigrating);
        }

        try
        {
            // 阶段 A：切割
            foreach (var svc in _services())
            {
                try
                {
                    await svc.OnEntityMigratingOutHookAsync(player, targetHubId);
                }
                catch (Exception e)
                {
                    Log.Error("migrating-out hook err:{0}", e);
                }
            }

            player.BeginMigrationBuffering();
            player.PostTask(() => state.Fence.TrySetResult());

            var fenceDone = await Task.WhenAny(state.Fence.Task, Task.Delay(opt.FenceTimeoutMs));
            if (fenceDone != state.Fence.Task)
            {
                return await FailMigrationAsync(player, state, MigrationCodes.FenceTimeout);
            }

            // 阶段 B：落地 + 带缓冲发请求
            var buffer = player.TakeMigrationBuffer();
            if (buffer.Count > opt.BufferCap)
            {
                return await FailMigrationAsync(player, state, MigrationCodes.BufferOverflow, buffer);
            }

            player.Save();
            state.Seq = _sessionSeq.TryGetValue(userId, out var sq) ? sq : 0;

            var request = new HubMigrateRequest
            {
                UserId = userId,
                EntityId = entityId,
                Seq = state.Seq,
                FromHub = _hubId,
            };
            foreach (var b in buffer)
            {
                request.Buffer.Add(ToProto(b));
            }

            await SendHubCommandAsync(targetHubId, Consts.HubMigrateRequest, request);

            var resultDone = await Task.WhenAny(state.Result.Task, Task.Delay(opt.MigrateTimeoutMs));
            if (resultDone != state.Result.Task)
            {
                return await FailMigrationAsync(player, state, MigrationCodes.Timeout);
            }

            var (ok, code) = await state.Result.Task;
            if (!ok)
            {
                return await FailMigrationAsync(player, state,
                    code == "no_loader" ? MigrationCodes.NotFound : MigrationCodes.TargetFailed);
            }

            // 阶段 D：转尾批 + 立墓碑 + 摘除
            var tail = player.TakeMigrationBuffer();
            var forward = new HubMigrateForward { EntityId = entityId, End = false };
            foreach (var b in tail)
            {
                forward.Items.Add(ToProto(b));
            }

            await SendHubCommandAsync(targetHubId, Consts.HubMigrateForward, forward);

            _tombstones[entityId] = targetHubId;
            _tombForwardCount[entityId] = 0;
            await RetirePlayerAsync(player, "migrated_out");
            _ = TombstoneGraceLoopAsync(entityId, targetHubId, opt);

            return new MigrationResult(true, MigrationCodes.Ok,
                (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
        catch (Exception e)
        {
            Log.Error("migrate user:{0} err:{1}", userId, e);
            return await FailMigrationAsync(player, state, MigrationCodes.TargetFailed);
        }
        finally
        {
            _migrations.TryRemove(entityId, out _);
        }
    }

    private static HubBufferedMsg ToProto(BufferedClientMsg b) => new()
    {
        ConnId = b.ConnId,
        MsgId = b.MsgId,
        Method = b.Method,
        Content = b.Content,
    };

    private async Task<MigrationResult> FailMigrationAsync(Player player, MigrationState state, string code,
        List<BufferedClientMsg>? buffered = null)
    {
        // 回滚：停止缓冲，把 buffer 里的按序本地执行，恢复服务
        player.AbortMigrationBuffering();
        foreach (var b in buffered ?? player.TakeMigrationBuffer())
        {
            player.PostTask(() => player.OnDoMsg(b.ConnId, b.MsgId, null, b.Method, b.Content));
        }

        _migrations.TryRemove(state.EntityId, out _);
        await Task.CompletedTask;
        return new MigrationResult(false, code);
    }

    /// <summary>墓碑静默期：一个 grace 周期内没有新转发（或到上限）→ 通知新 hub 可以 re-point。</summary>
    private async Task TombstoneGraceLoopAsync(string entityId, string targetHub, MigrateOptions opt)
    {
        var deadline = Environment.TickCount64 + opt.TombstoneMaxMs;
        var last = -1;
        while (Environment.TickCount64 < deadline)
        {
            await Task.Delay(Math.Max(200, opt.QuietGraceMs / 4));
            var now = _tombForwardCount.TryGetValue(entityId, out var c) ? c : 0;
            if (now == last && last >= 0)
            {
                break;
            }

            last = now;
        }

        _tombstones.TryRemove(entityId, out _);
        _tombForwardCount.TryRemove(entityId, out _);
        await SendHubCommandAsync(targetHub, Consts.HubMigrateQuiet, new HubMigrateQuiet { EntityId = entityId });
    }

    /// <summary>GateMsgHandle 发现实体不在本 hub：墓碑期内转发给目标 hub，否则记错误。</summary>
    public Task ForwardMissingEntityAsync(string entityId, string connId, string msgId, string method,
        byte[] content)
    {
        if (!_tombstones.TryGetValue(entityId, out var target))
        {
            Log.Error($"OnClientRequestHub Entity:{entityId} not found!");
            return Task.CompletedTask;
        }

        _tombForwardCount.AddOrUpdate(entityId, 1, (_, v) => v + 1);
        var forward = new HubMigrateForward { EntityId = entityId, End = false };
        forward.Items.Add(new HubBufferedMsg
        {
            ConnId = connId,
            MsgId = msgId,
            Method = method,
            Content = ByteString.CopyFrom(content),
        });
        return SendHubCommandAsync(target, Consts.HubMigrateForward, forward);
    }

    // ---------------- 新 hub 侧 ----------------

    private async Task OnHubMigrateRequestAsync(HubHubNotifyEntity ev)
    {
        var req = new MessageParser<HubMigrateRequest>(() => new HubMigrateRequest())
            .ParseFrom(ev.Event.Content.ToByteArray());

        Player? player = null;
        Service? owner = null;
        foreach (var svc in _services())
        {
            player = await svc.LoadOfflinePlayerAsync(req.UserId);
            if (player != null)
            {
                owner = svc;
                break;
            }
        }

        if (player == null || owner == null)
        {
            await SendHubCommandAsync(ev.SrcHubId, Consts.HubMigrateResult, new HubMigrateResult
            {
                UserId = req.UserId,
                EntityId = req.EntityId,
                Seq = req.Seq,
                Ok = false,
                Code = "no_loader",
            });
            return;
        }

        RegisterEntity(player);

        // 回包路由：clients 条目先挂上（不发 CreatePlayerEntity，re-point 在阶段 E）
        var sess = await Sessions.GetAsync(req.UserId);
        var gateName = sess?.GateName ?? string.Empty;
        var connId = sess?.ConnId ?? string.Empty;
        owner.AttachClientOnly(player, gateName, connId);

        // 从存档建出的实体必须重新挂协议 + 跑子系统上线同步，否则请求全部 method not exist
        await owner.AttachMigratedPlayerAsync(player);

        // 按序 replay 切割缓冲
        foreach (var b in req.Buffer)
        {
            player.PostTask(() => player.OnDoMsg(b.ConnId, b.MsgId, null, b.Method, b.Content));
        }

        var (_, seq) = await Sessions.ClaimAsync(req.UserId, gateName, connId, player.EntityId);
        _sessionSeq[req.UserId] = seq;
        RegisterSessionReplacedHandler(player);

        var quiet = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _migrateQuiet[player.EntityId] = quiet;

        await SendHubCommandAsync(ev.SrcHubId, Consts.HubMigrateResult, new HubMigrateResult
        {
            UserId = req.UserId,
            EntityId = req.EntityId,
            Seq = seq,
            Ok = true,
            Code = MigrationCodes.Ok,
        });

        // 阶段 E：等墓碑静默后再 re-point gate（保序的关键）
        _ = Task.Run(async () =>
        {
            var done = await Task.WhenAny(quiet.Task, Task.Delay(30_000));
            _migrateQuiet.TryRemove(player.EntityId, out _);
            if (done != quiet.Task)
            {
                Log.Error("migrate in: quiet timeout user:{0}", req.UserId);
            }

            await RePointGateAsync(player, gateName);
            foreach (var svc in _services())
            {
                try
                {
                    await svc.OnEntityMigratedInHookAsync(player, ev.SrcHubId);
                }
                catch (Exception e)
                {
                    Log.Error("migrated-in hook err:{0}", e);
                }
            }
        });
    }

    private void OnHubMigrateForward(HubHubNotifyEntity ev)
    {
        var fwd = new MessageParser<HubMigrateForward>(() => new HubMigrateForward())
            .ParseFrom(ev.Event.Content.ToByteArray());
        if (!_entities.TryGetValue(fwd.EntityId, out var entity))
        {
            Log.Error("migrate forward: entity {0} not here", fwd.EntityId);
            return;
        }

        foreach (var b in fwd.Items)
        {
            entity.PostTask(() => entity.OnDoMsg(b.ConnId, b.MsgId, null, b.Method, b.Content));
        }
    }

    private void OnHubMigrateQuiet(HubHubNotifyEntity ev)
    {
        var quiet = new MessageParser<HubMigrateQuiet>(() => new HubMigrateQuiet())
            .ParseFrom(ev.Event.Content.ToByteArray());
        if (_migrateQuiet.TryGetValue(quiet.EntityId, out var tcs))
        {
            tcs.TrySetResult(true);
        }
    }

    private void OnHubMigrateResult(HubHubNotifyEntity ev)
    {
        var result = new MessageParser<HubMigrateResult>(() => new HubMigrateResult())
            .ParseFrom(ev.Event.Content.ToByteArray());
        if (_migrations.TryGetValue(result.EntityId, out var state))
        {
            state.Result.TrySetResult((result.Ok, result.Code));
        }
    }

    /// <summary>新 hub 向玩家所在 gate 重发 CreatePlayerEntity → gate 把该 entity 的网络指向本 hub。</summary>
    private async Task RePointGateAsync(Player player, string gateName)
    {
        if (string.IsNullOrEmpty(gateName) || !_clients.TryGetValue(player.UserId, out var cli))
        {
            return;
        }

        await player.CreateRemotePlayer(cli);
    }
}
