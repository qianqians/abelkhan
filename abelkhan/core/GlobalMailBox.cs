using Newtonsoft.Json.Linq;

namespace core;

/// <summary>邮件附件：物品 id + 数量。</summary>
public record GlobalMailAttachment(string ItemId, int Count);

/// <summary>一封全局邮件（已从 Redis list 里的 JSON 解析出来）。</summary>
public record GlobalMail(
    string MailId,
    string Title,
    string Content,
    string Sender,
    IReadOnlyList<GlobalMailAttachment> Attachments,
    long CreateTime,
    int ExpireSeconds,
    bool PopUp,
    int ValidDays,
    int MinLevel,
    int MaxLevel,
    int MinCreateDays,
    int MaxCreateDays);

/// <summary>
/// 全局邮件箱：全局邮件在所有玩家之间共用一份，存在 Redis list 里。
///
/// 玩家「请求邮件」时调用 <see cref="GetUnclaimed"/>：
///   1. 按最小刷新间隔（默认 5 秒）从 Redis 读取全局邮件列表并缓存到内存，避免每次请求都打 Redis；
///   2. 用 per-player 的已领取集合（Redis SET）过滤掉已经领过的邮件；
///   3. 按邮件自带的条件过滤：发布后有效天数、玩家等级范围、建角天数范围。
/// 领取指定邮件用 <see cref="GetClaimable"/> + <see cref="MarkClaimed"/>。
///
/// 邮件里的 PopUp 表示是否「拍脸」，由调用方决定怎么用（本类只透传）。
/// </summary>
public class GlobalMailBox
{
    private readonly RedisHandle _redis;
    private readonly string _mailListKey;
    private readonly string _claimedKeyFormat;
    private readonly long _refreshIntervalMs;

    private readonly object _cacheLock = new();
    private GlobalMail[] _cache = Array.Empty<GlobalMail>();
    private long _lastRefresh;

    /// <param name="redis">共享的 RedisHandle</param>
    /// <param name="mailListKey">全局邮件 list 的 key</param>
    /// <param name="claimedKeyFormat">已领取集合 key 格式，{0} = playerId</param>
    /// <param name="refreshIntervalMs">列表缓存的最小刷新间隔（频率约束）</param>
    public GlobalMailBox(RedisHandle redis, string mailListKey,
        string claimedKeyFormat = "admin_mail_claimed_{0}", long refreshIntervalMs = 5000)
    {
        _redis = redis;
        _mailListKey = mailListKey;
        _claimedKeyFormat = claimedKeyFormat;
        _refreshIntervalMs = refreshIntervalMs;
    }

    /// <summary>
    /// 返回该玩家当前可领取的全局邮件（已过滤已领取 / 有效天数 / 等级 / 建角天数）。
    /// level = 玩家等级，createDays = 建角天数。
    /// </summary>
    public async Task<List<GlobalMail>> GetUnclaimed(string playerId, int level, int createDays)
    {
        var mails = await GetMails();
        if (mails.Length == 0)
        {
            return new List<GlobalMail>();
        }

        var claimed = await _redis.SMembers(string.Format(_claimedKeyFormat, playerId));
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var result = new List<GlobalMail>(mails.Length);
        foreach (var mail in mails)
        {
            if (claimed.Contains(mail.MailId))
            {
                continue;
            }
            if (!IsValid(mail, now, level, createDays))
            {
                continue;
            }
            result.Add(mail);
        }
        return result;
    }

    /// <summary>取一封指定邮件，只有在「未领取且满足条件」时返回，否则返回 null。</summary>
    public async Task<GlobalMail?> GetClaimable(string playerId, int level, int createDays, string mailId)
    {
        if (string.IsNullOrEmpty(mailId))
        {
            return null;
        }

        var mails = await GetMails();
        var claimed = await _redis.SMembers(string.Format(_claimedKeyFormat, playerId));
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        foreach (var mail in mails)
        {
            if (mail.MailId != mailId)
            {
                continue;
            }
            if (claimed.Contains(mail.MailId))
            {
                return null;
            }
            if (!IsValid(mail, now, level, createDays))
            {
                return null;
            }
            return mail;
        }
        return null;
    }

