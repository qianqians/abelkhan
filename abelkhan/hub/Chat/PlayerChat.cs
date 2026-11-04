using chat;
using consts;
using engine;

namespace hub;

/// <summary>
/// 玩家实体上的聊天协议（频道列表 / 发言 / 历史 / 私聊）与作用域声明。
/// 编排逻辑在 <see cref="ChatService"/>，这里只做协议解析和结果拼装。
/// </summary>
public abstract partial class Player
{
    /// <summary>
    /// 游戏提供：该玩家在某个作用域里的 id（当前场景实例 / 帮会 id / 门派 id）。
    /// 返回 null 表示他不在这个作用域里（例如没有帮会 → 不能在帮会频道发言）。
    /// 全区 / 全服不需要实现（用 ChatSettings 里的固定 id）。
    /// </summary>
    protected internal virtual string? GetChatScope(ChatScope scope) => null;

    /// <summary>
    /// 游戏提供：内容过滤（敏感词 / 禁言 / 拉黑）。返回过滤后的内容；返回 null 或空表示拒发。
    /// 频道发言时 <paramref name="channelId"/> 是频道 id，私聊时是空字符串。
    /// </summary>
    protected internal virtual string? FilterChatContent(string channelId, string content) => content;

    /// <summary>
    /// 玩家作用域发生变化时调用：切场景 / 加入或退出帮会 / 转门派。
    /// <paramref name="scopeId"/> 为空表示离开该作用域。
    /// </summary>
    public void SetChatScope(ChatScope scope, string? scopeId)
    {
        _chatService?.SetScope(UserId, scope, scopeId);
    }

    /// <summary>重新解析全部作用域（游戏把相关数据改完后调一次即可）。</summary>
    public void RefreshChatScopes()
    {
        _chatService?.RefreshScopes(this);
    }

    private ChatService? _chatService;

    internal void RegisterChatRequests(Service service)
    {
        var chatService = service.Chat;
        if (chatService == null)
        {
            return;
        }

        _chatService = chatService;
        // 存档 / 登录后由服务端在 OnOnlineAsync 里解析一次，这里只是记下引用

        RegisterRequest<ClientChatChannels, ClientChatChannelsResult>(Consts.ClientChatChannels, _ =>
        {
            var result = new ClientChatChannelsResult();
            foreach (var channel in chatService.Channels)
            {
                result.Channels.Add(new ClientChatChannelInfo
                {
                    ChannelId = channel.Id,
                    Name = channel.Name,
                    Scope = (int)channel.Scope,
                    CooldownMs = (uint)Math.Max(0, channel.CooldownMs),
                    HistoryCount = (uint)Math.Max(0, channel.HistoryCount),
                });
            }

            return Result<ClientChatChannelsResult, string>.Ok(result);
        });

        RegisterRequest<ClientChatSend, ClientChatSendResult>(Consts.ClientChatSend, async req =>
        {
            var (result, entry) = await chatService.SendChannelAsync(this, req.ChannelId, req.Content);
            return result.Ok
                ? Result<ClientChatSendResult, string>.Ok(new ClientChatSendResult
                {
                    Entry = ChatService.ToProto(entry!),
                })
                : Result<ClientChatSendResult, string>.Err(ChatMessageOf(result.Code));
        });

        RegisterRequest<ClientChatHistory, ClientChatHistoryResult>(Consts.ClientChatHistory, async req =>
        {
            var (result, entries) = await chatService.HistoryAsync(this, req.ChannelId, (int)req.Count);
            if (!result.Ok)
            {
                return Result<ClientChatHistoryResult, string>.Err(ChatMessageOf(result.Code));
            }

            var response = new ClientChatHistoryResult();
            foreach (var entry in entries)
            {
                response.Entries.Add(ChatService.ToProto(entry));
            }

            return Result<ClientChatHistoryResult, string>.Ok(response);
        });

        RegisterRequest<ClientChatPrivate, ClientChatPrivateResult>(Consts.ClientChatPrivate, async req =>
        {
            var (result, entry) = await chatService.SendPrivateAsync(this, req.ToUserId, req.Content);
            return result.Ok
                ? Result<ClientChatPrivateResult, string>.Ok(new ClientChatPrivateResult
                {
                    Entry = ChatService.ToProto(entry!),
                })
                : Result<ClientChatPrivateResult, string>.Err(ChatMessageOf(result.Code));
        });
    }

    private static string ChatMessageOf(string code) => code switch
    {
        ChatCodes.UnknownChannel => "频道不存在",
        ChatCodes.EmptyContent => "不能发送空消息",
        ChatCodes.TooLong => "消息太长了",
        ChatCodes.Cooldown => "说得太快了，歇一下",
        ChatCodes.NotInScope => "你不在这个频道对应的范围里",
        ChatCodes.SelfTarget => "不能给自己发私聊",
        ChatCodes.Offline => "对方不在线",
        ChatCodes.Blocked => "消息包含不被允许的内容",
        _ => "聊天失败：" + code,
    };
}
