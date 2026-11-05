using consts;
using core;
using leaderboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using shop;

namespace admin;

public struct AdminConfig()
{
    public string AdminId { get; set; } = "admin";
    public string Ip { get; set; } = "0.0.0.0";
    public ushort Port { get; set; } = 8080;
    public string RedisUrl { get; set; } = string.Empty;
    public string RedisPwd { get; set; } = string.Empty;
    public string MongoUrl { get; set; } = string.Empty;
    public string MongoDb { get; set; } = "game";
    public string PlayerCollection { get; set; } = "player";
    public string PaymentCollection { get; set; } = "payment";
    public string MailCollection { get; set; } = "mail";
    public string GmCollection { get; set; } = "gm";
    public string PlayerRegisterField { get; set; } = "RegisterTime";
    public string PaymentTimeField { get; set; } = "PayTime";
    public string PaymentAmountField { get; set; } = "Amount";
    public string AdminToken { get; set; } = string.Empty;
    // 全局邮件列表最多保留多少封（LTRIM 上限），防止 list 无限增长
    public int MailGlobalMaxCount { get; set; } = 200;

    // 商城商品配置集合
    public string ShopProductCollection { get; set; } = "shop_product";

    /// <summary>
    /// 商城参数。KeyPrefix / CurrencyItemId 必须与 hub 的 HubConfig.Shop 一致，
    /// 否则后台“上架”写入的 Redis key 游戏服读不到。
    /// </summary>
    public ShopSettings Shop { get; set; } = new();

    /// <summary>
    /// 排行榜参数。KeyPrefix 必须与 hub 的 HubConfig.Leaderboard 一致，
    /// 否则后台重置的不是游戏服的榜。
    /// </summary>
    public LeaderboardSettings Leaderboard { get; set; } = new();

    /// <summary>监控配置。</summary>
    public MonitoringOptions Monitoring { get; set; } = new();
}

public record MailSendRequest(
    string Title,
    string Content,
    string[]? PlayerIds = null,
    int ExpireSeconds = 0,
    string Sender = "",
    MailAttachmentInput[]? Attachments = null,
    // 全局邮件的领取条件（0 = 不限制）
    int MinLevel = 0,
    int MaxLevel = 0,
    int MinCreateDays = 0,
    int MaxCreateDays = 0,
    bool PopUp = false,
    int ValidDays = 0);

public record GmCommandRequest(string Command, string? PlayerId = null, string Args = "");

public record LeaderboardResetRequest(string BoardId);

