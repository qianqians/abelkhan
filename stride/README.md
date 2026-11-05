# Abelkhan + Stride 示例

把 `client`（ablekhan 的 C# 客户端 SDK）接进 Stride 4.4.1 的最小可运行工程。
它同时是「引擎接入」的参考实现：**线程模型、连接生命周期、事件转发**都在这里示范一遍。

```
stride/
├── Abelkhan.Stride.Game/           # 游戏本体（net10.0，Game Studio 用 .sdpkg 打开）
│   ├── AbelkhanGame.cs             # Game 子类：装主线程调度器 + 注册网络系统
│   ├── FreeCameraController.cs     # 模板自带风格的自由相机（和网络无关）
│   ├── SamplePlayer.cs             # client.Player 子类：把 SDK 事件转成游戏回调
│   ├── Net/
│   │   ├── MainThreadDispatcher.cs # 主线程队列 + SynchronizationContext
│   │   └── AbelkhanClientSystem.cs # GameSystem：连接 / 每帧 drain / 发言演示
│   ├── Assets/                     # Stride 场景 / 渲染配置（来自官方模板）
│   └── Abelkhan.Stride.Game.sdpkg  # Game Studio 的工程包清单
├── Abelkhan.Stride.Windows/        # Windows 启动器（net10.0-windows）
└── Abelkhan.Stride.slnx
```

## 1. 版本对齐

Stride **4.4.1 的目标框架就是 `net10.0`**，和 `client` / `engine` / `consts` / `proto` 完全一致，
所以**不需要**多目标（`docs/client-engine-integration.md` 里那条建议是针对 4.2/4.3 的）。

```
stride.engine/4.4.1/lib/net10.0/Stride.Engine.dll
```

## 2. 线程模型（接引擎最容易踩的坑）

| 阶段 | 跑在哪个线程 |
|---|---|
| WebSocket 收包、protobuf 解析、`Client` 内部状态机 | SDK 线程池 |
| `Entity.OnCreate` / `OnRefresh` / 服务器 notify / request handler | **主线程**（通过 `ClientConfig.PostToMainThread`） |
| 游戏逻辑 / `await player.Xxx()` 之后的续体 | **主线程**（主线程装了 `SynchronizationContext`） |

三个关键点：

1. **`ClientConfig.PostToMainThread = dispatcher.Post`** —— SDK 把实体回调排进主线程队列，
   并等它执行完才放行队列里的下一个（保序、不并发访问游戏对象）。
2. **主线程上 `SynchronizationContext.SetSynchronizationContext(dispatcher)`** ——
   这样 `await` 的续体也回到主线程，handler 里可以直接改 `Entity.Transform`。
3. **每帧 `dispatcher.Drain()`** —— 在 `AbelkhanClientSystem.Update` 的第一行，
   见 `Net/AbelkhanClientSystem.cs`。

> 队列只在该帧 drain：如果一帧内产生超过 `Drain(max)` 条回调，剩下的下一帧继续处理。

## 3. 跑起来

```bash
# 1) 起服务端（gate + hub + consul + redis + mongo），确保 gate 的 WebSocket 端口可连
# 2) 构建
dotnet build stride/Abelkhan.Stride.slnx

# 3) 运行（Windows）
stride/Bin/Windows/Debug/Abelkhan.Stride.Windows.exe \
    --wss=wss://127.0.0.1:9001/ --service=test_service --channel=world --autochat=8
```

也支持环境变量：`ABLEKHAN_WSS` / `ABLEKHAN_SERVICE` / `ABLEKHAN_USER` / `ABLEKHAN_CHANNEL` / `ABLEKHAN_AUTOCHAT`。

进来之后：

- 屏幕左上角叠加显示连接状态、频道列表、任务数、好友数、最近 8 条聊天；
- **空格** 发一条频道消息，**T** 给第一个好友发私聊；
- `--autochat=<秒>` 每隔 N 秒自动发一条（证明「发 → 广播 → 收到」闭环）。

用 **Game Studio** 打开 `Abelkhan.Stride.Game/Abelkhan.Stride.Game.sdpkg` 即可编辑场景。

## 4. 生命周期

```
Game.Initialize (主线程)
  ├─ SynchronizationContext = MainThreadDispatcher
  ├─ GameSystems.Add(new AbelkhanClientSystem(...))
  └─ client.Connect()                              → gate accept
       └─ 收到 NotifyConnID → 客户端发 VersionHandshake / ClientRequestService
            └─ hub 建实体 → CreatePlayerEntity → Client.UpsertEntity → Post 到主线程
                 └─ SampleClientService.CreatePlayerEntity → SamplePlayer
                      └─ 订阅 Friend*/Chat*/Guild*/Quest* 事件
```

退出时 `Game.OnExiting` → `AbelkhanClientSystem.Shutdown()` → `client.Close()`。

## 5. 已验证 / 未验证

- ✅ `dotnet build stride/Abelkhan.Stride.slnx` —— 0 错误（18 条警告全是 Stride 着色器编译器
  `X3557`，与接入代码无关）。
- ✅ 进程能起到 Stride 初始化阶段（日志 `[abelkhan] wss=...`）。
- ⚠️ **完整运行需要接真实的 gate**：本仓库没有把 gate/hub/consul 一起拉起来的脚本，示例本身
  只管「客户端怎么接」；没有 gate 时它会按 `AutoReconnect` 反复重连。
- ⚠️ 在无显示 / 无输入设备的沙箱里，Stride 会在 `DirectInput` 初始化时抛 `SharpDXException`
  （`HRESULT: 0x80004005`），这是运行环境限制，不是接入代码的问题。
- ✅ 主线程桥本身的语义有自动化测试保障：`abelkhan/tests/ProtocolSmoke` 的
  「引擎接入：ClientConfig.PostToMainThread 主线程桥」一节，断言 notify 全部跑在同一个线程且保序。

## 6. 换成你自己的游戏

1. 把 `SamplePlayer` 换成你的玩家实体（继承 `client.Player`），在 `OnCreate` 里解析服务器下发的 `argv`。
2. 事件订阅按需保留（`FriendRequested` / `ChatMessage` / `GuildInvited` / `QuestUpdated` …）。
3. 协议封装直接用（`player.SendChatMessage`、`player.CreateGuild`、`player.AcceptQuest`…），见
   `client/Player.*.cs` 与 `docs/*.md` 的「客户端（C# SDK）」小节。