    /// <summary>
    /// 把一封全局邮件标记为已领取。返回 true 表示本次成功标记；
    /// false 表示之前已经被领取过（并发/重复请求时只有一次会成功）。
    /// </summary>
    public Task<bool> MarkClaimed(string playerId, string mailId)
    {
        return _redis.SAdd(string.Format(_claimedKeyFormat, playerId), mailId);
    }

    // 带缓存 + 最小刷新间隔地读取全局邮件列表
    private async Task<GlobalMail[]> GetMails()
    {
        var now = Environment.TickCount64;
        lock (_cacheLock)
        {
            if (now - _lastRefresh < _refreshIntervalMs)
            {
                return _cache;
            }
        }

        var raw = await _redis.GetListRaw(_mailListKey);
        var mails = new List<GlobalMail>(raw.Count);
        foreach (var json in raw)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                continue;
            }

            var mail = Parse(json);
            if (mail != null)
            {
                mails.Add(mail);
            }
        }

        var result = mails.ToArray();
        lock (_cacheLock)
        {
            _cache = result;
            _lastRefresh = Environment.TickCount64;
        }
        return result;
    }

    private static bool IsValid(GlobalMail mail, long now, int level, int createDays)
    {
        if (mail.ValidDays > 0 && now - mail.CreateTime > (long)mail.ValidDays * 86_400_000L)
        {
            return false;
        }
        if (level < mail.MinLevel)
        {
            return false;
        }
        if (mail.MaxLevel > 0 && level > mail.MaxLevel)
        {
            return false;
        }
        if (createDays < mail.MinCreateDays)
        {
            return false;
        }
        if (mail.MaxCreateDays > 0 && createDays > mail.MaxCreateDays)
        {
            return false;
        }
        return true;
    }

    private static GlobalMail? Parse(string json)
    {
        try
        {
            var obj = JObject.Parse(json);
            var mailId = obj["mailId"]?.ToString();
            if (string.IsNullOrEmpty(mailId))
            {
                return null;
            }

            return new GlobalMail(
                mailId,
                obj["title"]?.ToString() ?? string.Empty,
                obj["content"]?.ToString() ?? string.Empty,
                obj["sender"]?.ToString() ?? string.Empty,
                ParseAttachments(obj["attachments"]),
                ReadLong(obj, "createTime"),
                ReadInt(obj, "expireSeconds"),
                ReadBool(obj, "popUp"),
                ReadInt(obj, "validDays"),
                ReadInt(obj, "minLevel"),
                ReadInt(obj, "maxLevel"),
                ReadInt(obj, "minCreateDays"),
                ReadInt(obj, "maxCreateDays"));
        }
        catch (Exception e)
        {
            Log.Error($"GlobalMailBox parse mail err:{e}");
            return null;
        }
    }

    private static IReadOnlyList<GlobalMailAttachment> ParseAttachments(JToken? token)
    {
        var result = new List<GlobalMailAttachment>();
        if (token is not JArray array)
        {
            return result;
        }

        foreach (var item in array)
        {
            if (item is not JObject obj)
            {
                continue;
            }

            var itemId = obj["itemId"]?.ToString();
            if (string.IsNullOrEmpty(itemId))
            {
                continue;
            }

            var count = ReadInt(obj, "count");
            result.Add(new GlobalMailAttachment(itemId, count));
        }
        return result;
    }

    private static int ReadInt(JObject obj, string name)
    {
        var token = obj[name];
        if (token == null || token.Type == JTokenType.Null)
        {
            return 0;
        }
        try
        {
            return token.Value<int>();
        }
        catch
        {
            return 0;
        }
    }

    private static long ReadLong(JObject obj, string name)
    {
        var token = obj[name];
        if (token == null || token.Type == JTokenType.Null)
        {
            return 0;
        }
        try
        {
            return token.Value<long>();
        }
        catch
        {
            return 0;
        }
    }

    private static bool ReadBool(JObject obj, string name)
    {
        var token = obj[name];
        if (token == null || token.Type == JTokenType.Null)
        {
            return false;
        }
        try
        {
            return token.Value<bool>();
        }
        catch
        {
            return false;
        }
    }
}
