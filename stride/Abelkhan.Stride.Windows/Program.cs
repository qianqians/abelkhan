using Abelkhan.Stride;
using Abelkhan.Stride.Net;

// Stride 的 Windows 启动器：只负责按命令行/环境变量拼好接入参数，然后跑 Game。
//
//   Abelkhan.Stride.Windows.exe --wss=wss://127.0.0.1:9001/ --service=test_service --user=u1
//
// 也可以用环境变量：ABLEKHAN_WSS / ABELKHAN_SERVICE / ABELKHAN_USER / ABELKHAN_CHANNEL / ABELKHAN_AUTOCHAT

var options = new AbelkhanClientOptions
{
    Wss = Env("ABLEKHAN_WSS") ?? Arg("wss") ?? "wss://127.0.0.1:9001/",
    ServiceName = Env("ABLEKHAN_SERVICE") ?? Arg("service") ?? string.Empty,
    UserId = Env("ABLEKHAN_USER") ?? Arg("user") ?? string.Empty,
    ChatChannel = Env("ABLEKHAN_CHANNEL") ?? Arg("channel") ?? "world",
};

if (double.TryParse(Env("ABLEKHAN_AUTOCHAT") ?? Arg("autochat"), out var interval))
{
    options.AutoChatIntervalSeconds = interval;
}

Console.WriteLine($"[abelkhan] wss={options.Wss} service='{options.ServiceName}' user='{options.UserId}'");

using var game = new AbelkhanGame(options);
game.Run();

static string? Arg(string name)
{
    var prefix = "--" + name + "=";
    return Environment.GetCommandLineArgs().FirstOrDefault(a => a.StartsWith(prefix, StringComparison.Ordinal))?[prefix.Length..];
}

static string? Env(string name)
{
    var value = Environment.GetEnvironmentVariable(name);
    return string.IsNullOrWhiteSpace(value) ? null : value;
}
