# admin — 服务器管理后台（dashboard）

为 abelkhan 服务端引擎提供 Web 管理后台：监控大盘、玩家查询、支付查询、GM 面板、邮件发送、
商城管理（氪金商品上架、玩家寄拍 / 限时拍卖查看）。
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
| `MailGlobalMaxCount` | 全局邮件 list 最多保留多少封（默认 200，超出用 `LTRIM` 裁掉最旧的） |
| `ShopProductCollection` | 商城商品配置集合名（默认 `shop_product`） |
| `Shop` | 商城参数，必须与 hub 的 `HubConfig.Shop` 一致（`KeyPrefix` / 货币 / 税率 / 时长 / 寄拍上限），详见 `docs/shop.md` |

## 接口

| 方法 | 路径 | 说明 |
| --- | --- | --- |
| GET | `/api/overview` | 监控大盘：在线玩家/连接、总玩家、今昨日注册、今昨日充值 |
| GET | `/api/players?keyword=&page=&pageSize=` | 按 UserId/EntityId 查询玩家 |
| GET | `/api/players/{id}` | 玩家详情 |
| GET | `/api/payments?userId=&start=&end=&page=&pageSize=` | 查询支付 |
| GET | `/api/mail` | 邮件列表 |
| POST | `/api/mail/send` | 发送邮件 `{title, content, sender?, attachments?[{itemId,count}], playerIds?[], expireSeconds?, minLevel?, maxLevel?, minCreateDays?, maxCreateDays?, popUp?, validDays?}`，`playerIds` 空 = 全局邮件 |
| GET | `/api/gm/logs` | GM 命令日志 |
| POST | `/api/gm/command` | 下发 GM 命令 `{command, playerId?, args?}` |
| GET | `/api/shop/meta` | 商城元信息（默认货币 / 寄拍税 / 寄拍上限） |
| GET | `/api/shop/products?keyword=&status=&page=&pageSize=` | 商品配置列表（`status=published/unpublished`） |
| GET | `/api/shop/products/{id}` | 商品配置详情 |
| POST | `/api/shop/products` | 新建商品 |
| PUT | `/api/shop/products/{id}` | 修改商品；在售且无出价 / 无销量时自动同步线上 |
| DELETE | `/api/shop/products/{id}?force=` | 删除商品（已上架会先下架） |
| POST | `/api/shop/products/{id}/publish?force=` | 上架；`force=true` 覆盖已在售的一口价商品 |
| POST | `/api/shop/products/{id}/unpublish?force=` | 下架；拍卖有出价时需 `force=true` 退款 |
| GET | `/api/shop/listings?source=&status=&page=&pageSize=` | 线上官方商品 / 玩家寄拍（含已结束） |
| POST | `/api/shop/listings/{id}/cancel` | 管理员强制下架（拍卖退款 / 寄拍退物） |
| GET | `/api/shop/orders?page=&pageSize=` | 成交 / 结算流水 |
| GET | `/health` | 健康检查 |

## 数据约定（游戏侧接入点）

时间字段同时兼容 **Unix 毫秒（long）** 与 **BSON DateTime** 两种存储，无需额外处理。

- **在线数**：`gate` 每 3 秒把本机在线玩家/连接数上报到 Redis：
  - `admin_online_gate_{gateId}`（已登录玩家数）
  - `admin_online_connection_{gateId}`（连接数）

  admin 汇总所有匹配 key 得到全局在线数（自动支持多 gate 横向扩展）。

- **GM 命令**（按是否带 `PlayerId` 区分）：
  - **带 `PlayerId`**：写入 Redis list `admin_gm_queue_{PlayerId}`，由该玩家的游戏服（hub）`LPOP`
    消费，元素是 JSON：`{"command","playerId","args","createTime"}`。
  - **不带 `PlayerId`**：直接在 **admin 进程本地执行**。游戏侧用
    `AdminGmHandlers.Register("命令名", args => "结果")` 注册处理器（`admin` 项目是普通程序集，
    游戏服务 ProjectReference 它、在启动 admin 前注册即可）。默认内置一个 `echo` 示例命令。

  两种情况都会记入 MongoDB `gm` 集合留档（字段 `Command/PlayerId/Args/Result/CreateTime`）。

- **邮件**（区分全局 / 批量玩家）：
  - **全局邮件**（`playerIds` 为空）：所有玩家共用一份，`RPUSH admin_mail_global`（Redis list）。
    每个元素是一条 JSON：`{"mailId","title","content","sender","attachments":[{"itemId","count"}],"createTime","expireSeconds","validDays","popUp","minLevel","maxLevel","minCreateDays","maxCreateDays"}`。
    写入后用 `LTRIM` 保留最近 `MailGlobalMaxCount` 封（默认 200），防止 list 无限增长。
    玩家**请求邮件**时，游戏服用 `core.GlobalMailBox` 读取该 list（按刷新间隔缓存，避免每次请求都打 Redis），
    自动按以下条件过滤后只返回可领取的：
    - `validDays`：发布后 N 天内有效（0 = 永久）
    - `minLevel` / `maxLevel`：玩家等级范围（0 = 不限）
    - `minCreateDays` / `maxCreateDays`：建角天数范围（0 = 不限）
    - 已领取（`admin_mail_claimed_{playerId}` SET）

    `popUp` 表示是否「拍脸」，由游戏/客户端自行决定如何使用（本类原样透传）。
    玩家领取时调用 `MarkClaimed` 打标记，之后不会再返回。
  - **批量玩家**（`playerIds` 非空）：给每个玩家写 `admin_mail_queue_{playerId}`，由游戏服按玩家消费。

  两种情况都记入 MongoDB `mail` 集合留档（字段 `MailId/Title/Content/Sender/Attachments/PlayerIds/CreateTime/ExpireSeconds/ValidDays/PopUp/MinLevel/MaxLevel/MinCreateDays/MaxCreateDays`，
  `PlayerIds` 空数组表示全局）。

