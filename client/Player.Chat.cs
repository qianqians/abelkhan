using consts;
using engine;

namespace client;

/// <summary>
/// 聊天协议（频道列表 / 发言 / 历史 / 私聊）与服务器推送的通知事件。
/// </summary>
public abstract partial class Player
{
    // ================================================================
    // 请求
    // ================================================================

    /// <summary>可用频道列表（含冷却与历史保留条数）。</summary>
    public Task<Result<ClientChatChannelsResult, string>> RequestChatChannels()
    {
        return Request<ClientChatChannels, ClientChatChannelsResult>(
            Consts.ClientChatChannels,
            new ClientChatChannels());
    }

    /// <summary>频道发言。频道不存在 / 冷却中 / 不在该频道作用域时返回错误。</summary>
    public Task<Result<ClientChatSendResult, string>> SendChatMessage(string channelId, string content)
    {
        return Request<ClientChatSend, ClientChatSendResult>(
            Consts.ClientChatSend,
            new ClientChatSend { ChannelId = channelId, Content = content });
    }

    /// <summary>拉该频道最近 N 条历史（按时间正序）。count = 0 用服务端默认上限。</summary>
    public Task<Result<ClientChatHistoryResult, string>> RequestChatHistory(string channelId, uint count = 0)
    {
        return Request<ClientChatHistory, ClientChatHistoryResult>(
            Consts.ClientChatHistory,
            new ClientChatHistory { ChannelId = channelId, Count = count });
    }

    /// <summary>点对点私聊（不限好友，仅在线；对方离线返回错误）。</summary>
    public Task<Result<ClientChatPrivateResult, string>> SendPrivateMessage(string toUserId, string content)
    {
        return Request<ClientChatPrivate, ClientChatPrivateResult>(
            Consts.ClientChatPrivate,
            new ClientChatPrivate { ToUserId = toUserId, Content = content });
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>收到一条频道消息（含自己发的）。</summary>
    public event Action<ClientChatMessageNotify>? ChatMessage;

    /// <summary>收到一条私聊消息。</summary>
    public event Action<ClientChatPrivateNotify>? ChatPrivate;

    private void RegisterChatNotifications()
    {
        TryRegisterNotify<ClientChatMessageNotify>(Consts.ClientChatMessageNotify,
            msg => ChatMessage?.Invoke(msg));
        TryRegisterNotify<ClientChatPrivateNotify>(Consts.ClientChatPrivateNotify,
            msg => ChatPrivate?.Invoke(msg));
    }
}
