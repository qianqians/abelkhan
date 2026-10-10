# 客户端引擎接入评估：Stride / Godot / FlaxEngine

> 结论先行：
> - **工程集成 + 许可 + 长期可维护性** → **Stride 4.4**（纯 .NET、粘合代码最少、MIT、.NET Foundation）；
> - **画质 / 大世界工具 / 着色器生产力** → **Flax 1.12**（体积雾、TAA、DDGI、地形植被都有，且 .NET 10/C# 14 原生对齐）——
>   见第六节的逐项对比：这一项 **Flax 领先一代**，代价是自定义 EULA + 单主作者推进；
> - **要最大生态/中文资料/移动端覆盖** → **Godot**（但 C# 不能导出 Web，是硬伤）。
> 三个引擎都不需要改 `client/` 的协议与实体模型，只需要 3 处改动（见第五节）。

---

## 一、被接入的这层是什么（代码事实）

`client/` 是**引擎无关**的纯 .NET 类库，对三个候选引擎都没有任何引用：

| 事实 | 位置 |
|---|---|
| `net10.0` 类库，只引用 `engine` + `consts` + `Google.Protobuf 3.36.2` | `client/client.csproj:4,10-16` |
| `engine` / `consts` / `proto` 也都是 `net10.0` | `abelkhan/engine/engine.csproj:4`、`abelkhan/consts/consts.csproj:4`、`abelkhan/proto/proto.csproj:4` |
| 唯一传输是 **WebSocket 客户端**（`ClientWebSocket`） | `client/WebSocketNetwork.cs:9,33,91` |
| 服务端是 `UseHttps(pfx, password)`，所以必须 `wss://`，自签证书靠 `IgnoreCertificate` | `abelkhan/core/WebSocketAcceptService.cs:45-52`、`client/ClientConfig.cs:8-15` |
| 4 字节小端长度前缀 + 64KB 单包上限，自带粘包/半包 | `client/ReceiveBuffer.cs:12,41-56` |
| 心跳 3s（gate 侧 10s 无事件断开）、断线自动重连、被踢不重连 | `client/Client.cs:220-247,394-402` |
| 重连后实体重同步：`StartResync` 标未认领 → 服务器重下发 → `ResyncTimeoutMs` 后删本地残留 | `client/Client.cs:568-611` |
| 实体模型：`Service` 工厂 + `Player`/`Entity` 基类，每实体一条串行队列，服务器 request 自动回包 | `client/Service.cs:10-26`、`client/Entity.cs:39-64,94-128` |

**接入面非常小**：派生 3 个类（`Service`/`Player`/`Entity`）+ 一个主线程桥 + 一个可选的传输替换。
所以就"能不能接"而言，三个引擎都是可行的，差别在**代价与长期天花板**，不在可行性。

### 唯一的架构级摩擦：线程模型

- 读循环跑在 `TaskCreationOptions.LongRunning` 的独立线程上（`client/WebSocketNetwork.cs:23,33`）；
- `WRpc` 的回调直接在**读线程**上触发（`client/Client.cs:51-53` 订阅 → `OnNotify`/`OnRequest`/`OnResponse`）；
- 实体任务走 `ContinueWith(..., TaskScheduler.Default)`，即**线程池**（`client/Entity.cs:52-62`）。

也就是说：**`OnCreate` / `OnRefresh` / `RegisterNotify` 的 handler 全都在后台线程执行**。
三个引擎都要求场景对象只在主线程改，所以"回到主线程"这一层是**必须写**的，跟选谁无关；
而且 handler 里一旦 `await`，在没有 `SynchronizationContext` 的主循环上会**继续在线程池上跑**——
这层如果只做"投递 Action 到主线程队列"而不装 `SynchronizationContext`，是最容易踩的坑。

---

## 二、先解掉的两个硬约束（选谁之前就该做）

### 1) TFM 对齐

`net10.0` 的类库**不能被 net8.0 的工程引用**。当前各引擎的 .NET 要求：

