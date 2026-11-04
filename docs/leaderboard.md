# 通用排行榜（Redis ZSET）

排行榜是一个 hub 侧的通用组件：游戏按 `board_id` 区分多个榜，每个榜是 Redis 里一个独立的
**有序集合（ZSET）**，`member = userId`、`score = 分数`。客户端只发打榜 / 拉榜协议，排序 / 存储全部在 hub 侧完成；重置由 admin 后台操作。

核心语义：

- **打榜只保留最高分**：上报的分数低于历史最高分时不更新、名次不变（Redis `ZADD GT`）；
- **名次 1-based、降序**：分数高者第 1；不在榜返回 0；
- **拉榜按区间**：`[start, end)`，`start` 0-based、`end` 不包含；
- **榜单条目带展示信息**：`display_name` / `avatar` 由游戏提供一个批量解析钩子填充，未提供时为空。

> `score` 在 Redis 里是 `double`，注意超过 2^53 会丢精度；游戏分数建议控制在 2^53 以内。
> 分数相同时 Redis 按 `member`（userId）字典序排序，即「名次并列时先按 userId 字典序」，
> 目前不做「先到者优先」这类额外并列规则。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/leaderboard/LeaderboardModels.cs` | `LeaderboardEntry` / `LeaderboardDisplay` 模型 |
| `abelkhan/leaderboard/LeaderboardSettings.cs` | `KeyPrefix`、`MaxEntries`、`RangePageLimit` 运行参数 |
| `abelkhan/leaderboard/LeaderboardKeys.cs` | Redis key 生成（`leaderboard:{boardId}`） |
| `abelkhan/leaderboard/LeaderboardStore.cs` | 打榜 / 拉榜 / 查条目 / 重置（打榜用单 key Lua 原子执行） |
| `abelkhan/hub/Leaderboard/PlayerLeaderboard.cs` | 玩家实体上的 3 个排行榜协议处理 |
| `abelkhan/hub/Service.cs` / `MainClass.cs` | 注入 `LeaderboardStore`、展示信息钩子 |
| `abelkhan/admin/DataService.cs` / `MainClass.cs` | 重置排行榜 API（`/api/leaderboard/reset`） |
| `proto/client.proto` → `abelkhan/proto/Client.cs` | `ClientLeaderboard*` 协议 |
| `client/Player.cs` | C# 客户端封装（`SubmitLeaderboardScore` 等） |
| `abelkhan/tests/LeaderboardSmoke` | 基于真实 Redis 的端到端冒烟测试 |

---

## 2. 数据流

```
玩家（hub 进程）
  └─ client_leaderboard_submit ──► LeaderboardStore.SubmitAsync
        └─ Lua：ZADD GT + CH（只保留最高分）
           ├─ MaxEntries > 0 时裁剪到 top N
           └─ 返回 名次 + 当前榜上分数

玩家（任意 hub 进程，同一个 Redis）
  └─ client_leaderboard_range ──► ZREVRANGE + ZCARD
        └─ Service.ResolveLeaderboardDisplayAsync 批量补 display_name / avatar
  └─ client_leaderboard_rank  ──► ZREVRANK + ZSCORE（member_id 空 = 自己）

admin 后台
  └─ POST /api/leaderboard/reset ──► LeaderboardStore.ResetAsync ──► DEL 整个榜
```

多个 hub 进程共享同一个 Redis，打榜是单 key Lua 原子操作，天然并发安全，不会出现
「两个进程同时打榜把较低分覆盖掉较高分」的问题。

---

## 3. Redis 结构

| Key | 类型 | 说明 |
|---|---|---|
| `leaderboard:{boardId}` | zset | 一个榜，`member = userId`，`score = 分数`（降序） |

多游戏共用一个 Redis 时用 `LeaderboardSettings.KeyPrefix` 隔离（不能包含 `{` / `}`），
实际 key 为 `{prefix}leaderboard:{boardId}`。每个榜是单 key，不需要跨 key 的 hash tag。

---

## 4. 玩家协议

`proto/client.proto` 新增（方法名见 `consts/Consts.cs`）：

| 方法 | 请求 | 响应 | 说明 |
|---|---|---|---|
| `client_leaderboard_submit` | `ClientLeaderboardSubmit` | `ClientLeaderboardSubmitResult` | 打榜；返回名次（0 = 不在榜）与当前分数 |
| `client_leaderboard_range` | `ClientLeaderboardRange` | `ClientLeaderboardRangeResult` | 拉 `[start, end)` 区间 + 总人数 |
| `client_leaderboard_rank` | `ClientLeaderboardRank` | `ClientLeaderboardRankResult` | 查名次；`member_id` 空 = 查自己 |

C# 客户端封装在 `client/Player.cs`：

```csharp
var submit = await player.SubmitLeaderboardScore("level", 1200);
var range  = await player.RequestLeaderboardRange("level", start: 0, end: 20);
var mine   = await player.RequestLeaderboardRank("level");        // 查自己
var other  = await player.RequestLeaderboardRank("level", "u123"); // 查别人
```

---

## 5. 游戏接入

1. **配置**：在 `HubConfig.Leaderboard` 里设置 `KeyPrefix` / `MaxEntries` / `RangePageLimit`。
   hub 启动时自动 `new LeaderboardStore` 并注入到每个 `Service`。
2. **展示信息**（可选）：重写 `Service.ResolveLeaderboardDisplayAsync`，按 userId 批量返回
   `LeaderboardDisplay(Name, Avatar)`；不重写则条目展示字段为空。
3. **客户端**：直接调上面的协议（或 `client/Player.cs` 的封装）。

```csharp
protected internal override Task<IReadOnlyDictionary<string, LeaderboardDisplay>>
    ResolveLeaderboardDisplayAsync(IReadOnlyCollection<string> userIds)
{
    // 从档案 / 缓存批量查昵称头像
    return Task.FromResult<IReadOnlyDictionary<string, LeaderboardDisplay>>(
        userIds.ToDictionary(id => id, id => new LeaderboardDisplay("玩家" + id, "")));
}
```

---

## 6. 重置（admin 后台）

重置不走客户端协议，由 admin 后台调用：

```
POST /api/leaderboard/reset
Content-Type: application/json

{ "boardId": "level" }
```

- 成功返回 `{ "ok": true, "boardId": "level" }`；`boardId` 为空返回 400。
- admin 的 `AdminConfig.Leaderboard.KeyPrefix` 必须与 hub 的 `HubConfig.Leaderboard.KeyPrefix` 一致，
  否则清的是另一个 key。
- `LeaderboardStore.ResetAsync` 即 `DEL` 整个 ZSET（不做单个成员踢出）。

---

## 7. 边界与后续

- **单次拉榜上限**由 `RangePageLimit` 限制（默认 100），请求里过大的 `[start, end)` 会被夹到上限。
- **裁剪**：`MaxEntries > 0` 时打榜后只保留最高 N 名；掉出 top N 的成员名次返回 0。
- 暂未实现：累加计分（`ZINCRBY`）、按日/周/月周期自动重置、榜单 meta（标题 / 图标）、
  全局榜单索引、admin 榜单可视化（查看 / 编辑）。这些可后置扩展（`leaderboard` 工程可被 admin 复用）。
