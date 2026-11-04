using consts;
using engine;
using leaderboard;

namespace hub;

/// <summary>
/// 玩家实体的排行榜协议处理（打榜 / 拉榜区间 / 查名次 / 重置）。
/// Redis 操作在 <see cref="LeaderboardStore"/>，这里做协议解析、区间校验和展示信息拼装。
/// </summary>
public abstract partial class Player
{
    internal void RegisterLeaderboardRequests(Service service)
    {
        var store = service.Leaderboard;
        if (store == null)
        {
            return;
        }

        // 打榜：以当前玩家身份上报分数，仅保留最高分
        RegisterRequest<ClientLeaderboardSubmit, ClientLeaderboardSubmitResult>(Consts.ClientLeaderboardSubmit,
            async req =>
            {
                if (string.IsNullOrWhiteSpace(req.BoardId))
                {
                    return Result<ClientLeaderboardSubmitResult, string>.Err("board_id 不能为空");
                }

                var (_, rank, score) = await store.SubmitAsync(req.BoardId, UserId, req.Score);
                return Result<ClientLeaderboardSubmitResult, string>.Ok(new ClientLeaderboardSubmitResult
                {
                    Rank = (uint)Math.Max(0, rank),
                    Score = score,
                });
            });

        // 拉取 [start, end) 名次区间（start 0-based，end 不包含）
        RegisterRequest<ClientLeaderboardRange, ClientLeaderboardRangeResult>(Consts.ClientLeaderboardRange,
            async req =>
            {
                if (string.IsNullOrWhiteSpace(req.BoardId))
                {
                    return Result<ClientLeaderboardRangeResult, string>.Err("board_id 不能为空");
                }

                var limit = Math.Max(1, store.Settings.RangePageLimit);
                var start = req.Start;
                var end = req.End <= start ? start + 1 : req.End;
                end = Math.Min(end, start + (uint)limit);

                var (entries, total) = await store.GetRangeAsync(req.BoardId, start + 1, end);
                var result = new ClientLeaderboardRangeResult { Total = (uint)Math.Max(0, total) };
                result.Entries.AddRange(await BuildEntriesAsync(service, entries));
                return Result<ClientLeaderboardRangeResult, string>.Ok(result);
            });

        // 查询名次（member_id 空 = 查自己）
        RegisterRequest<ClientLeaderboardRank, ClientLeaderboardRankResult>(Consts.ClientLeaderboardRank,
            async req =>
            {
                if (string.IsNullOrWhiteSpace(req.BoardId))
                {
                    return Result<ClientLeaderboardRankResult, string>.Err("board_id 不能为空");
                }

                var memberId = string.IsNullOrWhiteSpace(req.MemberId) ? UserId : req.MemberId;
                var entry = await store.GetEntryAsync(req.BoardId, memberId);
                var result = new ClientLeaderboardRankResult();
                if (entry != null)
                {
                    var built = await BuildEntriesAsync(service, new[] { entry });
                    result.Entry = built[0];
                }

                return Result<ClientLeaderboardRankResult, string>.Ok(result);
            });

    }

    /// <summary>把 store 条目拼成协议条目，并批量填充展示信息（未提供钩子时展示字段为空）。</summary>
    private async Task<List<ClientLeaderboardEntry>> BuildEntriesAsync(
        Service service, IReadOnlyList<leaderboard.LeaderboardEntry> entries)
    {
        var display = entries.Count == 0
            ? new Dictionary<string, LeaderboardDisplay>()
            : await service.ResolveLeaderboardDisplayAsync(
                entries.Select(e => e.MemberId).Distinct().ToList());

        var list = new List<ClientLeaderboardEntry>(entries.Count);
        foreach (var entry in entries)
        {
            var item = new ClientLeaderboardEntry
            {
                MemberId = entry.MemberId,
                Rank = (uint)Math.Max(0, entry.Rank),
                Score = entry.Score,
            };

            if (display.TryGetValue(entry.MemberId, out var d))
            {
                item.DisplayName = d.Name;
                item.Avatar = d.Avatar;
            }

            list.Add(item);
        }

        return list;
    }
}
