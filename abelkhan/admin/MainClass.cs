using consts;
using core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

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

public class MainClass
{
    public void RunMain(string[] args)
    {
        var cfg = ReadConfig(args[0]);
        var data = new AdminDataService(cfg);

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

        app.MapGet("/health", () => Results.Ok("healthy"));

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