## 客户端请求领取邮件（协议）

新增协议（`proto/client.proto`，已生成到 `abelkhan/proto/Client.cs`）：

| 方法名 | 请求 | 响应 |
|---|---|---|
| `client_request_mail` | `ClientRequestMail { mail_id }` | `ClientRequestMailResult { repeated MailInfo mails }` |

- `mail_id` 为空：返回当前可领取的全局邮件列表（**不**标记领取）；
- `mail_id` 非空：领取指定邮件（校验条件 → 标记已领取 → 附件原子放入 `Player.Bag` → 返回该邮件）；
  背包满 / 附件配置错误时会回滚领取标记，玩家整理后可重新领取。

`MailInfo` 字段：`mail_id`、`title`、`content`、`sender`、`create_time`、`pop_up`、`attachments[{item_id,count}]`。

### 客户端

```csharp
// 拉取可领取列表
var list = await player.RequestMail();
// 领取指定邮件
var one = await player.RequestMail(mailId);
```

### hub 侧（框架已接线）

`Service.EchoQueryServiceEntity` 创建玩家实体时会自动注册该协议，处理逻辑在
`hub.Player.RegisterMailRequest`：

1. 用 `Service.GlobalMailBox` 读取全局邮件 list（按刷新频率缓存，避免每次请求都打 Redis）；
2. 用 `Service.GetMailCondition(userId)` 拿到等级 / 建角天数，过滤
   **已领取 / 有效天数 / 等级 / 建角天数**；
3. 返回 `ClientRequestMailResult`；
4. 领取指定邮件时，附件会通过 `Player.GrantMailAttachments` → `Bag.AddRange` 原子放入背包
   （见仓库根目录 `docs/inventory.md`），背包满或附件非法时自动回滚
   `admin_mail_claimed_{playerId}` 标记。

游戏侧只需要重写一个方法：

```csharp
public class MyService(ConcurrentDictionary<string, Client> clients) : Service(clients)
{
    protected override (int Level, int CreateDays)? GetMailCondition(string userId)
    {
        var p = GetPlayer(userId);          // 游戏自己的玩家对象
        if (p == null) return null;
        return (p.Level, p.CreateDays);
    }
}
```

`GlobalMailBox` 由 `hub.MainClass` 在启动时创建并注入到各 `Service`（共用 hub 的 Redis 连接），
游戏侧不需要自己 new。拉取时机由客户端控制（登录 / 间隔由客户端决定）。

> key 常量见 `abelkhan/consts/Consts.cs`（`AdminGmQueueKey`、`AdminMailQueueKey`、
> `AdminMailGlobalListKey`、`AdminMailClaimedKey`）。

## 商城管理（氪金商品 + 玩家寄拍）

「商城」标签页提供：

- **商品配置**：新建 / 编辑弹窗，支持一口价与限时拍卖两种售卖方式、奖励物品、库存 / 限购、
  拍卖起拍价 / 最小加价 / 时长 / 自动续拍、开售 / 停售时间、启用 / 排序；
- **在售商品 / 玩家寄拍**：实时查看官方 listing 与玩家寄拍（来源、卖家、当前价 / 最高出价人、
  结束时间、状态），可强制下架（拍卖最高出价会退款、寄拍物品会退回卖家）；
- **成交 / 结算记录**：一口价、拍卖成交、流拍等流水。

### 上架 / 下架的数据流

商品配置存 MongoDB `shop_product`；点击「上架」后由 admin 直接写入 Redis（与 hub 共用
`core.RedisHandle`），hub 读到后立即生效，不需要重启。

- **上架**：校验价格 / 奖励 / 时间窗口，生成 `shop:{shop}:listing:{id}` hash 并加入索引；
- **下架**：取消 listing；拍卖有出价时必须 `force=true`，会把最高出价写回出价人的待领取队列；
- **修改已上架商品**：一口价无销量 / 拍卖无出价时自动同步线上；否则只保存配置并提示先下架。

> 一口价售罄 / 拍卖结束 / 到期后，Mongo 里的商品配置仍可能是「已上架」状态；
> 在后台点「下架」再「上架」即可补货或开新一场（拍卖也可配置 `AutoRestart` 自动续拍）。
> 保存一次商品配置也会自动把已结束的线上商品标记为未上架。

完整的字段、协议、Lua 结算流程与崩溃窗口说明见仓库根目录 `docs/shop.md`。

## 依赖

admin 项目引用 `core`（复用 `RedisHandle` 的连接重连逻辑）、`consts` 与 `shop`（商城存储层），
不再新增第三方依赖。
