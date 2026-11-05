using Abelkhan.Stride.Net;
using Stride.Engine;

namespace Abelkhan.Stride;

/// <summary>
/// 游戏入口：把 ablekhan 的客户端作为 GameSystem 挂进 Stride 的帧循环。
///
/// 职责只有三件事：
///   1. 主线程装 <see cref="MainThreadDispatcher"/> 作为 SynchronizationContext；
///   2. 注册 <see cref="AbelkhanClientSystem"/>；
///   3. 退出时关掉连接。
/// </summary>
public sealed class AbelkhanGame : Game
{
    private readonly MainThreadDispatcher _dispatcher = new();
    private readonly AbelkhanClientOptions _options;
    private AbelkhanClientSystem? _clientSystem;

    public AbelkhanGame(AbelkhanClientOptions? options = null)
    {
        _options = options ?? new AbelkhanClientOptions();
        IsMouseVisible = true;
    }

    public MainThreadDispatcher Dispatcher => _dispatcher;

    public AbelkhanClientSystem? ClientSystem => _clientSystem;

    protected override void Initialize()
    {
        base.Initialize();

        // 主线程（Initialize 就是在主线程上跑的）装上调度器
        SynchronizationContext.SetSynchronizationContext(_dispatcher);

        _clientSystem = new AbelkhanClientSystem(Services, _dispatcher, _options);
        GameSystems.Add(_clientSystem);

        Window.Title = $"Ablekhan + Stride — {_options.Wss}";
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        _clientSystem?.Shutdown();
        base.OnExiting(sender, args);
    }
}
