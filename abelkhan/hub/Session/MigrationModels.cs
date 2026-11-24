namespace hub;

/// <summary>迁移参数。</summary>
public sealed class MigrateOptions
{
    /// <summary>切割缓冲上限（条）。超过直接判失败（buffer_overflow），不做无界缓冲。</summary>
    public int BufferCap { get; set; } = 512;

    /// <summary>等 fence（切割前 job 跑完）的超时（毫秒）。</summary>
    public int FenceTimeoutMs { get; set; } = 5000;

    /// <summary>等新 hub 回执的超时（毫秒）。</summary>
    public int MigrateTimeoutMs { get; set; } = 10000;

    /// <summary>墓碑静默期（毫秒）：这段时间旧 hub 没有新转发，新 hub 才 re-point gate。</summary>
    public int QuietGraceMs { get; set; } = 2000;

    /// <summary>墓碑存活上限（毫秒），到期无条件删除并通知新 hub re-point。</summary>
    public int TombstoneMaxMs { get; set; } = 15000;
}

/// <summary>迁移结果码。</summary>
public static class MigrationCodes
{
    public const string Ok = "ok";
    public const string NotFound = "not_found";
    public const string AlreadyMigrating = "already_migrating";
    public const string SameHub = "same_hub";
    public const string FenceTimeout = "fence_timeout";
    public const string BufferOverflow = "buffer_overflow";
    public const string TargetFailed = "target_failed";
    public const string Timeout = "timeout";
}

/// <summary>迁移结果。</summary>
public sealed record MigrationResult(bool Ok, string Code = MigrationCodes.Ok, long ElapsedMs = 0);

/// <summary>一次迁移在旧 hub 侧的过程状态。</summary>
internal sealed class MigrationState
{
    public string UserId { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string TargetHub { get; init; } = string.Empty;
    public long Seq { get; set; }
    public TaskCompletionSource Fence { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource<(bool Ok, string Code)> Result { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int ForwardedAfterCut;
}
