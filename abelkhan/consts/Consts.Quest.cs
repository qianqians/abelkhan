namespace consts;

/// <summary>
/// 任务协议方法名 + 通知（对应 proto/quest.proto）
/// </summary>
public partial class Consts
{
    // ---- 任务（对应 quest.proto 的 ClientQuest* 消息） ----
    public const string ClientQuestList = "client_quest_list";
    public const string ClientQuestAccept = "client_quest_accept";
    public const string ClientQuestComplete = "client_quest_complete";

    // 任务通知（hub -> 客户端）
    public const string ClientQuestUpdatedNotify = "client_quest_updated_notify";
}
