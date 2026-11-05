## Ablekhan
abelkhan是一个轻量级的游戏服务器框架，基于c#开发，使用mongodb提供数据存储，支持c#和typescript前端，目标是提供一个稳定、高效、可扩展的服务器框架。  

## 架构
* 基于consul实现服务发现
* 使用protobuf进行序列化/反序列化  
&nbsp;
* dbproxy 数据库代理
* gate 网关服务器
* hub 微服务节点（含 `hub/Inventory` 抽象背包：数组存储、物品接口、邮件附件发放、BSON 存档，见 `docs/inventory.md`）
* shop 商城系统（氪金商品 / 玩家寄拍 / 限时拍卖，admin 后台编辑上架，见 `docs/shop.md`）
* leaderboard 通用排行榜（hub 侧 Redis ZSET：打榜保留最高分 / 区间拉榜 / 查名次 / 重置，见 `docs/leaderboard.md`）
* friend 好友系统（离线申请 / 临时占位 / 跨服确认 / 上限可扩展，见 `docs/friend.md`）
* chat 聊天系统（可配置频道按作用域跨 hub 群发 + 点对点私聊，见 `docs/chat.md`）
* guild 联盟/帮会系统（创建 / 邀请审批 / 三级职位 / 帮会频道 / 可编程帮会任务，见 `docs/guild.md`）
* quest 任务系统（可配置触发 / 进度 / 完成条件 + 任务链，见 `docs/quest.md`）
* admin 管理后台（dashboard：监控大盘/玩家查询/支付查询/GM面板/邮件发送/商城管理，见 abelkhan/admin/README.md）
* stride 引擎接入示例（Stride 4.4.1：客户端 SDK → 主线程桥 → GameSystem，见 `stride/README.md`）

## 测试

仓库自带的可运行测试（需要 Redis：不设环境变量时会自动起 `dependences/windows/redis`，找不到就打印 SKIP 并跳过）：

| 工程 | 覆盖 |
|---|---|
| `abelkhan/tests/ProtocolSmoke` | **所有 40 条客户端协议**，全部通过 `client` 模组的封装调用（`player.SendFriendRequest(...)` 等），走“客户端报文 → 进程内 gate → hub handler → Redis → 回包/推送 → 客户端分发”的真实链路；同时断言客户端通知事件 |
| `abelkhan/tests/InventorySmoke` | 背包 / 物品堆叠 / 邮件附件 |
| `abelkhan/tests/ShopSmoke` | 商城 / 寄拍 / 拍卖 / 幂等 |
| `abelkhan/tests/LeaderboardSmoke` | 排行榜 ZSET 语义 |
| `abelkhan/tests/FriendSmoke` | 好友申请 / 占位 / 跨服确认 / 上限 |
| `abelkhan/tests/ChatSmoke` | 频道历史 / 冷却 / pub-sub |
| `abelkhan/tests/GuildSmoke` | 帮会创建 / 审批 / 职位 / 转让 / 解散 / 任务 |
| `abelkhan/tests/QuestSmoke` | 任务三条件 / 任务链 |

```bash
dotnet run --project abelkhan/tests/ProtocolSmoke
dotnet run --project abelkhan/tests/FriendSmoke   # 其余同理
```

引擎接入示例单独一个解决方案（需要 Stride 4.4.1 包）：

```bash
dotnet build stride/Abelkhan.Stride.slnx
```

## License
[LGPL-2.1](https://github.com/qianqians/abelkhan/blob/master/LICENSE)
  
## 技术支持
theDarkforce@aliyun.com  

## QQ群
494405542
