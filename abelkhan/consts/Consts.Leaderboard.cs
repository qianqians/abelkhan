namespace consts;

/// <summary>
/// 排行榜协议方法名（对应 proto/leaderboard.proto）
/// </summary>
public partial class Consts
{
    // ---- 排行榜（对应 leaderboard.proto 的 ClientLeaderboard* 消息） ----
    public const string ClientLeaderboardSubmit = "client_leaderboard_submit";
    public const string ClientLeaderboardRange = "client_leaderboard_range";
    public const string ClientLeaderboardRank = "client_leaderboard_rank";
}
