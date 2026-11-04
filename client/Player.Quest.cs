using consts;
using engine;

namespace client;

/// <summary>
/// 任务协议（列表 / 接取 / 提交完成）与服务器推送的通知事件。
/// </summary>
public abstract partial class Player
{
    // ================================================================
    // 请求
    // ================================================================

    /// <summary>进行中（带逐条进度）+ 可接的任务列表。</summary>
    public Task<Result<ClientQuestListResult, string>> RequestQuestList()
    {
        return Request<ClientQuestList, ClientQuestListResult>(
            Consts.ClientQuestList,
            new ClientQuestList());
    }

    /// <summary>手动接取任务（校验触发条件）。</summary>
    public Task<Result<ClientQuestAcceptResult, string>> AcceptQuest(string questId)
    {
        return Request<ClientQuestAccept, ClientQuestAcceptResult>(
            Consts.ClientQuestAccept,
            new ClientQuestAccept { QuestId = questId });
    }

    /// <summary>提交完成（校验进度全满 + 完成门槛），返回本次激活 / 可接的后继任务。</summary>
    public Task<Result<ClientQuestCompleteResult, string>> CompleteQuest(string questId)
    {
        return Request<ClientQuestComplete, ClientQuestCompleteResult>(
            Consts.ClientQuestComplete,
            new ClientQuestComplete { QuestId = questId });
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>某条任务的状态 / 进度变化（含完成后触发的后继任务）。</summary>
    public event Action<ClientQuestUpdatedNotify>? QuestUpdated;

    private void RegisterQuestNotifications()
    {
        TryRegisterNotify<ClientQuestUpdatedNotify>(Consts.ClientQuestUpdatedNotify,
            msg => QuestUpdated?.Invoke(msg));
    }
}
