using chat;
using client;
using consts;
using core;
using engine;
using friend;
using Google.Protobuf;
using guild;
using hub;
using leaderboard;
using MongoDB.Bson;
using quest;
using shop;

namespace ProtocolSmoke;

/// <summary>
/// 所有客户端协议的用例，**全部通过 client 模块的封装调用**（client.Player.Xxx()），
/// 走真实链路：客户端报文 → 进程内 gate 转换 → hub 实体 handler → Redis/子系统
/// → 回包 → gate 转换 → 客户端分发 → 封装方法返回 Result。
///
/// 覆盖 45 条客户端协议：邮件 1 / 背包 5 / 商城 6 / 排行榜 3 / 好友 6 / 聊天 4 / 帮会 17 / 任务 3。
/// </summary>
internal static class Program
{
    /// <summary>测试专用的 hub→hub 事件方法名（不进产品 Consts）。</summary>
    private const string TestHubPingMethod = "test_hub_ping";

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
            await BagProtocols(h);
            await LeaderboardProtocols(h);
            await FriendProtocols(h);
            await FriendPushProtocols(h);
            await ChatProtocols(h);
            await GuildProtocols(h);
            await QuestProtocols(h);
            await ShopProtocols(h);
            await MainThreadBridgeProtocols(h);
            await MetricsProtocols(h);
            await HubHubQueueProtocols(h);
            await ActorSelfScheduleProtocols();

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

        // 退化路径：Service.GetMailCondition 默认返回 null → 协议直接返回空列表
        var noCond = await h.LoginAsync("mail_nocond");
        var empty = await noCond.Player.RequestMail();
        Check(empty, "RequestMail() 可调用");
        Check(empty.IsOk && empty.Value.Mails.Count == 0,
            "未提供领取条件时返回空列表（这是退化路径，不等于功能可用）");

        // 打开领取条件 + 投递真实邮件，跑完整逻辑
        h.Service.MailCondition = (Level: 10, CreateDays: 3);
        await h.PushGlobalMailAsync("m_level", "等级门槛邮件", [("potion", 3)], minLevel: 99);
        await h.PushGlobalMailAsync("m_ok", "新手礼包", [("potion", 3), ("gold", 100)]);

        var alice = await h.LoginAsync("mail_alice");
        var pull = await alice.Player.RequestMail();
        Check(pull, "RequestMail() 拉取可领取邮件");
        Check(pull.IsOk && pull.Value.Mails.Count == 1 && pull.Value.Mails[0].MailId == "m_ok",
            "只返回满足条件的那封（m_level 要 99 级，被过滤）");
        Check(pull.Value.Mails[0].Attachments.Count == 2 &&
              pull.Value.Mails[0].Attachments[0].ItemId == "potion" &&
              pull.Value.Mails[0].Attachments[0].Count == 3, "附件正确下发");

        var bag = alice.HubPlayer.Bag;
        Check(bag.GetItemCount("potion") == 0 && bag.GetItemCount("gold") == 0, "领取前背包为空");

        var claim = await alice.Player.RequestMail("m_ok");
        Check(claim, "RequestMail(mailId) 领取");
        Check(claim.IsOk && claim.Value.Mails.Count == 1, "返回领取到的邮件");
        Check(bag.GetItemCount("potion") == 3 && bag.GetItemCount("gold") == 100,
            "附件进入背包（potion 3 / gold 100）");

        CheckErr(await alice.Player.RequestMail("m_ok"), "重复领取被拒");

        var afterClaim = await alice.Player.RequestMail();
        Check(afterClaim.IsOk && afterClaim.Value.Mails.Count == 0, "领取后不再出现在可领取列表");

        CheckErr(await alice.Player.RequestMail("no_such_mail"), "领取不存在的邮件被拒");

