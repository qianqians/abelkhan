using client;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Input;

namespace Abelkhan.Stride.Net;

/// <summary>
/// 网络系统：把 ablekhan 的客户端跑在 Stride 的帧循环里。
///
/// 线程模型（这是接引擎最容易踩坑的地方）：
///   - 所有网络 IO / 报文解析在 SDK 内部的线程池上；
///   - 实体回调（OnCreate / 服务器 notify / request）通过
///     <see cref="ClientConfig.PostToMainThread"/> 排到 <see cref="MainThreadDispatcher"/>；
///   - 每帧 <see cref="Update"/> 先 drain 这个队列，再处理输入；
///   - 主线程上装了 SynchronizationContext，所以 <c>await player.Xxx()</c> 的续体也回到主线程。
/// </summary>
public sealed class AbelkhanClientSystem : GameSystem
{
    private readonly MainThreadDispatcher _dispatcher;
    private readonly AbelkhanClientOptions _options;
    private readonly SampleClientService _service;
    private readonly List<string> _status = new();

    private client.Client? _session;
    private SamplePlayer? _player;
    private bool _connecting;
    private double _autoChatTimer;
    private bool _spacePrev;
    private bool _tPrev;

    // 注意 global::：我们的命名空间是 Abelkhan.Stride.*，不加 global:: 时 Stride.Core 会被
    // 解析成 Abelkhan.Stride.Core（不存在）
    public AbelkhanClientSystem(global::Stride.Core.IServiceRegistry services, MainThreadDispatcher dispatcher,
        AbelkhanClientOptions options) : base(services)
    {
        _dispatcher = dispatcher;
        _options = options;
        _service = new SampleClientService(OnPlayerCreated);
    }

    public SamplePlayer? Player => _player;

    public override void Initialize()
    {
        base.Initialize();

        // 1) 主线程装 SynchronizationContext：await 之后回到主线程
        SynchronizationContext.SetSynchronizationContext(_dispatcher);

        // 2) 建会话：把「排到主线程」交给 SDK
        var config = new ClientConfig
        {
            Wss = _options.Wss,
            ServiceName = _options.ServiceName,
            UserId = _options.UserId,
            Argv = _options.Argv,
            IgnoreCertificate = _options.IgnoreCertificate,
            AutoReconnect = true,
            PostToMainThread = _dispatcher.Post,
        };

        _session = new client.Client(config, _service);
        _session.OnConnected += connId => AddStatus($"connected connId={connId}");
        _session.OnDisconnected += () => AddStatus("disconnected");
        _session.OnReconnected += connId => AddStatus($"reconnected connId={connId}");

        _ = ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        if (_session == null || _connecting)
        {
            return;
        }

        _connecting = true;
        try
        {
            var ok = await _session.Connect();
            AddStatus(ok ? "connect ok, waiting for player" : "connect FAILED");
        }
        catch (Exception e)
        {
            AddStatus("connect error: " + e.Message);
        }
        finally
        {
            _connecting = false;
        }
    }

    private void OnPlayerCreated(SamplePlayer player)
    {
        // 这里已经跑在主线程上了（Client.UpsertEntity 走 Entity.Post）
        _player = player;
        player.Log += line => Console.WriteLine("[abelkhan] " + line);
        AddStatus($"player ready id={player.EntityId}");

        // 上线后做一次常规拉取，验证整条链路
        _ = AfterLoginAsync(player);
    }

    private async Task AfterLoginAsync(SamplePlayer player)
    {
        try
        {
            var channels = await player.RequestChatChannels();
            if (channels.IsOk)
            {
                var names = string.Join(", ", channels.Value.Channels.Select(c => c.ChannelId));
                AddStatus($"channels: {names}");
            }
            else
            {
                AddStatus("chat channels error: " + channels.Error);
            }

            var quests = await player.RequestQuestList();
            AddStatus(quests.IsOk
                ? $"quests: active={quests.Value.Active.Count} available={quests.Value.Available.Count}"
                : "quests error: " + quests.Error);

            var friends = await player.RequestFriendList();
            AddStatus(friends.IsOk ? $"friends: {friends.Value.Friends.Count}" : "friends error: " + friends.Error);
        }
        catch (Exception e)
        {
            AddStatus("after-login error: " + e.Message);
        }
    }

    public override void Update(GameTime gameTime)
    {
        // 1) 先把网络事件在本帧主线程上跑掉
        _dispatcher.Drain();

        var player = _player;
        if (player == null)
        {
            if (!_connecting && _session is { IsConnected: false })
            {
                _ = ConnectAsync();
            }

            return;
        }

        var dt = gameTime.Elapsed.TotalSeconds;

        // 2) 自动发言（演示：证明发 -> 广播 -> 收到 的闭环）
        if (_options.AutoChatIntervalSeconds > 0)
        {
            _autoChatTimer += dt;
            if (_autoChatTimer >= _options.AutoChatIntervalSeconds)
            {
                _autoChatTimer = 0;
                _ = SendChatAsync(player, $"hello from stride {DateTime.Now:HH:mm:ss}");
            }
        }

        // 3) 手动：空格发频道消息，T 发私聊给第一个好友
        if (Game.Input.HasKeyboard)
        {
            var space = Game.Input.IsKeyDown(Keys.Space);
            if (space && !_spacePrev)
            {
                _ = SendChatAsync(player, "hello (space)");
            }

            _spacePrev = space;

            var t = Game.Input.IsKeyDown(Keys.T);
            if (t && !_tPrev)
            {
                _ = SendPrivateAsync(player);
            }

            _tPrev = t;
        }

        // 4) 屏幕左上角显示状态 + 最近消息
        DrawOverlay();
    }

    private async Task SendChatAsync(SamplePlayer player, string content)
    {
        var result = await player.SendChatMessage(_options.ChatChannel, content);
        if (!result.IsOk)
        {
            AddStatus($"chat error: {result.Error}");
        }
    }

    private async Task SendPrivateAsync(SamplePlayer player)
    {
        var friends = await player.RequestFriendList();
        if (!friends.IsOk || friends.Value.Friends.Count == 0)
        {
            AddStatus("私聊：没有好友");
            return;
        }

        var target = friends.Value.Friends[0].UserId;
        var result = await player.SendPrivateMessage(target, "hi from stride");
        AddStatus(result.IsOk ? $"已私聊 {target}" : $"私聊失败: {result.Error}");
    }

    private void DrawOverlay()
    {
        try
        {
            var lines = new List<string> { $"abelkhan: {(_session?.IsConnected == true ? "online" : "offline")}" };
            lines.AddRange(_status.TakeLast(4));
            lines.Add("空格=发言  T=私聊");
            lines.AddRange(_player?.RecentMessages.TakeLast(8) ?? []);

            Game.DebugTextSystem.Print(string.Join(Environment.NewLine, lines), new Int2(8, 8),
                (Color4?)null, (TimeSpan?)null);
        }
        catch
        {
            // 覆盖层只是调试用，画不出来不影响游戏
        }
    }

    private void AddStatus(string line)
    {
        Console.WriteLine("[abelkhan] " + line);
        _status.Add(line);
        if (_status.Count > 16)
        {
            _status.RemoveAt(0);
        }
    }

    /// <summary>退出时关掉连接（由 Game 在 OnExiting 里调用）。</summary>
    public void Shutdown()
    {
        _session?.Close().GetAwaiter().GetResult();
    }
}
