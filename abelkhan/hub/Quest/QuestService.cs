using System.Collections.Concurrent;
using consts;
using core;
using engine;
using Google.Protobuf;
using quest;

namespace hub;

/// <summary>
/// 任务系统的编排层（hub 侧）。
///
/// 数据以 Redis 为权威（<see cref="QuestStore"/>），本类负责：
///   - 事件状态更新 + 把事件分发给进行中任务的进度条件；
///   - 触发判定（上线 / 事件 / 完成后跑），自动接取任务或让它们进「可接列表」；
///   - 完成判定（进度全满 + 完成门槛）与任务链（按 NextQuestIds 顺序挑后继）；
///   - 完成回调（游戏按 RewardKey 发奖）。
///
/// 并发前提：同一个玩家串行（Actor 队列 + 一个玩家只在一个 hub 上），所以这里按玩家
/// 「读状态 → 算增量 → 写回」是安全的，不需要把整条判定逻辑塞进 Lua。
/// </summary>
public sealed class QuestService
{
    private readonly QuestStore _store;
    private readonly RedisHandle _redis;
    private readonly WRpc _rpc = new();

    /// <summary>本 hub 在线的玩家：userId → Player（离线也能推进状态，只是不推通知）。</summary>
    private readonly ConcurrentDictionary<string, Player> _players = new();

    public QuestService(QuestStore store, RedisHandle redis)
    {
        _store = store;
        _redis = redis;
    }

    public QuestStore Store => _store;

    /// <summary>任务目录（游戏注册的配置）。由 MainClass 从 Service 注入。</summary>
    public IQuestCatalog Catalog { get; set; } = QuestCatalog.Empty;

    /// <summary>完成门槛的实时判定钩子（事件状态不满足时才会问它，例如「背包里有 5 个药水」）。</summary>
    public Func<Player, QuestDefinition, QuestCondition, bool>? CheckCondition { get; set; }

    /// <summary>任务完成回调（发奖励）。</summary>
    public Func<QuestCompletion, Task>? OnCompleted { get; set; }

    // ================================================================
    // 上下线
    // ================================================================

    public async Task OnOnlineAsync(Player player)
    {
        _players[player.UserId] = player;
        // 上线时跑一次触发判定：等级 / 场景这类事件可能是在离线期间变的
        var accepted = await EvaluateTriggersAsync(player.UserId);
        foreach (var questId in accepted)
        {
            await NotifyQuestAsync(player.UserId, questId, player);
        }
    }

    public Task OnOfflineAsync(string userId)
    {
        _players.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    // ================================================================
    // 事件上报（游戏调用）
    // ================================================================

    /// <summary>
    /// 上报一个事件：更新玩家事件状态 → 推进进行中任务的进度 → 触发判定 → （可选）自动完成。
    /// </summary>
    /// <param name="conditionType">事件类型，如 kill / level / talk。</param>
    /// <param name="conditionKey">事件 key，如 goblin / npc_7。</param>
    /// <param name="amount">数量或绝对值（取决于 <paramref name="mode"/>）。</param>
    /// <param name="mode">Count 累加 / Max 取最大 / Flag 置位。</param>
    public async Task ReportEventAsync(string userId, string conditionType, string conditionKey,
        long amount = 1, QuestConditionMode mode = QuestConditionMode.Count)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(conditionType))
        {
            return;
        }

        var key = QuestCondition.StoreKey(conditionType, conditionKey);
        Metrics.Count("abelkhan_quest_events_total", "Quest events reported", 1,
            ("type", conditionType));
        var value = await _store.ApplyEventAsync(userId, key, mode, amount);

        var active = await _store.GetActiveStatesAsync(userId);
        var touched = new List<string>();

        foreach (var (questId, _) in active)
        {
            if (!Catalog.TryGet(questId, out var definition) || definition == null)
            {
                continue;
            }

            var bumped = false;
            foreach (var condition in definition.Progress)
            {
                if (condition.ConditionKey != key)
                {
                    continue;
                }

                // Max 模式下上报的是绝对值，直接用它做进度；Count/Flag 用本次增量
                var delta = mode == QuestConditionMode.Count ? Math.Max(1, amount) : value;
                await _store.BumpProgressAsync(userId, questId, condition, delta);
                bumped = true;
            }

            if (bumped)
            {
                touched.Add(questId);
            }
        }

        // 进度可能刚好满了：配置了自动完成就直接交
        if (_store.Settings.AutoCompleteWhenReady)
        {
            foreach (var questId in touched.ToList())
            {
                var (ready, _) = await IsReadyAsync(userId, questId);
                if (ready)
                {
                    var (result, _) = await CompleteCoreAsync(userId, questId);
                    if (result.Ok)
                    {
                        touched.Remove(questId);
                    }
                }
            }
        }

