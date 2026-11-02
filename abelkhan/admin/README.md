# admin — 服务器管理后台（dashboard）

为 abelkhan 服务端引擎提供 Web 管理后台：监控大盘、玩家查询、支付查询、GM 面板、邮件发送。
前端是纯静态页面（无构建步骤），由 `Microsoft.AspNetCore` 托管在 `wwwroot` 下。

## 运行

```bash
dotnet run --project abelkhan/admin -- path/to/config.json
```

浏览器打开 `http://<Ip>:<Port>`（默认 `http://127.0.0.1:8080`）。

## 配置字段

| 字段 | 说明 |
| --- | --- |
| `Ip` / `Port` | 管理后台监听地址 / 端口 |
| `RedisUrl` / `RedisPwd` | 与 gate/hub 相同的 Redis |
| `MongoUrl` / `MongoDb` | MongoDB 连接串与库名 |
| `PlayerCollection` | 玩家集合名（默认 `player`） |
| `PaymentCollection` | 支付集合名（默认 `payment`） |
| `MailCollection` | 邮件集合名（默认 `mail`） |
| `GmCollection` | GM 命令日志集合名（默认 `gm`） |
| `PlayerRegisterField` | 玩家文档里的注册时间字段（默认 `RegisterTime`） |
| `PaymentTimeField` | 支付文档里的支付时间字段（默认 `PayTime`） |
| `PaymentAmountField` | 支付文档里的金额字段（默认 `Amount`） |
| `AdminToken` | 可选。非空时 `/api/*` 需要 `Authorization: Bearer <token>` |

## 接口

| 方法 | 路径 | 说明 |
| --- | --- | --- |
| GET | `/api/overview` | 监控大盘：在线玩家/连接、总玩家、今昨日注册、今昨日充值 |
| GET | `/api/players?keyword=&page=&pageSize=` | 按 UserId/EntityId 查询玩家 |
| GET | `/api/players/{id}` | 玩家详情 |
| GET | `/api/payments?userId=&start=&end=&page=&pageSize=` | 查询支付 |
| GET | `/api/mail` | 邮件列表 |
| POST | `/api/mail/send` | 发送邮件 `{userId?, title, content, expireSeconds?}` |
| GET | `/api/gm/logs` | GM 命令日志 |
| POST | `/api/gm/command` | 下发 GM 命令 `{command, target?, args?}` |
| GET | `/health` | 健康检查 |

## 数据约定（游戏侧接入点）

时间字段同时兼容 **Unix 毫秒（long）** 与 **BSON DateTime** 两种存储，无需额外处理。

- **在线数**：`gate` 每 3 秒把本机在线玩家/连接数上报到 Redis：
  - `admin_online_gate_{gateId}`（已登录玩家数）
  - `admin_online_connection_{gateId}`（连接数）

  admin 汇总所有匹配 key 得到全局在线数（自动支持多 gate 横向扩展）。

- **GM 命令**：admin 把命令写入 Redis list `admin_gm_queue`（同时记入 MongoDB `gm` 集合留档）。
  游戏服 GM 模块按 `LPOP` 消费，元素是 JSON：`{"command","target","args","createTime"}`。

- **邮件**：admin 把邮件写入 Redis list `admin_mail_queue_{userId}`，全体邮件写入
  `admin_mail_queue_broadcast`（同时记入 MongoDB `mail` 集合留档）。游戏服邮件模块按 `LPOP`
  消费，元素是 JSON：`{"userId","title","content","createTime","expireSeconds"}`。

> 队列 key 常量见 `abelkhan/consts/Consts.cs`（`AdminGmQueueKey`、`AdminMailQueueKey`）。

## 依赖

admin 项目引用 `core`（复用 `RedisHandle` 的连接重连逻辑）与 `consts`，不再新增第三方依赖。
