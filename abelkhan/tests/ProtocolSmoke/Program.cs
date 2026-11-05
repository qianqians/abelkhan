using chat;
using client;
using consts;
using core;
using engine;
using friend;
using guild;
using hub;
using leaderboard;
using quest;
using shop;

namespace ProtocolSmoke;

/// <summary>
/// 所有客户端协议的用例，**全部通过 client 模块的封装调用**（client.Player.Xxx()），
/// 走真实链路：客户端报文 → 进程内 gate 转换 → hub 实体 handler → Redis/子系统
/// → 回包 → gate 转换 → 客户端分发 → 封装方法返回 Result。
///
/// 覆盖 40 条客户端协议：邮件 1 / 商城 6 / 排行榜 3 / 好友 6 / 聊天 4 / 帮会 17 / 任务 3。
/// </summary>
internal static class Program
{
    private static int _passed;
    private static int _failed;
    private static readonly List<string> Failures = new();

    private static async Task<int> Main()
    {
        var redisExe = FindBundledRedis();
        var external = Environment.GetEnvironmentVariable("PROTOCOL_TEST_REDIS");
        if (string.IsNullOrWhiteSpace(external) && redisExe == null)
        {
            Console.WriteLine("SKIP: 未找到 Redis（设置 PROTOCOL_TEST_REDIS 或放入 dependences/windows/redis）");
            return 0;
        }

        System.Diagnostics.Process? redisProcess = null;
        string? tempDir = null;
        int port;
        if (!string.IsNullOrWhiteSpace(external))
        {
            port = int.Parse(external.Split(':', 2)[1]);
        }
        else
        {
            port = FreePort();
            tempDir = Path.Combine(AppContext.BaseDirectory, "protocol-smoke-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            redisProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
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
            await using var h = ProtocolHarness.Create($"127.0.0.1:{port}");

            await MailProtocols(h);
            await LeaderboardProtocols(h);
            await FriendProtocols(h);
            await ChatProtocols(h);
            await GuildProtocols(h);
            await QuestProtocols(h);
            await ShopProtocols(h);
            await MainThreadBridgeProtocols(h);
            await MetricsProtocols(h);

            Console.WriteLine();
            Console.WriteLine($"通过 {_passed} 项，失败 {_failed} 项");
            foreach (var f in Failures)
            {
                Console.WriteLine("  FAIL " + f);
            }

            return _failed == 0 ? 0 : 1;
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
    }

    // ================================================================
    // 邮件（1）
    // ================================================================

    private static async Task MailProtocols(ProtocolHarness h)
    {
        Section("邮件 client_request_mail");
        var alice = await h.LoginAsync("mail_alice");

        var pull = await alice.Player.RequestMail();
        Check(pull, "RequestMail() 拉取可领取全局邮件");
        Check(pull.IsOk && pull.Value.Mails.Count == 0, "未配置全局邮件时返回空列表");
    }

    // ================================================================
    // 排行榜（3）
    // ================================================================

    private static async Task LeaderboardProtocols(ProtocolHarness h)
    {
        Section("排行榜 client_leaderboard_*");
        var alice = await h.LoginAsync("lb_alice");
        var bob = await h.LoginAsync("lb_bob");

        var submit = await alice.Player.SubmitLeaderboardScore("power", 150);
        Check(submit, "SubmitLeaderboardScore() 打榜");
        Check(submit.IsOk && submit.Value.Rank == 1, "名次 1，分数 150");

        await bob.Player.SubmitLeaderboardScore("power", 300);
        await alice.Player.SubmitLeaderboardScore("power", 50); // 低于最高分不覆盖

        var range = await alice.Player.RequestLeaderboardRange("power", 0, 10);
        Check(range, "RequestLeaderboardRange() 区间拉榜");
        Check(range.IsOk && range.Value.Total == 2 && range.Value.Entries[0].Score == 300,
            "降序排列且低分不覆盖高分");

        var rank = await alice.Player.RequestLeaderboardRank("power");
        Check(rank, "RequestLeaderboardRank() 查自己名次");
        Check(rank.IsOk && rank.Value.Entry.Rank == 2 && rank.Value.Entry.Score == 150, "alice 第 2 名");

        var other = await alice.Player.RequestLeaderboardRank("power", "lb_bob");
        Check(other.IsOk && other.Value.Entry.Rank == 1, "查别人名次");

        var bad = await alice.Player.SubmitLeaderboardScore("", 1);
        CheckErr(bad, "board_id 为空被拒");
    }

    // ================================================================
    // 好友（6）
    // ================================================================

    private static async Task FriendProtocols(ProtocolHarness h)
    {
        Section("好友 client_friend_*");
        var alice = await h.LoginAsync("fr_alice");
        var bob = await h.LoginAsync("fr_bob");
        var carol = await h.LoginAsync("fr_carol");

        var requested = new List<ClientFriendRequestNotify>();
        bob.Player.FriendRequested += n => requested.Add(n);
        var online = new List<ClientFriendOnlineNotify>();
        bob.Player.FriendOnline += n => online.Add(n);

        var send = await alice.Player.SendFriendRequest("fr_bob");
        Check(send, "SendFriendRequest() 发送申请");
        Check(send.IsOk && send.Value.Pending, "申请已进入待处理");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => requested.Count == 1), "被申请方收到 FriendRequested 事件");
        Check(requested.Count == 1 && requested[0].From.UserId == "fr_alice", "事件里是 alice");

        var dup = await alice.Player.SendFriendRequest("fr_bob");
        CheckErr(dup, "重复申请被拒");

        var requests = await bob.Player.RequestFriendRequests();
        Check(requests, "RequestFriendRequests() 拉待处理申请");
        Check(requests.IsOk && requests.Value.Requests.Count == 1, "申请列表 1 条");

        var accept = await bob.Player.AcceptFriendRequest("fr_alice");
        Check(accept, "AcceptFriendRequest() 同意（占位 + 跨服确认）");

        var list = await alice.Player.RequestFriendList();
        Check(list, "RequestFriendList() 拉好友列表");
        Check(list.IsOk && list.Value.Friends.Any(f => f.UserId == "fr_bob"), "列表里有 bob");

        // 拒绝路径
        await carol.Player.SendFriendRequest("fr_alice");
        await h.PumpAsync();
        var reject = await alice.Player.RejectFriendRequest("fr_carol");
        Check(reject, "RejectFriendRequest() 拒绝申请");

        var remove = await alice.Player.RemoveFriend("fr_bob");
        Check(remove, "RemoveFriend() 删除好友");

        var removeAgain = await alice.Player.RemoveFriend("fr_bob");
        CheckErr(removeAgain, "重复删除被拒");
    }