        // 附件发放失败必须回滚已领取标记，否则玩家永久丢邮件。
        // 用一个目录里没有的 item_id 触发 UnknownItem 分支（和背包满走同一条回滚路径）
        await h.PushGlobalMailAsync("m_bad", "附件配置错误的邮件", [("no_such_item", 1)]);
        CheckErr(await alice.Player.RequestMail("m_bad"), "附件无法创建时领取失败");
        var rollback = await alice.Player.RequestMail();
        Check(rollback.IsOk && rollback.Value.Mails.Any(m => m.MailId == "m_bad"),
            "领取失败后邮件回到可领取列表（已回滚认领标记）");

        h.Service.MailCondition = null;
    }

    // ================================================================
    // 背包（5）
    // ================================================================

    private static async Task BagProtocols(ProtocolHarness h)
    {
        Section("背包 client_bag_*");
        var alice = await h.LoginAsync("bag_alice");

        ProtocolHarness.GiveItem(alice.HubPlayer, "potion", 5);
        var changed = new List<ClientBagChangedNotify>();
        alice.Player.BagChanged += n => changed.Add(n);

        var info = await alice.Player.RequestBagInfo();
        Check(info, "RequestBagInfo() 拉背包");
        Check(info.IsOk && info.Value.Slots.Count == 1 &&
              info.Value.Slots[0].ItemId == "potion" && info.Value.Slots[0].Count == 5,
            "背包视图含初始物品");

        ProtocolHarness.GiveItem(alice.HubPlayer, "gold", 10);
        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => changed.Count >= 1), "收到 BagChanged 事件");
        Check(changed.Any(n => n.Slots.Any(s => s.ItemId == "gold")),
            "变更推送带新物品的最终状态");

        var moved = await alice.Player.MoveBagItem(0, 5);
        Check(moved, "MoveBagItem() 移动到空格位");
        Check(moved.IsOk && moved.Value.Bag.Slots.Any(s => s.Slot == 5 && s.ItemId == "potion"),
            "移动后 potion 在 5 号格");

        var sorted = await alice.Player.SortBag();
        Check(sorted, "SortBag() 整理背包");
        Check(sorted.IsOk &&
              sorted.Value.Bag.Slots[0].ItemId == "gold" &&
              sorted.Value.Bag.Slots[1].ItemId == "potion",
            "整理后按 item_id 排序且空格压缩");

        var discarded = await alice.Player.DiscardBagItem(1, 2);
        Check(discarded, "DiscardBagItem() 丢弃部分数量");
        Check(discarded.IsOk && discarded.Value.Bag.Slots.Any(s => s.Slot == 1 && s.Count == 3),
            "丢弃 2 个后剩 3 个");

        var used = await alice.Player.UseBagItem(1, 1);
        Check(used, "UseBagItem() 使用物品（走服务端钩子）");
        Check(used.IsOk && used.Value.Message.Contains("potion") &&
              used.Value.Bag.Slots.Any(s => s.Slot == 1 && s.Count == 2),
            "使用 1 个后剩 2 个");

        var empty = await alice.Player.DiscardBagItem(2);
        CheckErr(empty, "空格位丢弃被拒");

        var badUse = await alice.Player.UseBagItem(999);
        CheckErr(badUse, "越界格位使用被拒");
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

        var added = new List<ClientFriendAddedNotify>();
        alice.Player.FriendAdded += n => added.Add(n);

        var accept = await bob.Player.AcceptFriendRequest("fr_alice");
        Check(accept, "AcceptFriendRequest() 同意（占位 + 跨服确认）");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => added.Count == 1),
            "申请方收到 FriendAdded 推送（对方同意后）");
        Check(added.Count == 1 && added[0].UserId == "fr_bob", "FriendAdded 里是同意方 bob");

        var list = await alice.Player.RequestFriendList();
        Check(list, "RequestFriendList() 拉好友列表");
        Check(list.IsOk && list.Value.Friends.Any(f => f.UserId == "fr_bob"), "列表里有 bob");

        // 拒绝路径
        await carol.Player.SendFriendRequest("fr_alice");
        await h.PumpAsync();
        var reject = await alice.Player.RejectFriendRequest("fr_carol");
        Check(reject, "RejectFriendRequest() 拒绝申请");

        var removedNtf = new List<ClientFriendRemovedNotify>();
        bob.Player.FriendRemoved += n => removedNtf.Add(n);

        var remove = await alice.Player.RemoveFriend("fr_bob");
        Check(remove, "RemoveFriend() 删除好友");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => removedNtf.Count == 1),
            "被删除方收到 FriendRemoved 推送");

        var removeAgain = await alice.Player.RemoveFriend("fr_bob");
        CheckErr(removeAgain, "重复删除被拒");
    }

    // ================================================================
    // 聊天（4）
    // ================================================================

    /// <summary>
    /// 剩下两个不能靠普通请求触发的推送：
    ///   - FriendOffline：由 gate 上报下线触发（harness 绕过了 GateMsgHandle，所以直接调 OnOfflineAsync）
    ///   - FriendAcceptTimeout：由 hub 定时扫描超期占位触发
    /// </summary>
    private static async Task FriendPushProtocols(ProtocolHarness h)
    {
        Section("好友推送：下线 / 占位超时");

        var a = await h.LoginAsync("fp_a");
        var b = await h.LoginAsync("fp_b");
        await a.Player.SendFriendRequest("fp_b");
        await h.PumpAsync();
        Check((await b.Player.AcceptFriendRequest("fp_a")).IsOk, "准备：两人成为好友");
        await h.PumpAsync();

        var offline = new List<ClientFriendOfflineNotify>();
        b.Player.FriendOffline += n => offline.Add(n);

        // gate 剔除连接时会发 ClientDisconnect → hub 调 OnOfflineAsync；
        // harness 直接调 handler（不走 GateMsgHandle），所以这里直接调服务层
        await h.FriendService.OnOfflineAsync("fp_a");
        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => offline.Count == 1),
            "好友下线时在线方收到 FriendOffline 推送");
        Check(offline.Count == 1 && offline[0].UserId == "fp_a", "FriendOffline 里是下线的那个人");

        // 占位超时回收：直接在 store 上造一个占位，等它过期后扫
        var timeoutNtf = new List<ClientFriendAcceptTimeoutNotify>();
        var c = await h.LoginAsync("fp_c");
        c.Player.FriendAcceptTimeout += n => timeoutNtf.Add(n);

        var store = h.FriendService.Store;
        // 占位的前提是收件箱里真有这条申请，所以先让 fp_ghost 申请 fp_c
        Check((await store.SubmitRequestAsync("fp_ghost", "fp_c")).Ok, "准备：fp_ghost 申请 fp_c");
        var (reserve, _) = await store.AcceptReserveAsync("fp_c", "fp_ghost");
        Check(reserve.Ok, "准备：造一个临时占位");

        Check((await store.GetLimitAsync("fp_c")).Reserved == 1,
            "占位确实生效（Reserved=1）—— 否则后面的回收断言会空过");

        await Task.Delay(store.Settings.ReserveTimeoutMs + 400);
        await h.FriendService.SweepReservesAsync(10);
        await h.PumpAsync();

        Check(await TestClient.WaitForAsync(() => timeoutNtf.Count == 1),
            "占位超时后同意方收到 FriendAcceptTimeout 推送");
        Check(timeoutNtf.Count == 1 && timeoutNtf[0].UserId == "fp_ghost", "通知里是对方 id");
        Check((await store.GetLimitAsync("fp_c")).Reserved == 0, "超期占位已回收（好友位没泄漏）");
    }

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

        var guildUpdated = new List<ClientGuildUpdatedNotify>();
        alice.Player.GuildUpdated += n => guildUpdated.Add(n);

        var reply = await bob.Player.ReplyGuildInvite(guildId, true);
        Check(reply, "ReplyGuildInvite() 接受邀请");
        Check(reply.IsOk && reply.Value.Guild.MemberCount == 2, "入帮后 2 人");

        await h.PumpAsync();
        Check(await TestClient.WaitForAsync(() => guildUpdated.Count >= 1),
            "帮主收到 GuildUpdated 推送（有人入帮）");

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
        Check(Value("abelkhan_hub_messages_total") >= 45,
            $"跑完 45+ 条协议后 hub 报文计数 >= 45（实际 {Value("abelkhan_hub_messages_total"):0}）");

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

        // Mongo：不需要真 mongod —— 指向一个连不上的地址，验证「失败路径」也出指标
        var mongo = new core.MongoProxy("mongodb://127.0.0.1:1/?serverSelectionTimeoutMS=300");
        var threw = false;
        try
        {
            await mongo.Count("game", "player", new BsonDocument("a", 1).ToBson());
        }
        catch (Exception)
        {
            threw = true;
        }

        Check(threw, "MongoProxy 连不上时抛出（行为不变）");
        var mongoText = await ProtocolHarness.ScrapeMetricsAsync();
        Check(mongoText.Contains("abelkhan_mongo_errors_total{node=\"smoke-node\",operation=\"count\"} 1"),
            "Mongo 失败路径记了 errors 计数（收口点确实包住了）");
        Check(mongoText.Contains("abelkhan_mongo_operation_duration_seconds_count{node=\"smoke-node\",operation=\"count\"} 1"),
            "Mongo 操作进了延迟直方图");
        Check(!mongoText.Contains("operation=\"player\""), "collection 名没有进指标标签（避免高基数）");
    }

    /// <summary>
    /// hub→hub 队列的 Push/Pop 配对：
    ///   - CallEntityEvent 推进去的字节，必须能被 MainClass 那样的 PopList + ParseFrom 还原；
    ///   - 不同 hubId 必须落到不同的 key（守住「缺 {0} 占位符 → 所有 hub 抢同一队列」这个坑）；
    ///   - 事件能真的派发到 RegisterHubNotify 的 handler（不走 connId / client）。
    /// </summary>
    private static async Task HubHubQueueProtocols(ProtocolHarness h)
    {
        Section("hub→hub 队列：Push/Pop 配对与事件派发");

        // 1) key 随 hubId 变化（缺 {0} 时这条失败）
        Check(string.Format(Consts.HubHubMq, "hub-1") != string.Format(Consts.HubHubMq, "hub-2"),
            "HubHubMq 带 {0} 占位符：不同 hub 不同队列");

        // 2) 注册一个不依赖 connId 的 hub 事件 handler
        var alice = await h.LoginAsync("hh_alice");
        var got = new List<string>();
        alice.HubPlayer.RegisterHubNotify<NotifyConnID>(TestHubPingMethod,
            (srcHubId, msg) => { lock (got) { got.Add(srcHubId + ":" + msg.ConnId); } });

        var global = new HubGlobalEvent(h.Redis);
        global.OnEntityCreate(alice.HubPlayer);

        // 3) 从「另一个 hub」推事件，再用 MainClass 同样的方式 Pop + 派发
        await global.CallEntityEvent("hub-other", "hub-target", alice.HubPlayer.EntityId,
            new CallRpc { ProtoName = TestHubPingMethod, Content = new NotifyConnID { ConnId = "hello" }.ToByteString() });

        var popTask = h.Redis.PopList(string.Format(Consts.HubHubMq, "hub-target"), 8);
        var frames = popTask == null ? null : await popTask;
        Check(frames != null && frames.Count == 1, "PopList 取到 CallEntityEvent 推的那一条（编码对称）");

        var parser = new MessageParser<HubHubNotifyEntity>(() => new HubHubNotifyEntity());
        var ev = parser.ParseFrom(frames![0]);
        Check(ev.SrcHubId == "hub-other" && ev.EntityId == alice.HubPlayer.EntityId,
            "src_hub_id / entity_id 原样还原");

        global.OnEntityEvent(ev.SrcHubId, ev.EntityId, ev.Event);
        Check(await TestClient.WaitForAsync(() => { lock (got) return got.Count == 1; }),
            "事件派发到 RegisterHubNotify 的 handler（不走 connId）");
        lock (got)
        {
            Check(got.Count == 1 && got[0] == "hub-other:hello", "handler 收到 srcHubId 和载荽");
        }

        // 4) 注销后不再派发（防串号 / 防泄露给已销毁实体）
        global.OnEntityDestroy(alice.HubPlayer);
        await global.CallEntityEvent("hub-other", "hub-target", alice.HubPlayer.EntityId,
            new CallRpc { ProtoName = TestHubPingMethod, Content = new NotifyConnID { ConnId = "gone" }.ToByteString() });
        var popTask2 = h.Redis.PopList(string.Format(Consts.HubHubMq, "hub-target"), 8);
        var frames2 = popTask2 == null ? null : await popTask2;
        var ev2 = parser.ParseFrom(frames2![0]);
        global.OnEntityEvent(ev2.SrcHubId, ev2.EntityId, ev2.Event);
        await Task.Delay(150);
        lock (got)
        {
            Check(got.Count == 1, "OnEntityDestroy 后不再派发（计数仍是 1）");
        }
    }

    /// <summary>
    /// Actor 自调度语义：入队后无人 drain 也会自己跑起来，且保序 / 不并发 / 单个 job 炸了不影响后续。
    /// 这是「GateMsgHandle 的 PostTask 从来没被执行」那个 bug 的回归守卫。
    /// </summary>
    private static async Task ActorSelfScheduleProtocols()
    {
        Section("Actor 自调度：保序 / 隔离 / 重入");

        // 1) FIFO + 无人手动 drain 也会执行
        var actor = new Actor();
        var order = new List<int>();
        var gate = new SemaphoreSlim(0);
        for (var i = 0; i < 50; i++)
        {
            var n = i;
            actor.PostTask(() => { lock (order) order.Add(n); });
        }

        for (var i = 0; i < 50; i++)
        {
            await PostAndWaitAsync(actor, gate);
        }

        lock (order)
        {
            Check(order.Count == 50, "全部 50 个 job 都执行了（无人手动 drain）");
            Check(order.SequenceEqual(Enumerable.Range(0, 50)), "严格 FIFO");
        }

        Check(await TestClient.WaitForAsync(() => actor.PendingCount == 0), "drain 完毕后队列为空（不漏不残）");

        // 2) 单个 job 抛异常不中断队列
        var after = 0;
        actor.PostTask(() => throw new InvalidOperationException("boom"));
        actor.PostTask(() => Interlocked.Increment(ref after));
        Check(await TestClient.WaitForAsync(() => Volatile.Read(ref after) == 1),
            "前一个 job 抛异常后，后续 job 仍执行");

        // 3) 重入：job 内部再 Post 不死锁，且排在当前 job 之后
        var inner = 0;
        var outerDone = 0;
        actor.PostTask(() =>
        {
            actor.PostTask(() => Interlocked.Exchange(ref inner, Volatile.Read(ref outerDone) + 10));
            Interlocked.Increment(ref outerDone);
        });
        Check(await TestClient.WaitForAsync(() => Volatile.Read(ref inner) == 11),
            "嵌套 Post 执行了，且看到外层 job 的写入（串行，不并发）");

        // 4) 多线程并发 Post：每个 job 恰好一次，总数不丢
        var counter = 0;
        var done2 = new SemaphoreSlim(0);
        Parallel.For(0, 200, _ =>
        {
            actor.PostTask(() =>
            {
                Interlocked.Increment(ref counter);
                done2.Release();
            });
        });
        for (var i = 0; i < 200; i++)
        {
            await done2.WaitAsync(TimeSpan.FromSeconds(5));
        }

        Check(Volatile.Read(ref counter) == 200, "并发 Post 200 个，恰好执行 200 次");
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

    /// <summary>往 actor 里 post 一个会 release 信号量的 job：等到它 = 前面所有 job 都跑完了（保序）。</summary>
    private static async Task PostAndWaitAsync(Actor actor, SemaphoreSlim gate)
    {
        actor.PostTask(() => gate.Release());
        await gate.WaitAsync(TimeSpan.FromSeconds(5));
    }
}
