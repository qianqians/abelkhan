namespace consts;

/// <summary>
/// 聊天协议方法名 + 通知（对应 proto/chat.proto）
/// </summary>
public partial class Consts
{
    // ---- 聊天（对应 chat.proto 的 ClientChat* 消息） ----
    public const string ClientChatChannels = "client_chat_channels";
    public const string ClientChatSend = "client_chat_send";
    public const string ClientChatHistory = "client_chat_history";
    public const string ClientChatPrivate = "client_chat_private";

    // 聊天通知（hub -> 客户端）
    public const string ClientChatMessageNotify = "client_chat_message_notify";
    public const string ClientChatPrivateNotify = "client_chat_private_notify";
}
