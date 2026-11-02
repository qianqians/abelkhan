namespace admin;

/// <summary>
/// 无 PlayerId 的 GM 命令在 admin 进程本地执行。
/// 游戏侧通过 <see cref="Register"/> 注册处理器：admin 项目本身是普通程序集，
/// 游戏服务直接 ProjectReference 它，在启动 admin 前注册即可。
/// </summary>
public static class AdminGmHandlers
{
    private static readonly Dictionary<string, Func<string, Task<string>>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    static AdminGmHandlers()
    {
        // 示例：用于验证「本地执行」链路，游戏侧可覆盖或删除
        Register("echo", args => args);
    }

    /// <summary>注册同步本地命令处理器。</summary>
    public static void Register(string command, Func<string, string> handler) =>
        _handlers[command] = args => Task.FromResult(handler(args));

    /// <summary>注册异步本地命令处理器。</summary>
    public static void Register(string command, Func<string, Task<string>> handler) =>
        _handlers[command] = handler;

    /// <summary>执行本地命令，返回文本结果。</summary>
    public static async Task<string> RunAsync(string command, string args)
    {
        if (_handlers.TryGetValue(command, out var handler))
        {
            try
            {
                return await handler(args);
            }
            catch (Exception ex)
            {
                return $"error: {ex.Message}";
            }
        }
        return $"no local handler for '{command}'";
    }
}
