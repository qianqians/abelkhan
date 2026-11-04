using consts;
using engine;
using quest;

namespace hub;

/// <summary>
/// 玩家实体上的任务协议（列表 / 接取 / 提交完成）。
/// 判定与编排都在 <see cref="QuestService"/>，这里只做协议解析和结果拼装。
/// </summary>
public abstract partial class Player
{
    private QuestService? _questService;

    internal void RegisterQuestRequests(Service service)
    {
        var questService = service.Quest;
        if (questService == null)
        {
            return;
        }

        _questService = questService;

        RegisterRequest<ClientQuestList, ClientQuestListResult>(Consts.ClientQuestList, async _ =>
        {
            var (active, available, doneCount) = await questService.BuildListAsync(this);
            var result = new ClientQuestListResult { CompletedCount = (uint)Math.Max(0, doneCount) };
            result.Active.AddRange(active);
            result.Available.AddRange(available);
            return Result<ClientQuestListResult, string>.Ok(result);
        });

        RegisterRequest<ClientQuestAccept, ClientQuestAcceptResult>(Consts.ClientQuestAccept, async req =>
        {
            var result = await questService.AcceptAsync(this, req.QuestId);
            if (!result.Ok)
            {
                return Result<ClientQuestAcceptResult, string>.Err(QuestMessageOf(result.Code));
            }

            var info = await questService.BuildQuestAsync(this, req.QuestId);
            return Result<ClientQuestAcceptResult, string>.Ok(new ClientQuestAcceptResult
            {
                Quest = info ?? new ClientQuestInfo { QuestId = req.QuestId },
            });
        });

        RegisterRequest<ClientQuestComplete, ClientQuestCompleteResult>(Consts.ClientQuestComplete, async req =>
        {
            var (result, nextIds) = await questService.CompleteAsync(this, req.QuestId);
            if (!result.Ok)
            {
                return Result<ClientQuestCompleteResult, string>.Err(QuestMessageOf(result.Code));
            }

            var response = new ClientQuestCompleteResult { QuestId = req.QuestId };
            response.NextQuestIds.AddRange(nextIds);
            return Result<ClientQuestCompleteResult, string>.Ok(response);
        });
    }

    private static string QuestMessageOf(string code) => code switch
    {
        QuestCodes.UnknownQuest => "任务不存在",
        QuestCodes.AlreadyActive => "这个任务已经在进行中",
        QuestCodes.AlreadyDone => "这个任务已经完成了",
        QuestCodes.NotActive => "你没有在进行这个任务",
        QuestCodes.NotAvailable => "还不能接这个任务",
        QuestCodes.ProgressNotFull => "任务进度还没满",
        QuestCodes.GateNotSatisfied => "完成条件还没满足",
        QuestCodes.TooManyActive => "进行中的任务太多了",
        _ => "任务操作失败：" + code,
    };
}
