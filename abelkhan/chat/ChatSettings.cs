namespace chat;

/// <summary>
/// 聊天系统运行参数。
///
/// 频道在代码里声明（不是后台配置）：每个频道自带作用域，消息群发给「该作用域内」的玩家。
/// 场景 / 帮会 / 门派的作用域 id 由游戏提供，全区 / 全服直接用这里的固定 id。
/// </summary>
public sealed class ChatSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>「全区」作用域的 id（一个部署里固定）。</summary>
    public string ZoneId { get; set; } = "zone-1";

    /// <summary>「全服」作用域的 id（一个部署里固定）。</summary>
    public string ServerId { get; set; } = "server-1";

    /// <summary>单条消息最大字符数。</summary>
    public int MaxLength { get; set; } = 200;

    /// <summary>私聊的发言间隔（毫秒），0 = 不限。</summary>
    public int PrivateCooldownMs { get; set; } = 500;

    /// <summary>拉历史时单次最多返回多少条（客户端请求会被夹到这个上限）。</summary>
    public int MaxHistoryFetch { get; set; } = 100;

    /// <summary>频道列表。默认给一套常见的：世界 / 全服 / 帮会 / 门派 / 当前。</summary>
    public List<ChatChannel> Channels { get; set; } =
    [
        new("world", "世界", ChatScope.Zone, 3000, 100),
        new("server", "全服", ChatScope.Server, 5000, 100),
        new("guild", "帮会", ChatScope.Guild, 1000, 50),
        new("sect", "门派", ChatScope.Sect, 1000, 50),
        new("scene", "当前", ChatScope.Scene, 1000, 0),
    ];

    /// <summary>按 id 查频道定义。</summary>
    public ChatChannel? FindChannel(string? channelId)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            return null;
        }

        foreach (var channel in Channels)
        {
            if (string.Equals(channel.Id, channelId, StringComparison.OrdinalIgnoreCase))
            {
                return channel;
            }
        }

        return null;
    }

    /// <summary>
    /// 把内容裁到上限内并做基础清洗（去首尾空白、丢掉控制字符）。
    /// 返回空字符串表示这条消息不合法。
    /// </summary>
    public string Sanitize(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(Math.Min(content.Length, Math.Max(1, MaxLength)));
        foreach (var ch in content.Trim())
        {
            if (char.IsControl(ch) && ch != '\n')
            {
                continue;
            }

            builder.Append(ch);
        }

        var text = builder.ToString().Trim();
        if (text.Length > Math.Max(1, MaxLength))
        {
            text = text[..Math.Max(1, MaxLength)];
        }

        return text;
    }
}
