# 聊天系统（可配置频道群发 + 点对点私聊）

两类消息：

1. **频道聊天**：频道在代码里声明（`ChatSettings.Channels`），每条频道自带一个**作用域**，
   消息群发给「该作用域内」的所有玩家：当前场景 / 帮会 / 门派 / 全区 / 全服。
2. **点对点私聊**：不限好友、仅在线；对方不在线直接报错，不存离线消息。

> 关于「以 group 为基准群发」：框架的 `hub.Group` 只是「本 hub 的在线客户端」，
> 真正决定群发范围的是 `(频道, 作用域 id)` 的成员集合。所以同一个频道
> 「帮会」在不同玩家的帮会 id 下是不同的 group，消息只发给同帮会的人。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/chat/ChatModels.cs` | `ChatScope` / `ChatChannel` / `ChatEntry` / `ChatDisplay` / `ChatCodes` |
| `abelkhan/chat/ChatSettings.cs` | 频道清单、`ZoneId` / `ServerId`、长度上限、冷却、内容清洗 |
| `abelkhan/chat/ChatKeys.cs` | 历史 / 冷却 / 广播通道的 key |
| `abelkhan/chat/ChatStore.cs` | 历史写入（Lua `LPUSH`+`LTRIM`）与读取、冷却（`SET NX PX`）、publish / subscribe |
| `abelkhan/core/RedisHandle.cs` | 新增 `Publish` / `Subscribe`，重连后自动重订阅 |
| `abelkhan/hub/Chat/ChatService.cs` | 本 hub 成员索引、频道发言、广播分发、私聊 |
| `abelkhan/hub/Chat/PlayerChat.cs` | 玩家实体上的 4 个聊天协议 + 游戏钩子 + `SetChatScope` |
| `abelkhan/hub/Service.cs` / `MainClass.cs` | 注入 `Chat`、订阅广播、下线清理索引 |
| `proto/chat.proto` → `abelkhan/proto/Chat.cs` | `ClientChat*` 协议与通知 |
| `proto/hub_hub.proto` | `ChatBroadcast`（跨 hub 广播载荷） |
| `abelkhan/tests/ChatSmoke` | 基于真实 Redis 的冒烟测试（26 项断言） |

---

## 2. 频道与作用域

```csharp
new ChatSettings
{
    ZoneId = "zone-1",      // 「全区」的作用域 id
    ServerId = "server-1",  // 「全服」的作用域 id
    MaxLength = 200,
    Channels = {
        new("world",  "世界", ChatScope.Zone,   3000, 100), // 冷却 3s，保留 100 条
        new("server", "全服", ChatScope.Server, 5000, 100),
        new("guild",  "帮会", ChatScope.Guild,  1000,  50),
        new("sect",   "门派", ChatScope.Sect,   1000,  50),
        new("scene",  "当前", ChatScope.Scene,  1000,   0), // 不存历史
    },
}
```

| 作用域 | 作用域 id 从哪来 |
|---|---|
| `Scene` | 游戏：`Player.GetChatScope(ChatScope.Scene)` → `scene:1001:3` |
| `Guild` | 游戏：`Player.GetChatScope(ChatScope.Guild)` → 帮会 id，没帮会返回 null |
| `Sect` | 游戏：`Player.GetChatScope(ChatScope.Sect)` → 门派 id |
| `Zone` | `ChatSettings.ZoneId` |
| `Server` | `ChatSettings.ServerId` |

作用于空的返回 null：表示玩家不在这个作用域里，发言会返回 `not_in_scope`（例如没有帮会就不能在帮会频道说话）。

---

## 3. 成员索引与群发

每个 hub 维护**本 hub 的成员索引**，避免每条消息都去问游戏「这个玩家属于哪个帮会」：

- 登录：`ChatService.OnOnlineAsync` 对每个作用域解析一次，写入索引
  `_members["{scope}:{scopeId}"] → {userId}`。
- 变化：切场景 / 加入或退出帮会 / 转门派时，游戏调
  `player.SetChatScope(ChatScope.Guild, "guild-9")`（`null` = 退出），索引增量更新；
  数据一次改多项时可以直接 `player.RefreshChatScopes()`。
- 下线：`OnOfflineAsync` 从索引移除。
- `Zone` / `Server` 不建索引——每个在线玩家都是成员，广播时分发时直接遍历本地在线玩家。

一条频道消息的完整路径：

```
client_chat_send {channel_id, content}
  → 校验频道 / 长度 / 内容过滤钩子 / 作用域 → 冷却（SET NX PX，跨 hub 生效）
  → 写历史（HistoryCount > 0）
  → PUBLISH chat:{prefix}:broadcast  ChatBroadcast{channel_id, scope_id, entry}
  → 每个 hub（含发送方）收到：
        Zone/Server → 推给本地全部在线玩家
        其它        → 只推 _members[scope:scopeId] 里的人
  → entity_{userId}_client_mq → gate → client_chat_message_notify
