using System.Collections.Concurrent;
using core;

namespace hub;

public abstract class Service(ConcurrentDictionary<string, Client> clients)
{
    private readonly Group _group = new();

    /// <summary>
    /// 全局邮件箱，由 <see cref="MainClass"/> 在启动时注入（与 hub 共用同一个 Redis）。
    /// 未启用邮件时保持为 null，客户端邮件请求会返回空列表。
    /// </summary>
    public GlobalMailBox? GlobalMailBox { get; set; }

    protected abstract Player CreateEntity(string gateName, string cliConnId, byte[] info);

    /// <summary>
    /// 游戏提供：该玩家当前的邮件领取条件。返回 null 表示该玩家暂不可领取邮件。
    /// level = 玩家等级，createDays = 建角天数。
    /// </summary>
    protected internal virtual (int Level, int CreateDays)? GetMailCondition(string userId) => null;

    public virtual async Task<Player> EchoQueryServiceEntity(string gateName, string cliConnId, byte[] info)
    {
        var player = CreateEntity(gateName, cliConnId, info);
        // 给玩家实体挂上「客户端请求领取邮件」协议的处理
        player.RegisterMailRequest(this);
        var cli = clients.AddOrUpdate(player.UserId, key => new Client(player.UserId, gateName, cliConnId), (key, old) => old);
        await player.CreateRemotePlayer(cli);
        await _group.CreateRemotePlayer(player);
        return player;
    }

    public virtual async Task<List<BaseEntity>?> EchoQueryServiceExt(List<(string, string, byte[])> infoData)
    {
        try
        {
            var lEntities = new List<BaseEntity>();
            foreach (var (gateName, cliConnId, info) in infoData)
            {
                lEntities.Add(await EchoQueryServiceEntity(gateName, cliConnId, info));
            }
            return lEntities;
        }
        catch (Exception ex)
        {
            Log.Error($"EchoQueryServiceExt err:{ex}");
        }
        return null;
    }
}