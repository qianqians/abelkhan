namespace leaderboard;

/// <summary>一条榜单条目（Redis 里取出的原始数据，未带展示信息）。</summary>
public sealed record LeaderboardEntry(string MemberId, double Score, long Rank);

/// <summary>游戏提供的玩家展示信息（昵称 / 头像等）。字段为空时客户端自行兜底。</summary>
public sealed record LeaderboardDisplay(string Name = "", string Avatar = "");
