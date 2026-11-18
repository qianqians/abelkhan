using consts;
using engine;

namespace client;

/// <summary>
/// 排行榜协议（打榜 / 区间拉榜 / 查名次）。
/// </summary>
public abstract partial class Player
{
    // ---------------- 排行榜（Redis ZSET） ----------------

    /// <summary>打榜：上报分数，仅保留最高分。返回打榜后的名次（1-based，0 = 不在榜）与当前分数。</summary>
    public Task<Result<ClientLeaderboardSubmitResult, string>> SubmitLeaderboardScore(string boardId, double score)
    {
        return Request<ClientLeaderboardSubmit, ClientLeaderboardSubmitResult>(
            Consts.ClientLeaderboardSubmit,
            new ClientLeaderboardSubmit { BoardId = boardId, Score = score });
    }

    /// <summary>拉取 [start, end) 名次区间（start 0-based，end 不包含）。</summary>
    public Task<Result<ClientLeaderboardRangeResult, string>> RequestLeaderboardRange(string boardId, uint start, uint end)
    {
        return Request<ClientLeaderboardRange, ClientLeaderboardRangeResult>(
            Consts.ClientLeaderboardRange,
            new ClientLeaderboardRange { BoardId = boardId, Start = start, End = end });
    }

    /// <summary>查询名次。memberId 为空 = 查自己；不在榜时返回 entry.rank = 0。</summary>
    public Task<Result<ClientLeaderboardRankResult, string>> RequestLeaderboardRank(string boardId, string memberId = "")
    {
        return Request<ClientLeaderboardRank, ClientLeaderboardRankResult>(
            Consts.ClientLeaderboardRank,
            new ClientLeaderboardRank { BoardId = boardId, MemberId = memberId });
    }
}
