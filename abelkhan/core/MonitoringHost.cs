using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace core;

/// <summary>
/// 独立的监控监听：只在 <see cref="MonitoringOptions.PrometheusHost"/>:
/// <see cref="MonitoringOptions.PrometheusPort"/> 上提供 <c>/metrics</c> 与 <c>/health</c>。
///
/// 为什么不复用现有的健康检查端口：那个端口绑的是 <c>cfg.Ip</c>（常常是 0.0.0.0），
/// 把 <c>/metrics</c> 挂上去等于把内部指标暴露到公网。这里默认绑回环，内网/同机采集就够。
/// </summary>
public sealed class MonitoringHost
{
    private readonly WebApplication _app;
    private readonly string _url;

    private MonitoringHost(WebApplication app, string url)
    {
        _app = app;
        _url = url;
    }

    /// <summary>按配置启动；未启用返回 null。</summary>
    public static MonitoringHost? Start(MonitoringOptions options, int maxIdleMs = 10_000)
    {
        if (!options.PrometheusEnabled)
        {
            return null;
        }

        try
        {
            var app = WebApplication.Create();
            app.MapGet("/metrics", async (HttpContext ctx) =>
            {
                ctx.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";
                await Metrics.ExportAsync(ctx.Response.Body);
            });

            // 监控端口上也放一个真健康检查，方便 Prometheus 之外的工具探活
            app.MapGet("/health", async () =>
            {
                var (ok, detail) = await ServiceHealth.CheckAsync(maxIdleMs);
                return ok ? Results.Ok(new { status = "healthy" }) : Results.Json(
                    new { status = "unhealthy", detail }, statusCode: StatusCodes.Status503ServiceUnavailable);
            });

            var url = $"http://{options.PrometheusHost}:{options.PrometheusPort}";
            _ = app.RunAsync(url);
            Log.Info("monitoring: /metrics on {0} (service={1} node={2})",
                url, options.ServiceName, options.NodeId);
            return new MonitoringHost(app, url);
        }
        catch (Exception e)
        {
            Log.Error("monitoring: start failed: {0}", e.Message);
            return null;
        }
    }

    public string Url => _url;

    public Task StopAsync()
    {
        try
        {
            return _app.StopAsync();
        }
        catch
        {
            return Task.CompletedTask;
        }
    }
}