    // ================================================================
    // 聊天（4）
    // ================================================================

    private static async Task ChatProtocols(ProtocolHarness h)
    {
        Section("聊天 client_chat_*");
        var alice = await h.LoginAsync("ch_alice");
        var bob = await h.LoginAsync("ch_bob");

        var messages = new List<ClientChatMessageNotify>();
        bob.Player.ChatMessage += n => messages.Add(n);
        var privates = new List<ClientChatPrivateNotify>();
        bob.Player.ChatPrivate += n => privates.Add(n);

        var channels = await alice.Player.RequestChatChannels();
        Check(channels, "RequestChatChannels() 拉频道列表");
        Check(channels.IsOk && channels.Value.Channels.Count > 0, "至少一个频道");

        var send = await alice.Player.SendChatMessage("world", "hello");
        Check(send, "SendChatMessage() 世界频道发言");
        Check(send.IsOk && send.Value.Entry.Content == "hello", "返回自己的消息");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => messages.Count >= 1), "同频道在线玩家收到 ChatMessage 事件");

        var cooldown = await alice.Player.SendChatMessage("world", "again");
        CheckErr(cooldown, "频道冷却期内被拒");

        var unknown = await alice.Player.SendChatMessage("nope", "x");
        CheckErr(unknown, "未知频道被拒");

        var history = await alice.Player.RequestChatHistory("world", 10);
        Check(history, "RequestChatHistory() 拉历史");
        Check(history.IsOk && history.Value.Entries.Any(e => e.Content == "hello"), "历史里有刚发的消息");

        var pm = await alice.Player.SendPrivateMessage("ch_bob", "hi bob");
        Check(pm, "SendPrivateMessage() 私聊（对方在线）");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => privates.Count == 1), "对方收到 ChatPrivate 事件");
        Check(privates.Count == 1 && privates[0].Entry.Content == "hi bob", "私聊内容正确");

        // 等私聊冷却（默认 500ms）过去，否则这条会先撞冷却、测不到「离线」
        await Task.Delay(700);
        var toOffline = await alice.Player.SendPrivateMessage("nobody_here", "x");
        CheckErrContains(toOffline, "对方不在线", "对方离线被拒");

        var toSelf = await alice.Player.SendPrivateMessage("ch_alice", "x");
        CheckErr(toSelf, "不能给自己发私聊");
    }

    // ================================================================
    // 帮会（17）
    // ================================================================

    private static async Task GuildProtocols(ProtocolHarness h)
    {
        Section("帮会 client_guild_*");
        var alice = await h.LoginAsync("gd_alice"); // 帮主
        var bob = await h.LoginAsync("gd_bob");     // 副帮主
        var carol = await h.LoginAsync("gd_carol"); // 成员
        var dave = await h.LoginAsync("gd_dave");   // 路人

        var invited = new List<ClientGuildInvitedNotify>();
        bob.Player.GuildInvited += n => invited.Add(n);
        var removed = new List<ClientGuildRemovedNotify>();
        carol.Player.GuildRemoved += n => removed.Add(n);
        var taskUpdated = new List<ClientGuildTaskUpdatedNotify>();
        alice.Player.GuildTaskUpdated += n => taskUpdated.Add(n);

        var create = await alice.Player.CreateGuild("铁血盟");
        Check(create, "CreateGuild() 创建帮会");
        Check(create.IsOk && create.Value.Guild.InGuild &&
              create.Value.Guild.MyRole == (uint)GuildRole.Leader, "创建者是帮主");
        var guildId = create.Value.Guild.GuildId;

        var dupName = await dave.Player.CreateGuild("铁血盟");
        CheckErr(dupName, "同名创建被拒（名字唯一）");

        var info = await alice.Player.RequestGuildInfo();
        Check(info, "RequestGuildInfo() 拉帮会信息");
        Check(info.IsOk && info.Value.MemberCount == 1, "1 名成员");

        var rename = await alice.Player.RenameGuild("铁血盟二");
        Check(rename, "RenameGuild() 改名（帮主）");

        var notice = await alice.Player.SetGuildNotice("今晚八点打团");
        Check(notice, "SetGuildNotice() 改公告");

        var invite = await alice.Player.InviteToGuild("gd_bob");
        Check(invite, "InviteToGuild() 邀请 bob");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => invited.Count == 1), "bob 收到 GuildInvited 事件");

        var reply = await bob.Player.ReplyGuildInvite(guildId, true);
        Check(reply, "ReplyGuildInvite() 接受邀请");
        Check(reply.IsOk && reply.Value.Guild.MemberCount == 2, "入帮后 2 人");

        var apply = await carol.Player.ApplyToGuild(guildId);
        Check(apply, "ApplyToGuild() 申请加入");

        var applies = await alice.Player.RequestGuildApplies();
        Check(applies, "RequestGuildApplies() 拉待审批列表");
        Check(applies.IsOk && applies.Value.Applies.Any(a => a.UserId == "gd_carol"), "列表含 carol");

        var approve = await alice.Player.ApproveGuildApply("gd_carol", true);
        Check(approve, "ApproveGuildApply() 审批通过");

        // carol 是被别人（alice 的 hub）拉进帮的，本地镜像要拉一次 info 才同步
        await carol.Player.RequestGuildInfo();
        var approveDenied = await carol.Player.ApproveGuildApply("gd_dave", true);
        CheckErrContains(approveDenied, "你没有这个权限", "普通成员审批被拒（权限）");

        var promote = await alice.Player.SetGuildRole("gd_bob", 2);
        Check(promote, "SetGuildRole() 升副帮主");

        var kick = await bob.Player.KickFromGuild("gd_carol");
        Check(kick, "KickFromGuild() 副帮主踢人");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => removed.Count == 1), "被踢的人收到 GuildRemoved 事件");
        Check(removed.Count == 1 && removed[0].Reason == "kicked", "原因为 kicked");

        var kickLeader = await bob.Player.KickFromGuild("gd_alice");
        CheckErr(kickLeader, "副帮主不能踢帮主");

        // 帮会任务（发布 / 领取 / 取消）
        var publish = await alice.Player.PublishGuildTask("g_task_daily");
        Check(publish, "PublishGuildTask() 发布任务");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => taskUpdated.Count >= 1), "收到 GuildTaskUpdated 事件");

        var publishDup = await alice.Player.PublishGuildTask("g_task_daily");
        CheckErr(publishDup, "重复发布被拒");

        var taskAccept = await bob.Player.AcceptGuildTask("g_task_daily");
        Check(taskAccept, "AcceptGuildTask() 领取任务");

        var taskCancel = await alice.Player.CancelGuildTask("g_task_daily");
        Check(taskCancel, "CancelGuildTask() 取消任务");

        // 退出 / 转让 / 解散
        var leaveLeader = await alice.Player.LeaveGuild();
        CheckErr(leaveLeader, "帮主不能直接退出");

        var transfer = await alice.Player.TransferGuild("gd_bob");
        Check(transfer, "TransferGuild() 转让帮主");

        var disbandDenied = await alice.Player.DisbandGuild();
        CheckErrContains(disbandDenied, "你没有这个权限", "老帮主（已降副帮主）解散被拒");

        var leaveOld = await alice.Player.LeaveGuild();
        Check(leaveOld, "LeaveGuild() 老帮主（已降副帮主）退出");

        var disband = await bob.Player.DisbandGuild();
        Check(disband, "DisbandGuild() 解散帮会");

        var infoAfter = await bob.Player.RequestGuildInfo();
        Check(infoAfter.IsOk && !infoAfter.Value.InGuild, "解散后不属于任何帮会");
    }

    // ================================================================
    // 任务（3）
    // ================================================================

    private static async Task QuestProtocols(ProtocolHarness h)
    {
        Section("任务 client_quest_*");
        var alice = await h.LoginAsync("qs_alice");

        var updated = new List<ClientQuestUpdatedNotify>();
        alice.Player.QuestUpdated += n => updated.Add(n);

        var list0 = await alice.Player.RequestQuestList();
        Check(list0, "RequestQuestList() 拉任务列表");
        Check(list0.IsOk, "列表可读");

        var notYet = await alice.Player.AcceptQuest("q_manual");
        CheckErr(notYet, "触发条件未满足时接取被拒");

        // 上报等级事件 → 自动任务被接取、手动任务进可接列表
        await h.QuestService.ReportEventAsync("qs_alice", "level", "", 1, QuestConditionMode.Max);
        await h.PumpAsync();

        var list1 = await alice.Player.RequestQuestList();
        Check(list1.IsOk && list1.Value.Active.Any(q => q.QuestId == "q_auto"), "自动任务已自动接取");
        Check(list1.IsOk && list1.Value.Available.Any(q => q.QuestId == "q_manual"), "手动任务进可接列表");

        var accept = await alice.Player.AcceptQuest("q_manual");
        Check(accept, "AcceptQuest() 手动接取");
        Check(accept.IsOk && accept.Value.Quest.Status == QuestStatus.Active, "接取后状态为进行中");

        var completeEarly = await alice.Player.CompleteQuest("q_auto");
        CheckErr(completeEarly, "进度/门槛没满足时提交被拒");

        await h.QuestService.ReportEventAsync("qs_alice", "kill", "goblin", 2);
        await h.QuestService.ReportEventAsync("qs_alice", "talk", "npc", 1, QuestConditionMode.Flag);
        await h.PumpAsync();

        Check(await TestClient.WaitForAsync(() => updated.Count >= 1), "收到 QuestUpdated 事件");

        var complete = await alice.Player.CompleteQuest("q_auto");
        Check(complete, "CompleteQuest() 提交完成");
        Check(complete.IsOk && complete.Value.NextQuestIds.Contains("q_next"), "完成后触发后继 q_next");

        var completeAgain = await alice.Player.CompleteQuest("q_auto");
        CheckErr(completeAgain, "重复提交被拒");

        var unknown = await alice.Player.AcceptQuest("no_such_quest");
        CheckErr(unknown, "未知任务被拒");
    }

    // ================================================================
    // 商城（6）
    // ================================================================

    private static async Task ShopProtocols(ProtocolHarness h)
    {
        Section("商城 client_shop_*");
        var seller = await h.LoginAsync("sh_seller");
        var buyer = await h.LoginAsync("sh_buyer");
        var bidder = await h.LoginAsync("sh_bidder");

        ProtocolHarness.GiveItem(buyer.HubPlayer, "diamond", 10_000);
        ProtocolHarness.GiveItem(bidder.HubPlayer, "diamond", 10_000);
        ProtocolHarness.GiveItem(seller.HubPlayer, "potion", 10);

        var shopStore = h.Service.Shop!;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Check((await shopStore.PublishAsync(new ShopProduct
        {
            ProductId = "p_fixed",
            Name = "新手礼包",
            SaleMode = ListingMode.Fixed,
            CurrencyItemId = "diamond",
            Price = 100,
            Stock = 5,
            Items = new List<shop.ShopItemGrant> { new shop.ShopItemGrant("potion", 1) },
        }, now)).Ok, "准备：官方一口价商品上架");

        var auction = await shopStore.PublishAsync(new ShopProduct
        {
            ProductId = "p_auction",
            Name = "限时神装",
            SaleMode = ListingMode.Auction,
            CurrencyItemId = "diamond",
            StartPrice = 100,
            MinIncrement = 10,
            AuctionDurationSeconds = 60,
            Items = new List<shop.ShopItemGrant> { new shop.ShopItemGrant("potion", 2) },
        }, now);
        Check(auction.Ok && auction.Listing != null, "准备：官方拍卖上架");
        var auctionId = auction.Listing!.ListingId;

        var list = await buyer.Player.RequestShopList();
        Check(list, "RequestShopList() 浏览商城");
        Check(list.IsOk && list.Value.Listings.Count >= 2, "列表里有两个商品");

        var buy = await buyer.Player.BuyShopItem("p_fixed", 1);
        Check(buy, "BuyShopItem() 一口价购买");
        Check(buy.IsOk && buy.Value.Listing.Stock == 4, "库存 5 → 4");

        var bid = await bidder.Player.BidShopItem(auctionId, 100);
        Check(bid, "BidShopItem() 拍卖出价");

        var consign = await seller.Player.ConsignShopItem("potion", 1, ShopSaleMode.Fixed,
            price: 500, durationSeconds: 60, title: "便宜药水");
        Check(consign, "ConsignShopItem() 玩家寄拍");

        var claim = await bidder.Player.ClaimShopDeliveries();
        Check(claim, "ClaimShopDeliveries() 领取待领取结算");

        var consign2 = await seller.Player.ConsignShopItem("potion", 1, ShopSaleMode.Fixed,
            price: 500, durationSeconds: 60);
        Check(consign2, "再寄拍一件用于取消");

        var cancel = await seller.Player.CancelShopConsign(consign2.Value.Listing.ListingId);
        Check(cancel, "CancelShopConsign() 取消寄拍");

        var buyOwn = await seller.Player.BuyShopItem(consign.Value.Listing.ListingId, 1);
        CheckErr(buyOwn, "不能购买自己寄拍的物品");
    }

    // ================================================================
    // 引擎接主线程桥（Stride / Godot 接入的关键约束）
    // ================================================================

    /// <summary>
    /// 验证 <c>ClientConfig.PostToMainThread</c>：设了之后，实体回调（服务器 notify）
    /// 必须全部跑到「主线程」上，且保持顺序；请求/响应不受影响。
    /// </summary>
    private static async Task MainThreadBridgeProtocols(ProtocolHarness h)
    {
        Section("引擎接入：ClientConfig.PostToMainThread 主线程桥");
        using var main = new FakeMainThread();

        var alice = await h.LoginAsync("mt_alice", main.Post);
        var bob = await h.LoginAsync("mt_bob", main.Post);

        var seenThreads = new List<int>();
        var seenContent = new List<string>();
        var lockObj = new object();

        bob.Player.ChatMessage += n =>
        {
            lock (lockObj)
            {
                seenThreads.Add(Environment.CurrentManagedThreadId);
                seenContent.Add(n.Entry.Content);
            }
        };

        // 请求/响应链路：设了主线程钩子也必须照常返回
        var send1 = await alice.Player.SendChatMessage("world", "mt-1");
        Check(send1, "设了主线程钩子后 request/response 仍然正常");

        await Task.Delay(3200); // 等频道冷却
        var send2 = await alice.Player.SendChatMessage("world", "mt-2");
        Check(send2, "第二次发言（绕过冷却）");

        await h.PumpAsync();
        await main.PumpAsync();

        lock (lockObj)
        {
            Check(seenThreads.Count >= 2, $"notify 都投递到了（收到 {seenThreads.Count} 条）");
            Check(seenThreads.All(id => id == main.MainThreadId),
                "notify handler 全部跑在主线程上（不是网络线程池）");
            Check(seenContent.SequenceEqual(["mt-1", "mt-2"]), "notify 按发送顺序到达（保序）");
        }
    }

    // ================================================================
    // 监控：指标本身也要被测（否则很容易出现「名字写错，图上永远 0」）
    // ================================================================

    private static async Task MetricsProtocols(ProtocolHarness h)
    {
        Section("监控：/metrics 输出与指标计数");
        var text = await ProtocolHarness.ScrapeMetricsAsync();

        double Value(string name)
        {
            // 取该指标所有样本之和
            var total = 0d;
            foreach (var line in text.Split('\n'))
            {
                if (line.StartsWith('#'))
                {
                    continue;
                }

                if (!line.StartsWith(name, StringComparison.Ordinal))
                {
                    continue;
                }

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && double.TryParse(parts[^1], out var v))
                {
                    total += v;
                }
            }

            return total;
        }

        Check(text.Contains("abelkhan_hub_messages_total"), "导出里包含 hub 报文计数");
        Check(Value("abelkhan_hub_messages_total") >= 40,
            $"跑完 40+ 条协议后 hub 报文计数 >= 40（实际 {Value("abelkhan_hub_messages_total"):0}）");

        Check(Value("abelkhan_redis_operation_duration_seconds_count") > 0,
            "Redis 操作直方图有样本（收口点确实被包住了）");
        Check(text.Contains("abelkhan_redis_operation_duration_seconds_bucket{node=\"smoke-node\",operation=\"leaderboard.submit\",le=\"0.0005\"}"),
            "Redis 直方图用了自定义细桶（le=0.0005），标签带 node + operation");

        Check(Value("abelkhan_quest_events_total") >= 3, "任务事件计数已累加");
        Check(Value("abelkhan_chat_broadcast_total") >= 1, "聊天广播计数已累加");
        Check(Value("abelkhan_guild_created_total") >= 1, "帮会创建计数已累加");

        Check(text.Contains("abelkhan_runtime_heap_bytes"), "运行时指标（GC 堆）已导出");
        Check(text.Contains("node=\"smoke-node\""), "所有指标都带 node 标签（多副本可区分）");
        Check(!text.Contains("user_id"), "指标里没有高基数的用户维度标签");
    }

    // ================================================================
    // 断言
    // ================================================================

    private static void Section(string title) => Console.WriteLine($"\n== {title} ==");

    private static void Check<T>(Result<T, string> result, string name) where T : class
    {
        if (result.IsOk)
        {
            Pass(name);
        }
        else
        {
            Fail($"{name}（返回错误：{result.Error}）");
        }
    }

    private static void Check(bool ok, string name)
    {
        if (ok)
        {
            Pass(name);
        }
        else
        {
            Fail(name);
        }
    }

    private static void CheckErrContains<T>(Result<T, string> result, string expected, string name)
        where T : class
    {
        if (result.IsOk)
        {
            Fail($"{name}（期望错误但成功了）");
        }
        else if (!result.Error.Contains(expected, StringComparison.Ordinal))
        {
            Fail($"{name}（期望包含「{expected}」，实际「{result.Error}」）");
        }
        else
        {
            Pass($"{name}  [{result.Error}]");
        }
    }

    private static void CheckErr<T>(Result<T, string> result, string name) where T : class
    {
        if (result.IsOk)
        {
            Fail($"{name}（期望错误但成功了）");
        }
        else
        {
            Pass($"{name}  [{result.Error}]");
        }
    }

    private static void Pass(string name)
    {
        _passed++;
        Console.WriteLine("  ok  " + name);
    }

    private static void Fail(string name)
    {
        _failed++;
        Failures.Add(name);
        Console.WriteLine("  FAIL " + name);
    }

    private static string? FindBundledRedis()
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

    private static int FreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static async Task<bool> WaitPort(int port, int timeoutMs)
    {
        var deadline = Environment.TickCount64 + timeoutMs;
        while (Environment.TickCount64 < deadline)
        {
            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                await client.ConnectAsync(System.Net.IPAddress.Loopback, port);
                return true;
            }
            catch
            {
                await Task.Delay(100);
            }
        }

        return false;
    }
}
