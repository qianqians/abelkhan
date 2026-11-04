namespace chat;

/// <summary>
/// 聊天系统用到的 Redis key / pub/sub 通道。
/// 聊天不需要跨 key 的原子脚本（历史是单 key 的 LPUSH+LTRIM），所以不强制 hash tag。
/// </summary>
public sealed class ChatKeys
{
    private readonly string _prefix;

    public ChatKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    /// <summary>频道历史（list，最新在前）。</summary>
    public string History(string channelId, string scopeId) =>
        _prefix + "chat:history:" + channelId + ":" + scopeId;

    /// <summary>频道发言冷却。</summary>
    public string Cooldown(string userId, string channelId) =>
        _prefix + "chat:cd:" + userId + ":" + channelId;

    /// <summary>私聊发言冷却。</summary>
    public string PrivateCooldown(string userId) => _prefix + "chat:cd:pm:" + userId;

    /// <summary>跨 hub 频道广播的 pub/sub 通道。</summary>
    public string Broadcast => _prefix + "chat:broadcast";
}