```

**为什么发送方也走订阅**：只有一条分发路径，少一个分叉。发送方的 `client_chat_send_result`
独立返回自己的消息 id / 时间。

**为什么用 pub/sub 而不是 `hub:{hubId}:mq` 扇出**：不用维护 hub 注册表，一条消息只写一次。
代价是瞬时断线的 hub 会丢这几条实时消息——聊天是实时语义，可以接受。

**昵称 / 头像在发送时冗余进消息**，接收方不需要为每条消息批量解析。

---

## 4. 点对点私聊

```
client_chat_private {to_user_id, content}
  → 校验（非空 / 长度 / 内容过滤 / 不能发给自己）→ 私聊冷却
  → 读 presence:{to_user_id}：不在线 → 返回 offline（不存离线消息）
  → 推 entity_{to_user_id}_client_mq → client_chat_private_notify
```

私聊不写历史、不进频道索引。`presence` 里带 `entityId`，所以对方在别的 hub 上也能收到。

---

## 5. Redis 结构

| Key / 通道 | 类型 | 说明 |
|---|---|---|
| `chat:{prefix}:history:{channelId}:{scopeId}` | list | 频道历史，最新在前，`LTRIM` 到 `HistoryCount` 条 |
| `chat:{prefix}:cd:{userId}:{channelId}` | string | 频道发言冷却 |
| `chat:{prefix}:cd:pm:{userId}` | string | 私聊冷却 |
| `chat:{prefix}:broadcast` | pub/sub | 跨 hub 频道广播 |

多游戏共用一个 Redis 时用 `ChatSettings.KeyPrefix` 隔离。

> pub/sub 的订阅记录在 `RedisHandle._subscriptions` 里，`Recover` 重连成功后会自动重新订阅
> ——否则 Redis 抖一次，频道广播就永久静默。

---

## 6. 协议

| 方法 | 请求 → 响应 | 说明 |
|---|---|---|
| `client_chat_channels` | `ClientChatChannels` → `ClientChatChannelsResult` | 频道列表（含冷却 / 保留条数） |
| `client_chat_send` | `ClientChatSend` → `ClientChatSendResult` | 频道发言 |
| `client_chat_history` | `ClientChatHistory` → `ClientChatHistoryResult` | 拉该频道最近 N 条（时间正序） |
| `client_chat_private` | `ClientChatPrivate` → `ClientChatPrivateResult` | 点对点私聊 |
| `client_chat_message_notify` | notify | 收到频道消息 |
| `client_chat_private_notify` | notify | 收到私聊 |

`ClientChatEntry`：`message_id` / `channel_id`（私聊为空）/ `sender_id` / `sender_name` /
`sender_avatar` / `content` / `send_time`（Unix 毫秒）。

错误文本（`ChatCodes`）：`unknown_channel` / `empty_content` / `too_long` / `cooldown` /
`not_in_scope` / `self_target` / `offline` / `blocked`。

---

## 7. 游戏接入

```csharp
// 1) 作用域：玩家在场景 / 帮会 / 门派里的 id（不在该作用域返回 null）
protected internal override string? GetChatScope(ChatScope scope) => scope switch
{
    ChatScope.Scene => SceneId,
    ChatScope.Guild => GuildId,     // null = 没有帮会
    ChatScope.Sect  => SectId,
    _ => null,                      // Zone / Server 由框架用 ChatSettings 的常量补
};

// 2) 内容过滤：敏感词 / 禁言 / 拉黑。返回空表示拒发。
protected internal override string? FilterChatContent(string channelId, string content)
    => IsMuted(channelId) ? null : SensitiveWordFilter.Replace(content);
```

展示信息（昵称 / 头像）走 `Service.ResolveChatDisplayAsync`：

```csharp
protected internal override Task<IReadOnlyDictionary<string, ChatDisplay>>
    ResolveChatDisplayAsync(IReadOnlyCollection<string> userIds) { ... }
```

作用域变化时主动通知框架：

```csharp
player.SetChatScope(ChatScope.Scene, newSceneInstanceId);      // 切场景
player.SetChatScope(ChatScope.Guild, guildId);                 // 入帮（null = 退帮）
player.RefreshChatScopes();                                    // 一次改多项时
```

---

## 8. 客户端（C# SDK）

`client/Player` 已经封装好（见 `client/Player.Chat.cs`）：

```csharp
var channels = await player.RequestChatChannels();
var sent     = await player.SendChatMessage("guild", "大家好");
var history  = await player.RequestChatHistory("guild", count: 20);
var pm       = await player.SendPrivateMessage("u100", "在吗");

// 服务端推送（框架自动注册）
player.ChatMessage += n => { /* 频道消息（含自己发的） */ };
player.ChatPrivate += n => { /* 私聊消息 */ };
```

## 9. 边界与后续

- 广播依赖 pub/sub：hub 瞬时断线期间的实时消息会丢（不进历史）；历史消息不受影响。
- 频道历史只保留最近 N 条，不落库；要审计 / 举报需要自己接 `MongoDB`（未实现）。
- 未实现：系统公告只读频道、私聊历史与会话未读数、拉黑名单、富文本 / 表情 / 语音 / 图片、
  跨服聊天（不同 Redis 前缀之间）。
