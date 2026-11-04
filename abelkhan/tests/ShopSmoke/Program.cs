using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using core;
using engine;
using hub.inventory;
using hub.shop;
using shop;
using ShopGrant = shop.ShopItemGrant;

// 商城冒烟测试：需要 Redis。
//   - 设置 SHOP_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。

var external = Environment.GetEnvironmentVariable("SHOP_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 SHOP_TEST_REDIS 或放入 dependences/windows/redis）");
    return 0;
}

Process? redisProcess = null;
string? tempDir = null;
var port = 0;
if (!string.IsNullOrWhiteSpace(external))
{
    var parts = external.Split(':', 2);
    port = int.Parse(parts[1]);
}
else
{
    port = FreePort();
    // 用输出目录而不是 %TEMP%：部分受限环境对系统临时目录没有写权限
    tempDir = Path.Combine(AppContext.BaseDirectory, "shop-smoke-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(tempDir);
    redisProcess = Process.Start(new ProcessStartInfo
    {
        FileName = redisExe!,
        Arguments = $"--port {port} --bind 127.0.0.1 --save \"\" --appendonly no --dir \"{tempDir}\" --logfile \"{Path.Combine(tempDir, "redis.log")}\"",
        UseShellExecute = false,
        CreateNoWindow = true,
    });
    if (redisProcess == null || !await WaitPort(port, 8000))
    {
        Console.WriteLine("SKIP: Redis 启动失败");
        return 0;
    }
}

try
{
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var settings = new ShopSettings
    {
        KeyPrefix = "smoke-" + Guid.NewGuid().ToString("N") + ":",
        CurrencyItemId = "diamond",
        MarketCurrencyItemId = "gold",
        SaleTaxPercent = 10,
        MaxListingsPerPlayer = 4,
        MinListingDurationSeconds = 1,
        MaxListingDurationSeconds = 3600,
        DefaultListingDurationSeconds = 60,
        DefaultAuctionDurationSeconds = 60,
    };
    var store = new ShopStore(redis, settings);

    var passed = 0;
    void Check(bool ok, string name)
    {
        if (!ok)
        {
            throw new Exception("FAILED: " + name);
        }

        passed++;
        Console.WriteLine("  ok  " + name);
    }

    void CheckErr<T>(Result<T, string> result, string name)
    {
        if (result.IsOk)
        {
            throw new Exception("FAILED(expect error): " + name);
        }

        passed++;
        Console.WriteLine("  ok  " + name + "  [" + result.Error + "]");
    }

    Check(!store.Keys.Listings.Contains("{{") && store.Keys.Listings.Contains("{shop}") &&
          !store.Keys.Delivery("p").Contains("{{") && store.Keys.Delivery("p").Contains("{shop}"),
        "Redis key 使用统一 {shop} hash tag");

    var catalog = new ItemCatalog();
    catalog.Register(new ItemDefinition("diamond", 0));
    catalog.Register(new ItemDefinition("gold", 0));
    catalog.Register(new ItemDefinition("potion", 99));
    catalog.Register(new ItemDefinition("sword", 1, ItemType.Equipment));
    var trader = new ShopTrader(store, catalog.Create);

    IBag NewBag(int capacity, params (string ItemId, long Count)[] items)
    {
        var bag = new ArrayBag(capacity, catalog);
        foreach (var (itemId, count) in items)
        {
            var added = bag.Add(itemId, count);
            if (added.Failed)
            {
                throw new Exception("初始化背包失败: " + added.Message);
            }
        }

        return bag;
    }

    static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    async Task ExpireNow(string listingId, long now)
    {
        var past = now - 1000;
        await redis.Execute("test.expire", db => db.HashSetAsync(store.Keys.Listing(listingId), "endTime", past));
        await redis.Execute("test.expire.zadd", db => db.SortedSetAddAsync(store.Keys.Active, listingId, past));
    }

    Console.WriteLine("== 1. 官方一口价：限购 / 库存 / 发放 ==");
    var fixedProduct = new ShopProduct
    {
        ProductId = "p_fixed",
        Name = "新手礼包",
        SaleMode = ListingMode.Fixed,
        CurrencyItemId = "diamond",
        Price = 100,
        Stock = 5,
        PerUserLimit = 2,
        Items = new List<ShopGrant> { new("sword", 1) },
    };
    var publish = await store.PublishAsync(fixedProduct, Now());
    Check(publish.Ok, "官方商品上架");

    var alice = NewBag(20, ("diamond", 10_000), ("gold", 1_000));
    var bob = NewBag(20, ("diamond", 10_000));
    var carol = NewBag(20, ("diamond", 10_000));

    var listed = await trader.ListAsync("alice", alice, new ClientShopList { Page = 1, PageSize = 20 });
    Check(listed.Listings.Count == 1 && listed.Listings[0].ListingId == "p_fixed", "玩家能看到上架商品");

    var buy2 = await trader.BuyAsync("alice", alice, "p_fixed", 2);
    Check(buy2.IsOk && buy2.Value.Listing.Stock == 3, "一次买 2 份，库存 5 -> 3");
    Check(alice.GetItemCount("sword") == 2, "购买奖励进入背包");
    Check(alice.GetItemCount("diamond") == 9_800, "扣除 2 份货币");

    var overLimit = await trader.BuyAsync("alice", alice, "p_fixed", 1);
    CheckErr(overLimit, "超过每人限购被拒绝");
    Check(alice.GetItemCount("diamond") == 9_800, "限购失败后货币已退回");

    var buyBob = await trader.BuyAsync("bob", bob, "p_fixed", 2);
    Check(buyBob.IsOk && buyBob.Value.Listing.Stock == 1, "bob 再买 2 份，库存剩 1");
    var buyCarol = await trader.BuyAsync("carol", carol, "p_fixed", 1);
    Check(buyCarol.IsOk && buyCarol.Value.Listing.Stock == 0, "买空剩余库存");
    Check(buyCarol.Value.Listing.Status == ListingStatuses.Sold, "库存为 0 时自动下架");
    CheckErr(await trader.BuyAsync("carol", carol, "p_fixed", 1), "售罄后无法购买");

    Console.WriteLine("== 2. 官方限时拍卖：出价 / 超价退款 / 结算 ==");
    var auctionProduct = new ShopProduct
    {
        ProductId = "p_auction",
        Name = "限时神装",
        SaleMode = ListingMode.Auction,
        CurrencyItemId = "diamond",
        StartPrice = 100,
        MinIncrement = 10,
        AuctionDurationSeconds = 1,
        Items = new List<ShopGrant> { new("potion", 2) },
    };
    var auctionPublish = await store.PublishAsync(auctionProduct, Now());
    Check(auctionPublish.Ok && auctionPublish.Listing != null, "官方拍卖上架");
    var auctionId = auctionPublish.Listing!.ListingId;

    var bidAlice = await trader.BidAsync("alice", alice, auctionId, 100);
    Check(bidAlice.IsOk, "alice 出价 100");
    var bidBob = await trader.BidAsync("bob", bob, auctionId, 120);
    Check(bidBob.IsOk && bidBob.Value.Listing.CurrentPrice == 120, "bob 出价 120 成为最高价");

    var aliceRefund = await trader.ClaimAsync("alice", alice);
    Check(aliceRefund.Granted.Any(g => g.ItemId == "diamond" && g.Count == 100), "alice 被超价后领回 100 保证金");
    CheckErr(await trader.BidAsync("carol", carol, auctionId, 125), "低于最低加价被拒绝");

    var auctionEnd = Now() + 2000;
    await ExpireNow(auctionId, auctionEnd);
    var settlements = await store.SettleExpiredAsync(100, auctionEnd);
    Check(settlements.Count == 1 && settlements[0].Type == "sold", "拍卖到期结算为成交");
    Check(settlements[0].WinnerId == "bob" && settlements[0].Price == 120, "成交人是 bob，价格 120");

    var bobClaim = await trader.ClaimAsync("bob", bob);
    Check(bobClaim.Granted.Any(g => g.ItemId == "potion" && g.Count == 2), "中标物品进入 bob 背包");
    var settledAgain = await store.SettleExpiredAsync(100, auctionEnd + 1000);
    Check(settledAgain.Count == 0, "重复扫描不会重复结算");

    Console.WriteLine("== 3. 玩家寄拍一口价：成交扣税、货款进待领取 ==");
    var seller = NewBag(20, ("potion", 10));
    var consign = await trader.ConsignAsync("seller", seller, new ClientShopConsign
    {
        ItemId = "potion",
        Count = 3,
        SaleMode = ShopSaleMode.Fixed,
        Price = 500,
        Title = "便宜药水",
    });
    Check(consign.IsOk && consign.Value.Listing.Source == ShopSource.Player, "玩家寄拍上架");
    Check(seller.GetItemCount("potion") == 7, "寄拍物品先从背包扣除");
    var playerListingId = consign.Value.Listing.ListingId;
    CheckErr(await trader.BuyAsync("seller", seller, playerListingId, 1), "不能购买自己寄拍的物品");

    var buyerGold = NewBag(20, ("gold", 1000));
    var buyListing = await trader.BuyAsync("buyer", buyerGold, playerListingId, 1);
    Check(buyListing.IsOk, "其他玩家购买寄拍");
    Check(buyerGold.GetItemCount("potion") == 3, "买家收到寄拍物品");
    Check(buyerGold.GetItemCount("gold") == 500, "买家扣除 500 金币");

    var sellerClaim = await trader.ClaimAsync("seller", seller);
    Check(sellerClaim.Granted.Any(g => g.ItemId == "gold" && g.Count == 450), "卖家收到扣 10% 税后的 450 金币");
    var sellerActive = await redis.Execute("test.scard",
        db => db.SetLengthAsync(store.Keys.SellerListings("seller")));
    Check(sellerActive == 0, "成交后释放寄拍名额");

    Console.WriteLine("== 4. 玩家限时拍卖：取消 / 流拍退回 / 成交 ==");
    var seller2 = NewBag(20, ("potion", 10));
    var cancelAuction = await trader.ConsignAsync("seller2", seller2, new ClientShopConsign
    {
        ItemId = "potion",
        Count = 2,
        SaleMode = ShopSaleMode.Auction,
        StartPrice = 100,
        MinIncrement = 10,
        DurationSeconds = 60,
    });
    Check(cancelAuction.IsOk, "玩家拍卖上架");
    var cancelId = cancelAuction.Value.Listing.ListingId;
    var cancel = await trader.CancelAsync("seller2", seller2, cancelId);
    Check(cancel.IsOk && cancel.Value.Listing.Status == ListingStatuses.Cancelled, "无出价时可以取消拍卖");
    Check(seller2.GetItemCount("potion") == 10, "取消后物品立即退回背包");

    var expireAuction = await trader.ConsignAsync("seller2", seller2, new ClientShopConsign
    {
        ItemId = "potion",
        Count = 2,
        SaleMode = ShopSaleMode.Auction,
        StartPrice = 100,
        MinIncrement = 10,
        DurationSeconds = 60,
    });
    var expireId = expireAuction.Value.Listing.ListingId;
    var expireNow = Now() + 2000;
    await ExpireNow(expireId, expireNow);
    var expireSettled = await store.SettleExpiredAsync(100, expireNow);
    Check(expireSettled.Count == 1 && expireSettled[0].Type == "expired", "无人出价到期流拍");
    var seller2Claim = await trader.ClaimAsync("seller2", seller2);
    Check(seller2Claim.Granted.Any(g => g.ItemId == "potion" && g.Count == 2), "流拍物品退回卖家");
    Check(await redis.Execute("test.scard2",
        db => db.SetLengthAsync(store.Keys.SellerListings("seller2"))) == 0, "取消 / 流拍后释放寄拍名额");
    Check(seller2.GetItemCount("potion") == 10, "流拍后物品数量恢复");

    var seller3 = NewBag(20, ("potion", 2));
    var auction2 = await trader.ConsignAsync("seller3", seller3, new ClientShopConsign
    {
        ItemId = "potion",
        Count = 1,
        SaleMode = ShopSaleMode.Auction,
        StartPrice = 100,
        MinIncrement = 10,
        DurationSeconds = 60,
    });
    var auction2Id = auction2.Value.Listing.ListingId;
    CheckErr(await trader.BidAsync("seller3", seller3, auction2Id, 100), "不能给自己的寄拍出价");
    Check((await trader.BidAsync("alice", alice, auction2Id, 100)).IsOk, "alice 竞拍玩家寄拍");
    CheckErr(await trader.CancelAsync("seller3", seller3, auction2Id), "有出价后卖家不能取消");
    var adminCancel = await store.CancelAsync(auction2Id, "admin", force: true, Now());
    Check(adminCancel.Ok, "admin 可以强制下架并退款");
    var aliceRefund2 = await trader.ClaimAsync("alice", alice);
    Check(aliceRefund2.Granted.Any(g => g.ItemId == "gold" && g.Count == 100), "强制下架后出价退回");

    Console.WriteLine("== 5. 背包满：物品留在待领取，不会丢 ==");
    var smallBag = NewBag(1, ("gold", 1));
    var c = await trader.ClaimAsync("smallBag", smallBag);
    Check(c.Remaining == 0, "初始没有待领取");

    var freeProduct = new ShopProduct
    {
        ProductId = "p_free",
        Name = "免费药水",
        SaleMode = ListingMode.Fixed,
        CurrencyItemId = "diamond",
        Price = 0,
        Stock = -1,
        Items = new List<ShopGrant> { new("potion", 1) },
    };
    Check((await store.PublishAsync(freeProduct, Now())).Ok, "免费商品上架");
    var freeBuy = await trader.BuyAsync("smallBag", smallBag, "p_free", 1);
    Check(freeBuy.IsOk && freeBuy.Value.Message.Length > 0, "背包满时购买成功但提示暂存");
    Check(await store.PendingDeliveryCountAsync("smallBag") == 1, "待领取队列保留 1 条");

    smallBag.RemoveAt(0);
    var retryClaim = await trader.ClaimAsync("smallBag", smallBag);
    Check(retryClaim.Granted.Any(g => g.ItemId == "potion" && g.Count == 1), "整理背包后成功领取");

    Console.WriteLine("== 6. 幂等：同一个 orderId 重试不会重复发货 ==");
    var dupOrder = Guid.NewGuid().ToString("N");
    var dup1 = await store.PurchaseAsync("p_free", "dup", 1, Now(), dupOrder, 0, "diamond");
    var dup2 = await store.PurchaseAsync("p_free", "dup", 1, Now(), dupOrder, 0, "diamond");
    Check(dup1.Ok && dup2.Ok, "重复 orderId 购买都返回成功");
    Check(await store.PendingDeliveryCountAsync("dup") == 1, "重复 orderId 只生成一条结算");

    // 第二次调用时商品已经售罄；幂等检查必须先于「售罄」校验，否则会误判失败并触发退款
    var oneStock = new ShopProduct
    {
        ProductId = "p_one",
        Name = "单库存商品",
        SaleMode = ListingMode.Fixed,
        CurrencyItemId = "diamond",
        Price = 1,
        Stock = 1,
        Items = new List<ShopGrant> { new("potion", 1) },
    };
    Check((await store.PublishAsync(oneStock, Now())).Ok, "单库存商品上架");
    var oneOrder = Guid.NewGuid().ToString("N");
    var one1 = await store.PurchaseAsync("p_one", "u1", 1, Now(), oneOrder, 1, "diamond");
    var one2 = await store.PurchaseAsync("p_one", "u1", 1, Now(), oneOrder, 1, "diamond");
    Check(one1.Ok && one2.Ok, "售罄后重试同一个 orderId 仍返回成功");

    var bidAuction = new ShopProduct
    {
        ProductId = "p_dup_bid",
        Name = "出价幂等",
        SaleMode = ListingMode.Auction,
        CurrencyItemId = "diamond",
        StartPrice = 10,
        MinIncrement = 1,
        AuctionDurationSeconds = 60,
        Items = new List<ShopGrant> { new("potion", 1) },
    };
    var bidAuctionPub = await store.PublishAsync(bidAuction, Now());
    Check(bidAuctionPub.Ok && bidAuctionPub.Listing != null, "出价幂等测试商品上架");
    var bidAuctionId = bidAuctionPub.Listing!.ListingId;
    var bidOrder = Guid.NewGuid().ToString("N");
    var b1 = await store.PlaceBidAsync(bidAuctionId, "dupbidder", 10, Now(), bidOrder, "diamond");
    var b2 = await store.PlaceBidAsync(bidAuctionId, "dupbidder", 10, Now(), bidOrder, "diamond");
    Check(b1.Ok && b2.Ok, "重复 orderId 出价不会因最低价变化而失败");
    var bidAfter = await store.GetListingAsync(bidAuctionId);
    Check(bidAfter != null && bidAfter.TopBidderId == "dupbidder" && bidAfter.CurrentPrice == 10,
        "重复出价不会重复抬价");

    Console.WriteLine("== 7. 官方拍卖自动续拍 ==");
    var restartProduct = new ShopProduct
    {
        ProductId = "p_restart",
        Name = "循环拍卖",
        SaleMode = ListingMode.Auction,
        CurrencyItemId = "diamond",
        StartPrice = 100,
        MinIncrement = 10,
        AuctionDurationSeconds = 60,
        AutoRestart = true,
        EndTime = Now() + 60_000,
        Items = new List<ShopGrant> { new("sword", 1) },
    };
    var restartPublish = await store.PublishAsync(restartProduct, Now());
    Check(restartPublish.Ok && restartPublish.Listing != null, "自动续拍商品上架");
    var firstAuctionId = restartPublish.Listing!.ListingId;
    Check((await trader.BidAsync("alice", alice, firstAuctionId, 100)).IsOk, "自动续拍场次有人出价");
    var settleAt = Now();
    await ExpireNow(firstAuctionId, settleAt);
    var firstSettle = await store.SettleExpiredAsync(100, settleAt);
    Check(firstSettle.Count == 1, "第一场到期结算");

    var activeAfter = await store.QueryListingsAsync(
        new ShopListingQuery(IncludeInactive: false, Take: 100), Now());
    Check(activeAfter.Items.Any(x => x.ProductId == "p_restart" && x.ListingId != firstAuctionId),
        "自动续拍生成新的在售拍卖");

    // 清理本次测试写入的 key，避免反复运行把共享 Redis 撑大
    foreach (var key in await redis.GetKeys(settings.KeyPrefix + "*"))
    {
        redis.DelData(key);
    }

    Console.WriteLine();
    Console.WriteLine($"全部通过：{passed} 项断言");
    return 0;
}
finally
{
    if (redisProcess is { HasExited: false })
    {
        try
        {
            redisProcess.Kill(entireProcessTree: true);
            redisProcess.WaitForExit(3000);
        }
        catch
        {
            // ignore
        }
    }

    if (tempDir != null && Directory.Exists(tempDir))
    {
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch
        {
            // ignore
        }
    }
}

static string? FindBundledRedis()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, "dependences", "windows", "redis", "redis-server.exe");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        dir = dir.Parent;
    }

    return null;
}

static int FreePort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();
    return port;
}

static async Task<bool> WaitPort(int port, int timeoutMs)
{
    var deadline = Environment.TickCount64 + timeoutMs;
    while (Environment.TickCount64 < deadline)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
            return true;
        }
        catch
        {
            await Task.Delay(100);
        }
    }

    return false;
}
