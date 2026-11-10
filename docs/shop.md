# 商城系统（氪金商城 + 玩家寄拍 + 限时拍卖）

商城同时支持两类商品：

1. **氪金商城**：管理员在 admin 后台编辑商品（名称、图标、奖励、价格、货币、库存、限购……），
   点击「上架」后写入 Redis，玩家在游戏内即可看到并购买；
2. **玩家寄拍**：玩家把背包里的物品挂到商城，选择一口价或限时拍卖；成交后货款（扣税）
   进入卖家待领取队列，流拍 / 取消则原物退回；
3. 官方商品和玩家寄拍**都支持限时拍卖**，并共用同一套出价、保证金、结算逻辑。

> 货币就是背包里的普通物品（默认 `diamond`，寄拍默认 `gold`），没有单独的充值货币字段。
> 这样充值、邮件附件、商城购买、拍卖结算都复用同一套背包 / 存档逻辑。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/shop/ShopModels.cs` | `ShopProduct` / `ShopListing` / `ShopDelivery` / `ShopOrder` 等模型 |
| `abelkhan/shop/ShopSettings.cs` | 货币、税率、寄拍时长、寄拍上限等运行参数 |
| `abelkhan/shop/ShopKeys.cs` | 所有 Redis key 的生成（统一带 `{shop}` hash tag） |
| `abelkhan/shop/ShopStore.cs` | 商品上架 / 查询 / 购买 / 出价 / 取消 / 到期结算（Lua 原子操作） |
| `abelkhan/hub/Shop/ShopTrader.cs` | 玩家侧交易编排：背包扣款、调用 ShopStore、失败退款、领取结算 |
| `abelkhan/hub/Shop/PlayerShop.cs` | 玩家实体上的 6 个商城协议处理 |
| `abelkhan/hub/Service.cs` / `MainClass.cs` | 注入 `ShopStore`、注册协议、定时扫描到期商品 |
| `abelkhan/admin/DataService.cs` | 商品 CRUD（MongoDB）+ 上架 / 下架（Redis）+ 线上 listing / 流水查询 |
| `abelkhan/admin/MainClass.cs` | `/api/shop/*` 管理接口 |
| `abelkhan/admin/wwwroot/*` | 「商城」管理页面（商品编辑、在售 / 寄拍、成交记录） |
| `proto/shop.proto` → `abelkhan/proto/Shop.cs` | `ClientShop*` 协议 |
| `abelkhan/tests/ShopSmoke` | 基于真实 Redis 的端到端冒烟测试 |

---

## 2. 数据流

```
admin 后台
  └─ 商品配置（MongoDB shop_product）
        └─ 点击「上架」──► ShopStore.PublishAsync ──► Redis listing（status=active）
                                                          │
玩家 A（hub）                                            │
  └─ client_shop_list ───────────────────────────────────┘
  └─ client_shop_buy / client_shop_bid
        ├─ 先扣自己背包里的货币
        ├─ ShopStore 的 Lua 原子抢占 listing
        │     ├─ 成功：买家 / 卖家生成待领取结算、记成交流水
        │     └─ 失败：立即把货币退回背包（放不下则进待领取）
        └─ 自动领取自己的待领取物品

玩家 B（卖家）
  └─ client_shop_consign（物品先从背包 escrow 出去）
  └─ 成交 / 流拍 / 取消后，货款或原物进入待领取
  └─ 下次进商城 / 主动 client_shop_claim 时进背包
```

### 状态

`active` → `sold`（一口价售罄 / 拍卖成交）、`expired`（到期无人要）、`cancelled`（取消 / 管理员强制下架）。

---

## 3. Redis 结构

所有 key 都带 `{shop}` hash tag，Lua 脚本可以安全地同时读写「商品 + 待领取 + 计数器」。
多游戏共用一个 Redis 时用 `ShopSettings.KeyPrefix` 隔离（不能包含 `{` / `}`）。

| Key | 类型 | 说明 |
|---|---|---|
| `shop:{shop}:listing:{listingId}` | hash | 一条官方商品 / 玩家寄拍的全部字段 |
| `shop:{shop}:listings` | set | 所有 listing id（admin 列表用，含已结束） |
| `shop:{shop}:active` | zset | `score = endTime`，hub 定时按分数扫描到期商品 |
| `shop:{shop}:seller:{playerId}` | set | 卖家在售寄拍 id（用于「最多同时寄拍 N 件」） |
| `shop:{shop}:purchased:{listingId}` | hash | `playerId -> 已购买数量`（每人限购） |
| `shop:{shop}:delivery:{playerId}` | hash | `deliveryId -> JSON`：待领取的物品 / 货币 |
| `shop:{shop}:job:{orderId}` | string | 购买 / 出价的幂等标记（24h），Redis 超时重试不会重复发货 |
| `shop:{shop}:orders` | list | 成交 / 结算流水，最多保留 5000 条 |

listing hash 里 `items` 字段是 camelCase JSON 数组：`[{"itemId":"potion","count":2}]`，
Lua 用 `cjson.decode` 读取，C# 用 Newtonsoft（大小写不敏感）反序列化。

### 待领取（delivery）

以下场景都会往 `shop:{shop}:delivery:{playerId}` 写一条记录：

- 拍卖中标 → 物品给中标人（`auction_won`）；
- 玩家寄拍成交 → 扣税后的货款给卖家（`sale_proceeds` / `auction_proceeds`）；
- 被超价 → 上一手保证金退回（`outbid_refund`）；
- 流拍 / 取消 → 原物退回卖家（`listing_expired` / `listing_cancelled`）；
- 管理员强制下架拍卖 → 最高出价退款（`listing_cancelled_refund`）；
- 背包满导致退款失败时的兜底（`shop_refund`）。

玩家请求 `client_shop_list` 时会**先自动尝试领取**；背包满则保留，并返回
`pending_deliveries` 数量，客户端整理背包后调用 `client_shop_claim` 再领。

---

## 4. 玩家协议

`proto/shop.proto` 新增（方法名见 `consts/Consts.cs`）：

| 方法 | 请求 | 响应 | 说明 |
|---|---|---|---|
| `client_shop_list` | `ClientShopList` | `ClientShopListResult` | 分页浏览；`mine=true` 只看自己寄拍；会先尝试领取待领取 |
| `client_shop_buy` | `ClientShopBuy` | `ClientShopBuyResult` | 一口价购买 `count` 份；返回真正进背包的物品 |
| `client_shop_bid` | `ClientShopBid` | `ClientShopBidResult` | 拍卖出价（总额，全额托管） |
| `client_shop_consign` | `ClientShopConsign` | `ClientShopConsignResult` | 寄拍：一口价 / 限时拍卖 |
| `client_shop_cancel` | `ClientShopCancel` | `ClientShopCancelResult` | 取消寄拍（拍卖有出价时不能取消） |
| `client_shop_claim` | `ClientShopClaim` | `ClientShopClaimResult` | 领取待领取结算 |

C# 客户端封装在 `client/Player.cs`：

```csharp
var list = await player.RequestShopList(category: "hot");
var buy  = await player.BuyShopItem(listingId, count: 2);
var bid  = await player.BidShopItem(listingId, price: 1200);
var consign = await player.ConsignShopItem(
    "potion", 3, ShopSaleMode.Auction,
    startPrice: 100, minIncrement: 10, durationSeconds: 3600, title: "便宜药水");
var claim = await player.ClaimShopDeliveries();
```

### 购买（一口价）

1. 校验 listing 在售、未过期、不是自己的寄拍、库存 / 限购；
2. 背包 `Consume(currency, price * count)`（本地原子）；
3. Lua 抢占库存：扣库存、写买家物品 delivery、写卖家货款 delivery；
4. 抢占失败 → 立即退款；成功 → 自动领取买家的 delivery。

### 出价（拍卖）

1. 校验拍卖在售、未结束、不是自己的寄拍、`price >= 最低出价`
   （首口 = 起拍价，之后 = 当前价 + 最小加价）；
2. 背包先扣**全额** `price` 作为保证金；
3. Lua 记录新最高价，并把上一手保证金写进上一手的待领取（自己顶价时也是退给自己）；
4. 失败立即退款。

> 因此「自己超过自己」时，客户端会看到先扣全额、随后待领取多出一笔旧保证金；
> 下一次进商城或 `client_shop_claim` 时会自动补回背包。

### 寄拍

- 物品先从背包扣除（escrow），listing 创建失败会立即退回；
- 一口价：`price > 0`，时长到期未卖出则原物退回；
- 拍卖：`start_price > 0`、`min_increment >= 1`，到期最高价成交；
- 每个玩家最多同时寄拍 `ShopSettings.MaxListingsPerPlayer` 件（默认 8）；
- 拍卖一旦有出价，卖家不能取消；管理员可以强制下架并退款。

### 到期结算

`hub.MainClass` 默认每 5 秒扫描一次 `shop:{shop}:active`（`ShopSettleIntervalMs` / `ShopSettleBatchSize`），
对到期 listing 执行 Lua 结算：

- 有最高出价 → `sold`：物品给中标人、货款扣税给卖家（官方商品不给货款）；
- 无出价 → `expired`：玩家寄拍原物退回卖家；
- 官方拍卖 `AutoRestart=true` → 结算后按原配置自动开下一场，直到商品 `EndTime` 或管理员下架。

多个 hub 同时扫描是安全的：Lua 里 `status` 只会从 `active` 成功切换一次。

---

## 5. 管理端

### 配置

admin 与 hub 的 `Shop` 配置必须一致（尤其 `KeyPrefix` 和货币），否则后台写入的 key 游戏服读不到。

```json
{
  "ShopProductCollection": "shop_product",
  "Shop": {
    "KeyPrefix": "",
    "CurrencyItemId": "diamond",
    "MarketCurrencyItemId": "gold",
    "SaleTaxPercent": 5,
    "MaxListingsPerPlayer": 8,
    "MinListingDurationSeconds": 300,
    "MaxListingDurationSeconds": 604800,
    "DefaultListingDurationSeconds": 86400,
    "DefaultAuctionDurationSeconds": 86400,
    "MaxBuyCount": 99
  }
}
```

hub 的 `HubConfig` 同样有 `Shop`（`hub` 启动时创建 `ShopStore` 并注入所有 `Service`），
另有 `ShopSettleIntervalMs`、`ShopSettleBatchSize` 控制结算频率：

```jsonc
{
  // HubConfig 其他字段……
  "ShopSettleIntervalMs": 5000,
  "ShopSettleBatchSize": 200,
  "Shop": {
    "KeyPrefix": "",              // 必须和 admin 完全一致
    "CurrencyItemId": "diamond",
    "MarketCurrencyItemId": "gold",
    "SaleTaxPercent": 5,
    "MaxListingsPerPlayer": 8,
    "MinListingDurationSeconds": 300,
    "MaxListingDurationSeconds": 604800,
    "DefaultListingDurationSeconds": 86400,
    "DefaultAuctionDurationSeconds": 86400,
    "MaxBuyCount": 99
  }
}
```

### 接口

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/api/shop/meta` | 货币 / 税率 / 寄拍上限等元信息 |
| GET | `/api/shop/products?keyword=&status=&page=&pageSize=` | 商品列表（`status=published/unpublished`） |
| GET | `/api/shop/products/{id}` | 商品详情 |
| POST | `/api/shop/products` | 新建商品 |
| PUT | `/api/shop/products/{id}` | 修改商品；在售且无销量 / 无出价时自动同步线上 |
| DELETE | `/api/shop/products/{id}?force=` | 删除商品（已上架会先下架） |
| POST | `/api/shop/products/{id}/publish?force=` | 上架；`force=true` 覆盖已在售的一口价商品 |
| POST | `/api/shop/products/{id}/unpublish?force=` | 下架；拍卖有出价时需 `force=true` 退款 |
| GET | `/api/shop/listings?source=&status=&page=&pageSize=` | 线上商品 / 玩家寄拍（含已结束） |
| POST | `/api/shop/listings/{id}/cancel` | 管理员强制下架（拍卖退款 / 寄拍退物） |
| GET | `/api/shop/orders?page=&pageSize=` | 成交 / 结算流水 |

商品入参（`ShopProductInput`）：

```jsonc
{
  "productId": "vip_weekly",        // 可空，空则自动生成
  "name": "每周钻石礼包",
  "description": "……",
  "icon": "💎",
  "category": "hot",
  "saleMode": 1,                    // 1 = 一口价，2 = 限时拍卖
  "currencyItemId": "diamond",       // 空则用服务端默认
  "price": 100,                      // 一口价
  "startPrice": 100,                 // 拍卖起拍价
  "minIncrement": 10,                // 拍卖最小加价
  "auctionDurationSeconds": 3600,    // 0 = 服务端默认
  "autoRestart": false,              // 官方拍卖结束后自动续拍
  "stock": -1,                       // 一口价库存，-1 = 不限
  "perUserLimit": 0,                 // 每人限购，0 = 不限
  "enabled": true,                   // 禁用后不能上架
  "sort": 0,                         // 排序权重
  "startTime": 0,                    // Unix 毫秒，0 = 立即
  "endTime": 0,                      // Unix 毫秒，0 = 不限
  "rewards": [ { "itemId": "diamond", "count": 100 } ]
}
```

### 管理页面

admin 的「商城」标签页包含三块：

1. **商品配置**：新建 / 编辑（弹窗）、上架、下架、删除；
2. **在售商品 / 玩家寄拍**：实时查看官方 listing 与玩家寄拍，可强制下架；
3. **成交 / 结算记录**：购买、拍卖成交、流拍等流水。

修改已上架商品时：

- 一口价且 `SoldCount == 0` → 直接覆盖线上价格 / 奖励；
- 拍卖且没有出价 → 取消旧场次并立即开新场次；
- 已有销量 / 出价 → 只保存配置并提示先下架，避免吞掉保证金。

---

## 6. 一致性与崩溃窗口

关键状态变更都在 Redis Lua 内原子完成，包括：

- 扣库存 / 每人限购计数 / 生成买家与卖家 delivery；
- 出价 + 上一手退款 delivery；
- 取消 + 退款 / 退物；
- 到期结算 + 中标物品 + 卖家货款 + 自动续拍判定。

购买 / 出价会把 `orderId` 写进 `shop:{shop}:job:{orderId}`（`SET NX EX 86400`）：
框架在 Redis 超时后重试同一个 Lua 时不会重复扣库存或重复发货。Lua 里还会再次校验
价格 / 货币与玩家扣款时看到的一致（`price_changed` / `currency_changed`），
防止管理员在购买瞬间改价导致少付或多付。

但「背包」在各玩家自己的 hub 进程里，和 Redis 无法放进同一个事务。因此仍有两个窄窗口：

1. **购买 / 出价**：先扣背包、再调 Lua。Lua 失败会立即退款；如果进程在「扣款成功、
   Lua 未执行」之间崩溃，这笔货币会丢（没有跨系统事务）。
2. **领取结算**：先把 delivery 放进背包，再从 Redis 删除。如果进程在「进背包成功、
   删除未执行」之间崩溃，重试时可能重复发放。这里选择 at-least-once（宁可重复不丢），
   与邮件系统的 at-most-once 取舍相反。

寄拍同理：物品先从背包扣除，再创建 listing；创建失败会退款，崩溃窗口内可能丢物品。

对经济安全要求极高的项目，应把「背包存档 + 扣款 / 发放」放进同一个 Mongo 事务或
outbox，而不是依赖 Redis + 本地背包的补偿。当前实现适合中小规模、可接受秒级补偿的场景。

### 其他注意

- 所有 Lua 同时访问的 key 都带 `{shop}`，单机 Redis 与 Redis Cluster 都能工作
  （Cluster 下需保证 `KeyPrefix` 不含 `{}`）；
- 结算扫描是**懒**的：即使 hub 短暂停机，重启后仍会扫描到期的 zset 并结算；
- 官方拍卖自动续拍若在「结算成功、下一场未创建」之间崩溃，会停在这一场（不重复扣款），
  管理员重新上架即可；
- `shop:orders` 只保留最近 5000 条，长期归档建议另接 Mongo / 数仓；
- 已结束的 listing（sold / expired / cancelled）会保留在 `shop:listings` 里供后台追溯；
  长期运行可定期清理超过 N 天的 listing hash 和索引成员；
- 列表查询会遍历 `shop:listings` 集合后过滤，适合几千条以内的商品量；更大规模建议按分类
  建二级索引或分页缓存。

---

## 7. 测试

`abelkhan/tests/ShopSmoke` 是**真实 Redis** 的端到端测试：

```bash
dotnet run --project abelkhan/tests/ShopSmoke
```

- Windows 下默认自动启动仓库自带的 `dependences/windows/redis/redis-server.exe`（随机端口、独立目录）；
- 也可以设置 `SHOP_TEST_REDIS=127.0.0.1:6379` 使用已有 Redis（Linux 上必须这样，只用随机 key 前缀，不 flush）；
- 没有 Redis 时打印 `SKIP` 并返回 0。

覆盖：官方一口价（限购 / 库存 / 售罄）、官方拍卖（出价 / 超价退款 / 结算 / 重复结算幂等）、
玩家寄拍一口价（扣税货款）、拍卖取消 / 流拍退回 / 有出价不能取消 / 管理员强制下架退款、
背包满时 delivery 保留、寄拍名额释放、官方拍卖自动续拍。

---

## 8. 扩展建议

- **唯一属性 / 强化等级**：当前 listing 只存 `itemId + count`（和框架的 `ItemStack` 一致）。
  装备实例属性需要扩展 `ShopItemGrant` 和 `IItem` 的序列化；
- **多货币 / 多分区**：`currencyItemId` 已按 listing 存储，可扩展为每个分类一套货币；
- **一口价议价 / 求购**：可在 `ShopStore` 增加新的 Lua 状态机，复用 delivery / 流水；
- **反作弊**：建议给 `client_shop_buy/bid` 增加频率限制（可复用 gate / actor 的限流设施）。