public class MainClass
{
    public void RunMain(string[] args)
    {
        var cfg = ReadConfig(args[0]);
        core.Metrics.Init(cfg.Monitoring);
        ServiceHealth.OnLoop();
        Tracing.Init(cfg.Monitoring);
        var monitoring = MonitoringHost.Start(cfg.Monitoring);
        var data = new AdminDataService(cfg);

        ServiceHealth.RegisterDependencies(async ct =>
        {
            await data.PingRedisAsync();
            return true;
        });

        var app = WebApplication.Create();
        // 让静态文件（wwwroot）始终从程序集输出目录加载，不受启动时工作目录影响
        var webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        app.Environment.ContentRootPath = AppContext.BaseDirectory;
        app.Environment.WebRootPath = webRoot;
        app.Environment.WebRootFileProvider = new PhysicalFileProvider(webRoot);

        // 简单的 Bearer Token 鉴权：配置了 AdminToken 时生效，保护 /api 前缀
        if (!string.IsNullOrEmpty(cfg.AdminToken))
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    var auth = context.Request.Headers["Authorization"].ToString();
                    if (auth != $"Bearer {cfg.AdminToken}")
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("unauthorized");
                        return;
                    }
                }
                await next();
            });
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapGet("/health", async () =>
        {
            var (ok, detail) = await ServiceHealth.CheckAsync();
            return ok
                ? Results.Ok(new { status = "healthy" })
                : Results.Json(new { status = "unhealthy", detail },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
        });

        // 监控大盘
        app.MapGet("/api/overview", () => Safe(async () =>
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var tomorrowStart = todayStart.AddDays(1);
            var yesterdayStart = todayStart.AddDays(-1);

            var onlinePlayers = await data.OnlineCount(Consts.AdminOnlineGateKey);
            var onlineConnections = await data.OnlineCount(Consts.AdminOnlineConnectionKey);
            var totalPlayers = await data.TotalPlayers();
            var todayRegisters = await data.CountPlayersByRegister(todayStart, tomorrowStart);
            var yesterdayRegisters = await data.CountPlayersByRegister(yesterdayStart, todayStart);
            var todayRecharge = await data.SumPayments(todayStart, tomorrowStart);
            var yesterdayRecharge = await data.SumPayments(yesterdayStart, todayStart);

            return Results.Json(new
            {
                onlinePlayers,
                onlineConnections,
                totalPlayers,
                todayRegisters,
                yesterdayRegisters,
                todayRecharge,
                yesterdayRecharge,
            });
        }));

        // 查询玩家
        app.MapGet("/api/players", (string? keyword = null, int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryPlayers(keyword ?? "", (page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items = items.Select(d => BsonUtil.ToJson(d)).ToList() });
        }));

        app.MapGet("/api/players/{id}", (string id) => Safe(async () =>
        {
            var doc = await data.GetPlayer(id);
            return doc == null
                ? Results.Json(new { error = "player not found" }, statusCode: StatusCodes.Status404NotFound)
                : Results.Json(BsonUtil.ToJson(doc));
        }));

        // 查询支付
        app.MapGet("/api/payments", (string? userId = null, string? start = null, string? end = null, int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryPayments(
                userId, ParseStart(start), ParseEnd(end), (page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items = items.Select(d => BsonUtil.ToJson(d)).ToList() });
        }));

        // 邮件
        app.MapGet("/api/mail", (int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryMail((page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items = items.Select(d => BsonUtil.ToJson(d)).ToList() });
        }));

        app.MapPost("/api/mail/send", (MailSendRequest req) => Safe(async () =>
        {
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.Json(new { error = "title/content required" }, statusCode: StatusCodes.Status400BadRequest);
            }
            var playerIds = (req.PlayerIds ?? Array.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct()
                .ToArray();
            var attachments = (req.Attachments ?? Array.Empty<MailAttachmentInput>())
                .Where(a => !string.IsNullOrWhiteSpace(a.ItemId))
                .Select(a => new MailAttachmentInput(a.ItemId.Trim(), Math.Max(0, a.Count)))
                .ToArray();
            var rule = new GlobalMailRule(
                req.MinLevel, req.MaxLevel,
                req.MinCreateDays, req.MaxCreateDays,
                req.PopUp, req.ValidDays);
            await data.SendMail(req.Title.Trim(), req.Content, req.Sender ?? "", attachments,
                playerIds, Math.Max(0, req.ExpireSeconds), rule);
            return Results.Json(new { ok = true, global = playerIds.Length == 0, count = playerIds.Length });
        }));

        // ---------------- 商城（氪金商品 + 玩家寄拍） ----------------

        app.MapGet("/api/shop/meta", () => Results.Json(new
        {
            currencyItemId = cfg.Shop.CurrencyItemId,
            marketCurrencyItemId = cfg.Shop.MarketCurrency,
            taxPercent = cfg.Shop.SaleTaxPercent,
            minDurationSeconds = cfg.Shop.MinListingDurationSeconds,
            maxDurationSeconds = cfg.Shop.MaxListingDurationSeconds,
            maxListingsPerPlayer = cfg.Shop.MaxListingsPerPlayer,
        }));

        app.MapGet("/api/shop/products", (string? keyword = null, string? status = null,
            int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryShopProducts(
                keyword, status, (page - 1) * pageSize, pageSize);
            return Results.Json(new
            {
                total,
                page,
                pageSize,
                items = items.Select(BsonUtil.ToJson).ToList(),
            });
        }));

        app.MapGet("/api/shop/products/{id}", (string id) => Safe(async () =>
        {
            var doc = await data.GetShopProduct(id);
            return doc == null
                ? Results.Json(new { error = "product not found" },
                    statusCode: StatusCodes.Status404NotFound)
                : Results.Json(BsonUtil.ToJson(doc));
        }));

        app.MapPost("/api/shop/products", (ShopProductInput req) => Safe(async () =>
        {
            var (ok, error, doc) = await data.CreateShopProduct(req);
            return ok
                ? Results.Json(new { ok = true, product = BsonUtil.ToJson(doc!) })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapPut("/api/shop/products/{id}", (string id, ShopProductInput req) => Safe(async () =>
        {
            var (ok, error, warning, doc) = await data.UpdateShopProduct(id, req);
            return ok
                ? Results.Json(new { ok = true, warning, product = BsonUtil.ToJson(doc!) })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapDelete("/api/shop/products/{id}", (string id, bool? force = null) => Safe(async () =>
        {
            var (ok, error) = await data.DeleteShopProduct(id, force ?? false);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapPost("/api/shop/products/{id}/publish", (string id, bool? force = null) => Safe(async () =>
        {
            var (ok, error, listing) = await data.PublishShopProduct(id, force ?? false);
            return ok
                ? Results.Json(new { ok = true, listing })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapPost("/api/shop/products/{id}/unpublish", (string id, bool? force = null) => Safe(async () =>
        {
            var (ok, error, warning) = await data.UnpublishShopProduct(id, force ?? false);
            return ok
                ? Results.Json(new { ok = true, warning })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapGet("/api/shop/listings", (string? source = null, string? status = null,
            int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryShopListings(
                source, status, (page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items });
        }));

        app.MapPost("/api/shop/listings/{id}/cancel", (string id) => Safe(async () =>
        {
            var (ok, error, listing) = await data.CancelShopListing(id);
            return ok
                ? Results.Json(new { ok = true, listing })
                : Results.Json(new { error }, statusCode: StatusCodes.Status400BadRequest);
        }));

        app.MapGet("/api/shop/orders", (int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryShopOrders((page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items });
        }));

        // ---------------- 排行榜 ----------------

        app.MapPost("/api/leaderboard/reset", (LeaderboardResetRequest req) => Safe(async () =>
        {
            var boardId = req.BoardId?.Trim() ?? "";
            if (string.IsNullOrEmpty(boardId))
            {
                return Results.Json(new { error = "boardId required" }, statusCode: StatusCodes.Status400BadRequest);
            }

            await data.ResetLeaderboard(boardId);
            return Results.Json(new { ok = true, boardId });
        }));

        // GM 面板
        app.MapGet("/api/gm/logs", (int page = 1, int pageSize = 20) => Safe(async () =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var (total, items) = await data.QueryGmLogs((page - 1) * pageSize, pageSize);
            return Results.Json(new { total, page, pageSize, items = items.Select(d => BsonUtil.ToJson(d)).ToList() });
        }));

        app.MapPost("/api/gm/command", (GmCommandRequest req) => Safe(async () =>
        {
            if (string.IsNullOrWhiteSpace(req.Command))
            {
                return Results.Json(new { error = "command required" }, statusCode: StatusCodes.Status400BadRequest);
            }
            var command = req.Command.Trim();
            var args = req.Args ?? "";

            // 带 PlayerId：定向到该玩家的队列，由游戏服消费
            if (!string.IsNullOrWhiteSpace(req.PlayerId))
            {
                var playerId = req.PlayerId.Trim();
                await data.RecordGmCommand(command, playerId, args, "queued");
                await data.PushPlayerGmCommand(command, playerId, args);
                return Results.Json(new { ok = true, mode = "queued", playerId });
            }

            // 无 PlayerId：admin 进程本地执行
            var result = await AdminGmHandlers.RunAsync(command, args);
            await data.RecordGmCommand(command, "", args, result);
            return Results.Json(new { ok = true, mode = "local", result });
        }));

        app.MapFallbackToFile("index.html");

        var url = $"http://{cfg.Ip}:{cfg.Port}";
        Log.Info("admin: shop key prefix:{0} currency:{1} (must match hub)",
            cfg.Shop.KeyPrefix, cfg.Shop.CurrencyItemId);
        Log.Info("admin: leaderboard key prefix:{0} (must match hub)",
            cfg.Leaderboard.KeyPrefix);
        Log.Info("admin: {0} listening on {1}", cfg.AdminId, url);
        app.Run(url);
    }

    private static async Task<IResult> Safe(Func<Task<IResult>> fn)
    {
        try
        {
            return await fn();
        }
        catch (Exception ex)
        {
            Log.Error("admin api error: {0}", ex);
            return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static DateTime? ParseStart(string? s)
    {
        if (string.IsNullOrWhiteSpace(s) || !DateTime.TryParse(s, out var d))
        {
            return null;
        }
        return d.ToUniversalTime();
    }

    private static DateTime? ParseEnd(string? s)
    {
        if (string.IsNullOrWhiteSpace(s) || !DateTime.TryParse(s, out var d))
        {
            return null;
        }
        var dt = d.ToUniversalTime();
        // 只给了日期（无时间部分）时，结束边界算到当天结束（次日 0 点）
        if (dt.TimeOfDay == TimeSpan.Zero)
        {
            dt = dt.AddDays(1);
        }
        return dt;
    }

    private static AdminConfig ReadConfig(string path)
    {
        using var fs = File.OpenRead(path);
        byte[] data = new byte[fs.Length];
        int offset = 0;
        int remaining = data.Length;
        while (remaining > 0)
        {
            int read = fs.Read(data, offset, remaining);
            if (read <= 0)
            {
                throw new EndOfStreamException($"file read at:{read} failed");
            }
            remaining -= read;
            offset += read;
        }
        var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<AdminConfig>(
            System.Text.Encoding.Default.GetString(data));
        return cfg;
    }

    static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        Log.Error($"admin not handle exception:{ex}");
    }

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        new MainClass().RunMain(args);
    }
}