- **Flax 1.12**：编辑器要求 **.NET SDK 10**，支持 C# 14（[官方文档](https://docs.flaxengine.com/manual/scripting/index.html)）→ 与我们最贴。
- **Godot**：master（4.8）文档写明 **需要 .NET 10 或更高**；但稳定线（4.6/4.7）仍是更低的 .NET，且 **C# 不能导出 Web**（[官方文档](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_web.html)）。
- **Stride 4.4**：放开限制，"Game Studio 可以打开使用更新版本 .NET 的工程"（[4.4 release notes](https://doc.stride3d.net/latest/en/ReleaseNotes/4.4.html)）。

建议：把 `client` / `engine` / `consts` / `proto` 改成多目标 **`net8.0;net10.0`**。
依据（已在本仓库 restore 结果里确认，不是推测）：`Google.Protobuf 3.36.2` 在 `net10.0` 下选中的就是
`lib/net8.0/Google.Protobuf.dll`（`client/obj/project.assets.json:5-13`），
且这 4 个工程没有使用任何 net9/net10 专属 API → 多目标基本零成本，却能同时兼容三家的稳定版。

### 2) 主线程桥

建议在 `client` 里开一个口子（默认行为不变，服务端/压测工具不受影响）：

```csharp
// ClientConfig 里加一个钩子
public Action<Action>? PostToMainThread { get; set; }   // null = 保持现在的线程池行为
```

`Entity.Post`（`client/Entity.cs:39-64`）改走这个钩子；引擎侧在**主线程**上装一个
`SynchronizationContext`（把队列在每帧 `Update` 里 drain），这样 handler 里的 `await` 之后也会回到主线程。
各引擎的"回主线程"手段：

| 引擎 | 回主线程 | 备注 |
|---|---|---|
| Flax | `Scripting.InvokeOnUpdate(...)`、`Scripting.MainThreadScheduler` | [Multithreading 文档](https://docs.flaxengine.com/manual/scripting/advanced/multithreading.html) 明确"编辑 gameplay 对象只能在主线程" |
| Godot | `CallDeferred`（注意：走 string 调用时必须是 snake_case 名，如 `CallDeferred("add_child")`） | [C# basics](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html) |
| Stride | 无一行式 API，自己用 `ConcurrentQueue` 在 `Game.Update`/`GameSystem` 里 drain（或用 `Stride.Core.MicroThreads` 的 `ScriptSystem`） | 需自行核实具体 API |

---

## 三、三引擎对照

| | Stride 4.4 | Godot 4.x | Flax 1.12 |
|---|---|---|---|
| 最新版本 | 4.4.0（2026-10-09，[GitHub](https://github.com/stride3d/stride/releases/tag/releases/4.4.0)） | 4.x 稳定线 / 4.8 开发线 | 1.12.6912（2026-05-18，[GitHub](https://github.com/FlaxEngine/FlaxEngine/releases/tag/1.12.6912)） |
| 语言/运行时 | 纯 C#/.NET（.NET Foundation） | C#（二等公民）+ GDScript + GDExtension | C# / C++ / Visual Script 混编，**要求 .NET SDK 10**、C# 14 |
| 引用现有 `client` 库 | 最直接（普通 csproj `ProjectReference`） | 可行（多目标 net8.0 即可） | 最直接（.NET 10 原生对齐） |
| 平台 | Windows/Linux/macOS/Android/iOS；4.4 起非 Windows 转正；NativeAOT/裁剪 | 全平台覆盖最广，含 Web（**但 C# 不能导出 Web**）；移动端 C# 自 4.2 起实验性 | Windows/Linux/macOS/Android/iOS/主机；**1.12 新增 Web 导出 + WebGPU 后端**（C# 在 Web 上的支持程度需实测） |
| 3D 能力 | Vulkan/D3D12 稳定、新 SPIR-V 着色器管线；大世界仍需自己搭 | 3D 可用但常被诟病；2D 很强 | 现代渲染（DDGI/SSR/体积雾等）最激进，画质天花板最高 |
| 编辑器/热重载 | Game Studio（目前 Windows 为主，跨平台重写中） | 编辑器成熟、迭代最快；C# 热重载**不保留状态**、改导出变量要 rebuild | Editor 支持 C# 热重载 + Visual Script |
| 生态/资料/招人 | 小（中文资料很少）；GitHub 7852 star / 1173 fork（2026-10 抓取） | **最大**，文档/教程/插件碾压 | 7000+ star / 720 fork，比想象的大，但基本单作者推进，中文资料极少 |
| 许可 | **MIT**（.NET Foundation） | MIT | **自定义 EULA**，GitHub 标记为 `Other / NOASSERTION`，商用前必读（[EULA](https://flaxengine.com/wp-content/uploads/2017/11/Flax%20EULA.pdf)） |
| 对我们最大的风险 | 生态小、编辑器平台限制 | C# 定位尴尬（Web 不能用、移动端实验性、双语言摩擦） | 生态最小，长期维护与招人风险最高 |

---

## 四、怎么选（决策树）

1. **目标平台是 PC 桌面 3D MMO、团队是 .NET 背景** → **Stride**。
   理由：`client` 库可以像普通类库一样被引用，网络层零改造；C# 是一等公民，没有 GDScript/interop 的双语言摩擦；
   4.4 刚把多平台与 D3D12/Vulkan 转正；MIT + .NET Foundation 许可最干净。
   代价：生态与中文资料少，招人难；编辑器目前以 Windows 为主。
2. **要最大生态、快速出原型、可能要做移动端、团队愿意用 GDScript 写表现层** → **Godot**。
   但要接受：**C# 不能导出 Web**（官方明确）；Android/iOS 上 C# 是实验性；C# 脚本改导出变量/信号要 rebuild、热重载丢状态。
   如果最终是"GDScript 写游戏 + C# 只做网络"，那 `client/` 这层的价值会大幅缩水（要么用 GDExtension 包一层，要么用 GDScript 重写协议）。
3. **硬需求是最高画质 3D、且能接受小生态** → **Flax 1.12**。
   它对 .NET 10 的贴合最自然（编辑器就要求 SDK 10），渲染最现代，2026-05 刚加 Web 导出。
   先核实三件事再决定：Web 导出下 C# 的支持程度、商业许可条款、社区规模/维护者数量。
4. **"浏览器里跑" 是硬需求** → Godot(C#) 和 Stride 都不行；
   只有 Flax 1.12 声称支持 Web 导出（需实测 C#），否则应该走 TypeScript 客户端
   （README 里本来就写了支持 ts 前端）。这一条经常直接决定结论，建议先明确。

**建议的决策方式**：用同一个 `client` 库，花 1–2 天分别接 Stride 和 Godot 的 30 行胶水（连上 gate → 收到
`CreatePlayerEntity` → 在场景里生成一个方块 → 移动同步回包），跑通再定，比看文档靠谱。

---

## 五、无论选谁，落地都要做的改动清单

1. **多目标 TFM**：`client` / `engine` / `consts` / `proto` → `<TargetFrameworks>net8.0;net10.0</TargetFrameworks>`。
2. **主线程桥**：`ClientConfig.PostToMainThread` + `Entity.Post` 改走它；引擎侧主线程装 `SynchronizationContext`。
   注意保持"每个实体一条串行队列"的顺序语义（`client/Entity.cs:39-64`）。
3. **传输抽象**：把 `WebSocketNetwork` 抽成接口（服务端已有先例：`abelkhan/core/INetwork.cs` 的
   `Task Send / void OnReceive / Task Close` 形状），桌面引擎继续用 `ClientWebSocket`，
   Godot 可换 `WebSocketPeer`，将来要上原生 TCP/TLS 也只换这一层。
4. **dev 环境的 `ws://`**：gate 现在只有 `UseHttps`（`abelkhan/core/WebSocketAcceptService.cs:45-52`），
   自签证书在不同引擎里的信任配置各不相同（.NET 侧有 `IgnoreCertificate`，Godot 侧要走 `TLSOptions`），
   开发期开一个明文 ws 端口能省掉大量踩坑。
5. **名称冲突**：`client.Entity` 与 `Stride.Engine.Entity` 同名，胶水代码里需要 `using` 别名。
6. **性能**（与引擎无关，但上引擎前值得知道）：每包至少两次分配——`new byte[len]`（`client/ReceiveBuffer.cs:55`）
   + protobuf 解析；`Entity.Post` 每次 `ContinueWith` 也会分配。
   MMO 20Hz × 数百实体的同步量下需要实测 GC，必要时改成"批量打包 + 复用缓冲"。

---

## 六、着色器与画质：Flax vs Stride（哪边更"现代"要分开看）

### 6.1 着色器系统

| | Stride 4.4（SDSL） | Flax 1.12（HLSL + META 宏） |
|---|---|---|
| 语言 | **SDSL = HLSL 的扩展**：支持 shader **类 / mixin / 继承**、自动编织 stage 输入输出；另有 **Effect 语言（C# 风格）** 在编译期做条件组合生成 permutation（[Graphics 手册](https://doc.stride3d.net/latest/en/manual/graphics/index.html)） | **就是标准 HLSL**（行业通用），用 `META_VS/PS/CS/HS/DS/GS`、`META_PERMUTATION_1..4`、`META_CB_BEGIN/END` 等宏标注；`.shader` 放 `Source/Shaders`，`.hlsl` 可 include（[Shaders 文档](https://docs.flaxengine.com/manual/graphics/shaders/index.html)） |
| 变体 | Effect 语言条件组合 + 4.4 后按 SPIR-S 模块组合，**文本只解析一次** | **显式静态声明** permutation（不生成多余变体），运行时按 index 选变体 |
| 编译管线 | 4.4 重写：`.sdsl` → SPIR-S（自家 SPIR-V 方言）→ `.sdfx` 在**字节码层**组合 → 标准 SPIR-V → SPIRV-Cross 出 D3D/Metal；D3D 侧也可出纯 HLSL。官方称这是为 **ray tracing / mesh shader / wave intrinsics** 铺路（[4.4 release notes](https://doc.stride3d.net/latest/en/ReleaseNotes/4.4.html)） | HLSL → 各平台原生（D3D/Vulkan/PS4…）；**打包时预编译，游戏运行时不能编译 shader**；编辑器内支持 shader 热重载 |
| 低层图形 API | 手册自述"从用户视角像 **D3D11**"；4.4 内部把 D3D12/Vulkan 做扎实（D3D12 需 Enhanced Barriers），但抽象仍是上一代风格 | 提供低层**面向对象** GPU API（`GPUDevice/GPUContext/GPUTexture/GPUBuffer/GPUPipelineState`，显式 SRV/UAV/CB 槽位绑定 + 反射），**C# 和 C++ 都能写自定义 pass**；后端 D3D11/D3D12/Vulkan/**WebGPU**/原生 |
| CPU 侧管线可扩展性 | `RenderFeature` / `RenderStage` / `RenderView` / `VisibilityGroup`，分 **collect → extract → prepare → draw** 四阶段，可插拔可并行（[Rendering pipeline](https://doc.stride3d.net/latest/en/manual/graphics/rendering-pipeline/index.html)）——思路接近 Unity SRP | 可自定义后处理/全屏/计算着色器与自定义几何绘制，但"整个渲染管线"级别的框架化程度弱一些 |

**结论（着色器）**：
- 论**语言与编译架构的现代性**，**Stride 4.4 赢**：类/mixin/继承 + Effect 语言 + SPIR-V 字节码级 effect 组合，是真·下一代做法（这也解释了"Game Studio 能开更新 .NET 的工程"，引擎在往上走）。
- 论**运行时光栅抽象 + 用 C#/C++ 直接写渲染的生产力**，**Flax 赢**：原生 HLSL 零学习成本、低层 GPU API 开放、WebGPU 后端、编辑器热重载。
- 论**日常出图效率**，Flax 更省事（会 HLSL 就能上手 + Visject 图）；Stride 的 SDSL 能力强但曲线更陡、资料更少。

### 6.2 画质（引擎自带渲染特性）

| 能力 | Stride 4.4（官方手册） | Flax 1.12（官方手册 / release notes） |
|---|---|---|
| 全局光照 | **Light probes**（[Lights and shadows](https://doc.stride3d.net/latest/en/manual/graphics/lights-and-shadows/index.html)）；另有 **Voxel Cone Tracing GI** 单页（实验性质） | 烘焙 lightmap（[GI 章节](https://docs.flaxengine.com/manual/graphics/lighting/index.html)）+ **DDGI 动态 GI**（1.12 反复优化 irradiance 过滤/采样/cascade 混合）+ Global SDF 驱动的效果 |
| 反射 | **Local reflections**（SSR），文档明确"会使 MSAA 失效"（[Post effects](https://doc.stride3d.net/latest/en/manual/graphics/post-effects/index.html)） | SSR + **Hi-Z 加速**（1.12）；Environment Probe + **Box Projection** |
| 抗锯齿 | 手册只写了 FXAA + MSAA，**但引擎源码里有完整的 TAA**（`TemporalAntiAliasEffect` + `TemporalAntiAliasShader.sdsl`，4.4 tag 内确认），配套的**速度缓冲**也在：`VelocityTargetSemantic`（语义 → `VelocityOutput` 着色器类）+ `MeshVelocityRenderFeature`（逐视图/逐对象上一帧矩阵）。**文档滞后，别以手册为准**——详见 8.3 | **TAA**（1.12 仍在修 TAA 伪影）+ TAAU 时域上采样 + DLSS/FSR 插件 + `RenderScale`（[Upscaling](https://docs.flaxengine.com/manual/graphics/overview/upscaling.html)） |
| 雾/大气 | **只有指数深度雾**，没有体积雾系统（另有 Light shafts 光轴效果）（[Fog](https://doc.stride3d.net/latest/en/manual/graphics/post-effects/fog.html)） | Exponential Height Fog + **Volumetric Fog**（1.12 继续优化质量与性能）（[Fog effects](https://docs.flaxengine.com/manual/graphics/fog-effects/index.html)） |
| 深度精度 | 手册未强调 | **Reverse Z 默认开启**（[Rendering overview](https://docs.flaxengine.com/manual/graphics/overview/index.html)） |
| 后处理 | Bloom / Bright filter / ToneMap / Gamma / Film grain / Vignette / DOF / Lens flare / Light streaks / Outline / AO / Local reflections | Bloom / Eye Adaptation / ToneMap / Color Grading / DOF / Motion Blur / Chromatic Distortion / Vignette / AO / SSR（1.12 另加 MSDF 字体等） |
| 大世界工具 | graphics 章节**没有地形/植被系统**（需自建或第三方，详见第七节）；引擎内有**纹理流式**（`Stride.Streaming.StreamingManager`）与**实例化**（`Stride.Engine.Processors.InstancingProcessor`） | 内置 **Terrain + Foliage**（1.12 还在修地形碰撞、植被 dithered LOD） |
| 渲染路径 | forward renderer + deferred shading，Graphics Compositor 可编排 | **Forward 与 Deferred 两种**（1.12 在修两者在雾/反射上的一致性） |
| 现代渲染议题 | 4.4 把 Vulkan/D3D12 做扎实、NativeAOT 支持 | WebGPU 后端、SPIR-V 压缩、GPU 查询 API（occlusion queries） |

**结论（画质）**：**Flax 明显领先一代**。对 MMORPG 这种大世界项目，**地形/植被、体积雾、TAA** 是硬需求——
这三项 Stride 的官方手册里都没有（只有 FXAA/MSAA、指数雾、无地形）。Stride 的后处理数量不少，
但缺时域抗锯齿和体积雾这两项对"看起来现代"影响最大。

### 6.3 需要提醒的三点

1. **"文档没写" ≠ "没有"**：Stride 的地形/植被可能在示例或第三方包里存在；Flax 的 GI 章节写的是 lightmap，
   DDGI 主要出现在 release notes 里（文档滞后）。真机验证前不要把上表当绝对结论。
2. **画质 = 引擎 + 美术管线**：两个引擎的官方示例/内容都比 Unity/Unreal 弱，MMO 的观感更多取决于美术规范、
   LOD/贴图预算和 TAA/上采样调优，而不是引擎特性表。
3. **风险面**：Flax 是自定义 EULA + 单主作者推进（渲染几乎所有条目出自同一作者），
   Stride 是 MIT + .NET Foundation + 更多贡献者。**Flax 的上限更高，Stride 的下限更稳。**

---

## 七、Stride 的地形 / 植被盘点

### 7.1 官方：没有，而且短期不会有

- `stride3d/stride` issue **#54 "Terrain System"**：2018-08-05 开，至今 **open**，最后活动 2022-03-11，9 条评论、**23 个 👍**，
  标签 `enhancement / area-Graphics / area-Physics`，**没有里程碑**（[#54](https://github.com/stride3d/stride/issues/54)）。
- 2024-09 的 #2425 与 2026-06 的 #3199（正文原文："as Flax Engine also has Terrain functionality. I think Stride needs this feature."）
  分别以 completed / **duplicate** 关闭 → 官方立场是"已知需求，不新增 issue"，**无排期**。
- 官方手册 graphics 章节里没有 Terrain / Foliage 页面；只有社区资源页
  [Terrain and Water](https://doc.stride3d.net/latest/en/community-resources/terrain-and-water.html) 列社区方案。

**结论：地形要按"用社区件 + 自己改"来排预算，不要等官方。**

### 7.2 社区方案清单（全部 MIT，所以只付维护成本、没有法务成本）

| 方案 | 活跃度 | 能力 | 风险 |
|---|---|---|---|
| **[TR.Stride](https://github.com/johang88/TR.Stride)**（johang88）59★ / 10 fork，最后提交 **2026-03-23** | 唯一"活着"的完整方案 | 模块化：`TR.Stride.Terrain`（高度图地形 + **植被**）、`TR.Stride.Atmosphere`（[UE Sky Atmosphere](https://github.com/sebh/UnrealEngineSkyAtmosphere) 移植）、`TR.Stride.Ocean`（[FFT Ocean](https://github.com/gasgiant/FFT-Ocean/) 移植）、`TR.Stride.Gameplay`、`TR.Stride.Windows` | 8 个 open issue；README 只写了 Atmosphere/Ocean 用法，地形/植被要看代码或 wiki；植被层要手动挂 Model + **Instancing** 组件，顺序错了 Game Studio 下次启动会崩 |
| **[StrideTerrainEditor](https://github.com/Idomeneas1970/StrideTerrainEditor)**（Idomeneas1970）11★ / 1 fork，最后提交 2026-01-25 | 唯一带 **Game Studio 编辑器**的方案 | 刷高度、8 张纹理混合/绘制、Perlin/Voronoi 生成、世界分块 tile + 异步加载 + 接缝缝合、Area 页签摆树/草/水面、图像工具 | **基于 Stride 4.1**，作者原话：4.2 起 shader 自动编译（`Stride.Core.Assets.CompilerApp`）跑不通、"升级可能出问题"；**摆放的物体没有实例化**（每个对象一个 entity，FPS 有代价）；地形基底最大 1024×1024；11★ = 单人项目 |
| **[StrideTerrain](https://github.com/johang88/StrideTerrain)**（johang88）24★，最后提交 **2020-07** | 已被 TR.Stride 取代 | 社区页描述为"现代高度图地形系统，带 LOD 与纹理 splatting" | 6 年未更新，仅作参考 |
| Xenko 时代样例：XenkoTerrain / XenkoMCTerrain（marching cubes 体素）/ XenkoHMTerrain（灰度图高度图）/ XenkoByteSized（细分平面）/ StrideVoxelScape | 基本停更 | 学习材料 | 2020 年已有"导入崩编辑器"的 issue（[#804](https://github.com/stride3d/stride/issues/804)） |
| 水面：StrideSimpleWater、XenkoFlowingWater | — | 简单/流动水面 | — |

### 7.3 植被具体能做到什么

引擎侧**有**实例化基础设施：`Stride.Engine.Processors.InstancingProcessor` + InstancingComponent，
社区工具包提供 [GPU Instancing 示例](https://stride3d.github.io/stride-community-toolkit/manual/code-only/examples/instancing.html)，
参考项目 [tebjan/StrideTransformationInstancing](https://github.com/tebjan/StrideTransformationInstancing)。

TR.Stride 的 `TerrainVegetationComponent`（源码已读）就在这之上：**每层植被**一个组件，靠
**mask 贴图 + 通道（RGBA 任选）**决定分布，可调 `Density`、`MinScale/MaxScale`、
**`MinSlope/MaxSlope`**、**`MinHeight/MaxHeight`**、`Seed`，以及 `ViewDistance`（默认 64）
与 `UseDistanceScaling`（接近视距上限时缩放到 0 淡出）。

即：**掩码散布 + 坡度/高度过滤 + 实例化 + 距离淡出**这套"够用"的植被是现成的；
**风场摆动、角色交互压草、多级 LOD、阴影级联策略**这些高级项没看到，要自己在 shader/代码里补。

### 7.4 对这个 MMO 项目的实际含义

- **把它当"集成 + 维护"而不是"从零造"**：TR.Stride 可作为起点（Terrain/Vegetation/Atmosphere/Ocean 一次到位、MIT、可读可改），
  StrideTerrainEditor 可当美术工具但要先解决 4.1→4.4 的版本问题（或只当参考实现）。
- **工作量级**：把 TR.Stride 拉进工程并跑通 = 天级；补风场/交互/LOD/流式/阴影 = 周级；不依赖编辑器、纯代码路线（Stride 对 code-only 友好）。
- **代价要认清**：你要 fork 一个 11★ 或 59★ 的单人项目并自己养；官方 4.5 变更有可能会打断它。
- **什么情况下这条线不划算**：如果地形/植被必须开箱即用、美术要自己刷大地图，那正是 Flax 的强项
  （内置 Terrain + Foliage + 体积雾 + TAA），Stride 这条路的差距会直接摊到工期上。

---

## 八、按"传送门分图、无大世界"的实际范围重新评估

**前提（项目范围）**：不做无缝大世界；地图是若干独立场景，用**传送光圈**互相连接；单图内有**山、水、树木、草皮**。

### 8.1 这一刀砍掉了 Stride 的哪些短板

- **世界流式/分区加载**：不需要 → TR.Stride 没有流式、StrideTerrainEditor 的 1024² 上限、tile 缝合与异步加载，全都不是问题。
- **超远视距 LOD / 大规模植被剔除**：不需要 → 单图视距 64–200m 足够，`TerrainVegetationComponent.ViewDistance` 正好是这个量级。
- **跨图无缝接管、兴趣迁移**：不需要 → 换图就是关卡切换 + 加载画面。
- **纹理流式**（`Stride.Streaming`）：锦上添花，不再是必需。

→ 结论：**"大世界工具"这一栏在 Stride 上的劣势，对本项目不成立。**

### 8.2 这个范围下仍然要做的，和 Stride 侧方案

| 需求 | 方案 | 备注 |
|---|---|---|
| 山 / 地形 | 每图一张高度图地形：TR.Stride 的 `TerrainComponent`（MIT），或直接用美术做好的 mesh + 高度图碰撞体 | 单图小，1024² 够；**判定与寻路应在服务端（hub）**，客户端只做插值与预测 |
| 水 | `TR.Stride.Ocean`（FFT，偏海面、可能过重）或 StrideSimpleWater（平面 + 反射/折射/波动） | 湖/河够用；**游泳、溺水、阻挡等规则在服务端** |
| 树 / 草 | TR.Stride `TerrainVegetationComponent`：mask 贴图 + density + min/max scale / slope / height + seed + ViewDistance + 距离缩放，底层走引擎 Instancing | 现成可用；**风摆、角色交互压草、多级 LOD 要自己补** |
| 传送光圈 | 纯玩法问题：进入 → 服务端切图 → 客户端加载目标场景 | 客户端可复用 `Client` 已有的 **epoch + 重同步**路径（`client/Client.cs:568-611`），不必另造一套 |

### 8.3 这个范围下**新增**的两个真风险（比 GI 更该先想）

1. **草皮闪烁 —— 已查证：Stride 有 TAA，也有 motion vector（不再是"要自研"）**
   源码实证（全部取自 `releases/4.4.0` tag）：
   - `TemporalAntiAliasEffect.cs`（`Stride.Rendering.Images`）：`RequiresDepthBuffer => true`、**`RequiresVelocityBuffer => true`**；
     实现是完整 TAA —— 16 帧 jitter 序列、按最浅深度做**速度膨胀**、3×3 邻域 min/max AABB 裁剪历史（`IntersectAABBWithLine`）、
     速度驱动的历史模糊/锐化、按速度差钳制混合权重、速度衰减。
   - `TemporalAntiAliasShader.sdsl`：明确声明 4 张输入 —— `Texture0=color, Texture1=depth, Texture2=velocity, Texture3=上一帧 color`。
   - **速度缓冲是一等公民**：`Stride.Rendering.Compositing.VelocityTargetSemantic`（公开 API）→ `ShaderClass = ShaderClassSource("VelocityOutput")`；
     `PostProcessingEffects.Draw()` 里 `outputValidator.Find<VelocityTargetSemantic>()` → `SetInput(6, ...)`；
     `PostProcessingEffects.RequiresVelocityBuffer => Antialiasing?.RequiresVelocityBuffer ?? false`。
   - **生产者也在**：`MeshVelocityRenderFeature : SubRenderFeature` —— "Output per-pixel motion vectors to a separate render target"，
     逐视图缓存 `PreviousViewProjection`、逐对象缓存上一帧 `World`，算出 `PreviousWorldViewProjection` 写进 per-draw 常量缓冲
     （`MeshVelocityKeys.PreviousWorldViewProjection`），并校验 `ComputeVelocityShader`（`ShaderClassSource("MeshVelocity")`）。
   - 另外 `BlendStateDescription.AlphaToCoverageEnable` 存在（默认 false），需要时草皮可以走 alpha-to-coverage。
   **结论**：草皮闪烁可以靠引擎自带 TAA 解决，不需要自己写速度缓冲。剩下的只是"接线 + 验证"：
   在 Graphics Compositor 里让渲染输出包含 velocity target（`VelocityOutput`），把 Post-processing 的 Antialiasing 类型选成
   TemporalAntiAlias（`Antialiasing` 是 `IScreenSpaceAntiAliasingEffect`，`[Display("Type","Antialiasing")]`，编辑器里可选具体实现）。
   **唯一未确认**：蒙皮网格（会动的角色/坐骑）是否带"上一帧骨骼变换"——`MeshVelocityRenderFeature` 用的是逐**对象**的 prev 矩阵，
   骨骼动画的形变速度可能要额外处理。静态植被与相机运动不受影响，所以对你"地图里有草皮"这个需求不构成阻碍。
2. **缺少烘焙光照**：Stride 没有 lightmapper。社区/论坛口径一致：可选方案只有**手动放置的 light probes**，
   另有实验性的 Voxel Cone Tracing GI（[论坛讨论](https://forums.stride3d.net/t/lightmapping-or-gi/973)、[manual 页](https://doc.stride3d.net/latest/en/manual/graphics/lights-and-shadows/voxel-cone-tracing-gi.html)）。
   而"传送门 + 静态小图"本来是最适合**烘焙 GI / 光照贴图**的场景 → Flax 的 lightmapping 在这一项优势明显。
   变通：light probes + 环境光 + SSAO 靠美术压，或外挂一个 lightmapper 工具链。

### 8.4 服务端（本仓库）因为"分图"新增的工作项

- **地图服务化**：一张地图 = 一个 hub 服务实例 / 一个场景 Actor（对应缺口清单的 P2-1 场景管理、P3-2 按地图路由）。
- **跨图接管**：`proto/hub_hub.proto` 的 `TakeOverUser` **目前只有生成类型、没有任何调用点**（差距分析 1.3）——
  传送光圈就是它的第一个真实用例。
- **可靠队列归属**：`entity_reliability_{userId}_client_mq` 以 userId 为 key，换图后 hub 归属变化，
  需要归属/租约标记（缺口清单 P0-7 的残留项）。
- **客户端切图**：建议加一条下行 `ChangeMap{map_id}`，触发与重连相同的 epoch/重同步流程，但**不重连 socket**
  （避免 gate 侧重建会话与可靠队列）。

### 8.5 结论

这个范围下 **Stride 的短板基本被砍掉，而"同构 .NET"的收益被放到最大**：地形/水/树草有 MIT 社区件接得上，
传送光圈是玩法 + 服务端问题，画质缺口只剩"**没有烘焙光照**"一项（草皮闪烁已被 TAA 解决，见 8.3）。
**Flax 的保留优势（烘焙 GI、体积雾、开箱地形）从"决定性"降级为"锦上添花"**（TAA 这一项已经不再独占）。

→ 建议按 Stride 走，验证顺序：
① TR.Stride 在 **Stride 4.4** 上能否编译运行（它最后提交早于 4.4 半年）；
② 在 Graphics Compositor 里接通 velocity 输出 + 把 Antialiasing 切成 TemporalAntiAlias，实测草皮闪烁与蒙皮角色的 TAA 质量；
③ 然后才是服务端的 `ChangeMap` / `TakeOverUser` 设计。

---

*本文所有关于本仓库的结论都标注了文件与行号；关于引擎的结论均给出官方来源链接或引擎源码路径，
星标/fork 数据取自 GitHub API（2026-10 抓取）。
仍需实测确认的项：C# 在 Flax Web 导出下的支持程度、Stride 的主线程 API 细节、
TR.Stride 在 Stride 4.4 上的兼容性（它最后提交于 2026-03，早于 4.4 发布）、
Stride 的 TAA 对**蒙皮网格**（角色骨骼形变）的速度处理是否完整。*