        foreach (var questId in touched)
        {
            await NotifyQuestAsync(userId, questId, null);
        }

        // 触发判定：事件可能让某些任务变得可接
        var accepted = await EvaluateTriggersAsync(userId);
        foreach (var questId in accepted)
        {
            await NotifyQuestAsync(userId, questId, null);
        }
    }

    // ================================================================
    // 接取 / 完成（客户端调用）
    // ================================================================

    /// <summary>手动接取（非 AutoAccept 的任务）。</summary>
    public async Task<QuestResult> AcceptAsync(Player player, string questId)
    {
        if (!Catalog.TryGet(questId, out var definition) || definition == null)
        {
            return QuestResult.Fail(QuestCodes.UnknownQuest);
        }

        var events = await _store.GetEventStateAsync(player.UserId);
        var done = await _store.GetDoneAsync(player.UserId);
        var active = await _store.GetActiveStatesAsync(player.UserId);

        if (done.ContainsKey(questId))
        {
            return QuestResult.Fail(QuestCodes.AlreadyDone);
        }

        if (active.ContainsKey(questId))
        {
            return QuestResult.Fail(QuestCodes.AlreadyActive);
        }

        if (!CanBeAccepted(definition, events, done, active))
        {
            return QuestResult.Fail(QuestCodes.NotAvailable);
        }

        var result = await _store.StartQuestAsync(player.UserId, definition, Now(),
            _store.Settings.MaxActiveQuests);
        if (result.Ok)
        {
            await NotifyQuestAsync(player.UserId, questId, player);
        }

        return result;
    }

    /// <summary>提交完成：校验进度全满 + 完成门槛，然后走完成流程（含任务链）。</summary>
    public async Task<(QuestResult Result, List<string> NextQuestIds)> CompleteAsync(Player player, string questId)
    {
        if (!Catalog.TryGet(questId, out var definition) || definition == null)
        {
            return (QuestResult.Fail(QuestCodes.UnknownQuest), new List<string>());
        }

        var active = await _store.GetActiveStatesAsync(player.UserId);
        if (!active.ContainsKey(questId))
        {
            var done = await _store.GetDoneAsync(player.UserId);
            return (QuestResult.Fail(done.ContainsKey(questId)
                ? QuestCodes.AlreadyDone
                : QuestCodes.NotActive), new List<string>());
        }

        var progress = await _store.GetQuestProgressAsync(player.UserId, questId);
        if (!QuestRules.ProgressFull(definition, progress))
        {
            return (QuestResult.Fail(QuestCodes.ProgressNotFull), new List<string>());
        }

        if (!await GatesSatisfiedAsync(player, definition))
        {
            return (QuestResult.Fail(QuestCodes.GateNotSatisfied), new List<string>());
        }

        return await CompleteCoreAsync(player.UserId, questId);
    }

    /// <summary>
    /// 完成核心：标记完成 → 回调发奖 → 按 NextQuestIds 顺序挑后继 → 再跑一次根任务触发判定。
    /// </summary>
    private async Task<(QuestResult Result, List<string> NextQuestIds)> CompleteCoreAsync(string userId,
        string questId)
    {
        if (!Catalog.TryGet(questId, out var definition) || definition == null)
        {
            return (QuestResult.Fail(QuestCodes.UnknownQuest), new List<string>());
        }

        var now = Now();
        var result = await _store.CompleteQuestAsync(userId, definition, now);
        if (!result.Ok)
        {
            return (result, new List<string>());
        }

        Metrics.Count("abelkhan_quest_completed_total", "Quests completed");
        await NotifyCompletedAsync(userId, questId);

        if (OnCompleted != null)
        {
            try
            {
                await OnCompleted(new QuestCompletion(userId, definition.QuestId, definition.Title,
                    definition.RewardKey, now));
            }
            catch (Exception e)
            {
                Log.Error("quest: on completed hook err:{0}", e);
            }
        }

        // 任务链：按配置顺序挑第一个满足条件的后继。
        // 命中后停止——这就是「多后继 + 条件挑一」的分支语义；链上的任务不参与全局扫描，
        // 否则同一条链会被激活两次。
        var nextAccepted = new List<string>();
        var events = await _store.GetEventStateAsync(userId);
        var done = await _store.GetDoneAsync(userId);
        var active = await _store.GetActiveStatesAsync(userId);

        // 「多后继 + 条件挑一」：按配置顺序取第一个满足条件的后继
        var picked = QuestRules.PickNext(definition, Catalog, events, done.Keys.ToList(), active.Keys.ToList());
        if (picked != null)
        {
            if (Catalog.TryGet(picked, out var next) && next != null)
            {
                nextAccepted.Add(picked);
                if (next.AutoAccept)
                {
                    var started = await _store.StartQuestAsync(userId, next, now, _store.Settings.MaxActiveQuests);
                    if (!started.Ok)
                    {
                        Log.Error("quest: 接取后继 {0} 失败:{1}", picked, started.Code);
                    }
                }
            }
        }
        else if (definition.NextQuests.Count > 0)
        {
            Log.Error("quest: {0} 完成后没有后继满足触发条件（配置可能有缺口）", questId);
        }

        // 非链上的（根任务）可能也因为这次完成而可接了
        foreach (var accepted in await EvaluateTriggersAsync(userId))
        {
            nextAccepted.Add(accepted);
        }

        foreach (var id in nextAccepted)
        {
            await NotifyQuestAsync(userId, id, null);
        }

        return (QuestResult.Success(), nextAccepted);
    }

    // ================================================================
    // 判定
    // ================================================================

    /// <summary>能不能接（判定逻辑在 quest.QuestRules，便于单测）。</summary>
    private static bool CanBeAccepted(QuestDefinition definition, Dictionary<string, long> events,
        Dictionary<string, long> done, Dictionary<string, QuestState> active) =>
        QuestRules.CanBeAccepted(definition, events, done.Keys.ToList(), active.Keys.ToList());

    /// <summary>
    /// 触发判定：只扫「根任务」（没被任何任务当作后继的那些），避免链上的任务被激活两次。
    /// 返回本次自动接取的任务 id。
    /// </summary>
    private async Task<List<string>> EvaluateTriggersAsync(string userId)
    {
        var accepted = new List<string>();
        if (Catalog.Roots.Count == 0)
        {
            return accepted;
        }

        var events = await _store.GetEventStateAsync(userId);
        var done = await _store.GetDoneAsync(userId);
        var active = await _store.GetActiveStatesAsync(userId);
        var now = Now();

        foreach (var definition in Catalog.Roots)
        {
            if (!definition.AutoAccept)
            {
                continue;
            }

            if (!CanBeAccepted(definition, events, done, active))
            {
                continue;
            }

            var result = await _store.StartQuestAsync(userId, definition, now, _store.Settings.MaxActiveQuests);
            if (result.Ok)
            {
                active[definition.QuestId] = QuestState.Create(definition.QuestId, now);
                accepted.Add(definition.QuestId);
            }
        }

        return accepted;
    }

    /// <summary>完成门槛是否满足：先查事件状态，不满足再问游戏的实时判定钩子。</summary>
    private async Task<bool> GatesSatisfiedAsync(Player player, QuestDefinition definition)
    {
        if (definition.Completions.Count == 0)
        {
            return true;
        }

        var events = await _store.GetEventStateAsync(player.UserId);
        foreach (var condition in definition.Completions)
        {
            if (QuestRules.ConditionSatisfied(condition, events.GetValueOrDefault(condition.ConditionKey)))
            {
                continue;
            }

            // 事件状态不满足（例如要检查背包里的实时数量）→ 交给游戏
            var check = CheckCondition;
            if (check != null && check(player, definition, condition))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    /// <summary>进度是否全满、门槛是否满足（用于 ready_to_complete 与自动完成）。</summary>
    private async Task<(bool Ready, Dictionary<string, long> Progress)> IsReadyAsync(string userId, string questId)
    {
        if (!Catalog.TryGet(questId, out var definition) || definition == null)
        {
            return (false, new Dictionary<string, long>());
        }

        var progress = await _store.GetQuestProgressAsync(userId, questId);
        if (!QuestRules.ProgressFull(definition, progress))
        {
            return (false, progress);
        }

        // 门槛需要 Player 才能问钩子；离线时只用事件状态判定
        var events = await _store.GetEventStateAsync(userId);
        foreach (var condition in definition.Completions)
        {
            if (!QuestRules.ConditionSatisfied(condition, events.GetValueOrDefault(condition.ConditionKey)))
            {
                if (_players.TryGetValue(userId, out var player))
                {
                    var check = CheckCondition;
                    if (check != null && check(player, definition, condition))
                    {
                        continue;
                    }
                }

                return (false, progress);
            }
        }

        return (true, progress);
    }

    // ================================================================
    // 协议组装
    // ================================================================

    /// <summary>进行中 + 可接列表。</summary>
    public async Task<(List<ClientQuestInfo> Active, List<ClientQuestInfo> Available, int DoneCount)>
        BuildListAsync(Player player)
    {
        var events = await _store.GetEventStateAsync(player.UserId);
        var done = await _store.GetDoneAsync(player.UserId);
        var active = await _store.GetActiveStatesAsync(player.UserId);

        var activeInfos = new List<ClientQuestInfo>();
        foreach (var questId in active.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (!Catalog.TryGet(questId, out var definition) || definition == null)
            {
                continue;
            }

            var (ready, progress) = await IsReadyAsync(player.UserId, questId);
            activeInfos.Add(ToInfo(definition, QuestStatus.Active, progress, ready));
        }

        var availableInfos = new List<ClientQuestInfo>();
        foreach (var definition in Catalog.All.OrderBy(d => d.QuestId, StringComparer.Ordinal))
        {
            if (definition.AutoAccept || !CanBeAccepted(definition, events, done, active))
            {
                continue;
            }

            availableInfos.Add(ToInfo(definition, QuestStatus.Available, null, false));
        }

        return (activeInfos, availableInfos, done.Count);
    }

    /// <summary>组装单个任务（找不到定义返回 null）。</summary>
    public async Task<ClientQuestInfo?> BuildQuestAsync(Player player, string questId)
    {
        if (!Catalog.TryGet(questId, out var definition) || definition == null)
        {
            return null;
        }

        var active = await _store.GetActiveStatesAsync(player.UserId);
        if (active.ContainsKey(questId))
        {
            var (ready, progress) = await IsReadyAsync(player.UserId, questId);
            return ToInfo(definition, QuestStatus.Active, progress, ready);
        }

        var done = await _store.GetDoneAsync(player.UserId);
        if (done.ContainsKey(questId))
        {
            return ToInfo(definition, QuestStatus.Completed, null, false);
        }

        // 不在进行中也没完成：按「可接」展示（是否真的满足触发条件由 client_quest_list 的可接列表决定，
        // 这里主要用于「接到通知后拉单条任务」的场景）
        return ToInfo(definition, QuestStatus.Available, null, false);
    }

    private static ClientQuestInfo ToInfo(QuestDefinition definition, string status,
        Dictionary<string, long>? progress, bool ready)
    {
        var info = new ClientQuestInfo
        {
            QuestId = definition.QuestId,
            Title = definition.Title,
            Description = definition.Description,
            Status = status,
            RewardKey = definition.RewardKey,
            ReadyToComplete = ready,
        };

        foreach (var next in definition.NextQuests)
        {
            info.NextQuestIds.Add(next);
        }

        if (progress != null)
        {
            foreach (var condition in definition.Progress)
            {
                info.Progress.Add(new ClientQuestProgress
                {
                    Type = condition.Type,
                    Key = condition.Key,
                    Current = progress.GetValueOrDefault(condition.ConditionKey),
                    Target = condition.Target,
                });
            }
        }

        return info;
    }

    // ================================================================
    // 通知
    // ================================================================

    private async Task NotifyQuestAsync(string userId, string questId, Player? player)
    {
        var target = player ?? (_players.TryGetValue(userId, out var p) ? p : null);
        if (target == null)
        {
            return; // 离线：只推进状态，不推通知
        }

        var info = await BuildQuestAsync(target, questId);
        if (info == null)
        {
            return;
        }

        await NotifyAsync(target, Consts.ClientQuestUpdatedNotify, new ClientQuestUpdatedNotify { Quest = info });
    }

    private async Task NotifyCompletedAsync(string userId, string questId)
    {
        var player = _players.TryGetValue(userId, out var p) ? p : null;
        if (player == null)
        {
            return;
        }

        var info = new ClientQuestInfo { QuestId = questId, Status = QuestStatus.Completed };
        if (Catalog.TryGet(questId, out var definition) && definition != null)
        {
            info.Title = definition.Title;
            info.RewardKey = definition.RewardKey;
        }

        await NotifyAsync(player, Consts.ClientQuestUpdatedNotify, new ClientQuestUpdatedNotify { Quest = info });
    }

    /// <summary>推给某个本 hub 在线的玩家（走 gate 的客户端队列）。</summary>
    public async Task NotifyAsync<T>(Player player, string method, T msg) where T : IMessage<T>, new()
    {
        var forward = new GateForwardHubNotifyClientMq
        {
            UserId = player.UserId,
            EntityId = player.EntityId,
            Event = new CallRpc
            {
                ProtoName = method,
                Content = msg.ToByteString(),
            },
        };

        await _redis.PushList(string.Format(Consts.EntityClientMq, player.UserId),
            _rpc.Notify(Consts.GateForwardHubNotifyClientMq, forward));
    }

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
