using System.Collections.Concurrent;
using consts;
using core;
using engine;
using MongoDB.Bson;

namespace hub;

public abstract class Player(
    string entityId,
    string entityType,
    string userId,
    RedisHandle redis,
    MongoProxy mongo,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) : BaseEntity(entityId, entityType, redis, clients, gates)
{
    public string UserId { get; } = userId;

    private readonly RedisHandle _redis = redis;

    private ConcurrentDictionary<string, Client> _clients = clients;
    private ConcurrentDictionary<string, GateNetwork> _gates = gates;

    protected abstract string MongoDbName();
    protected abstract string MongoCollectionName();
    
    public abstract override BsonDocument FullInfo();
    public abstract override BsonDocument ClientInfo();

    public async void Save()
    {
        try
        {
            var query = new BsonDocument()
            {
                { "EntityType", EntityType },
                { "EntityId", EntityId },
            };
            if (!await mongo.Update(MongoDbName(), MongoCollectionName(), query.ToBson(), FullInfo().ToBson(), true))
            {
                Log.Error($"Player entity:{EntityType} id:{EntityId} user:{UserId} save failed!");
            }
        }
        catch (Exception e)
        {
            Log.Error($"Player entity:{EntityType} id:{EntityId} user:{UserId} save error!", e);
        }
    }

    public new async Task CreateRemotePlayer(Client client)
    {
        await base.CreateRemotePlayer(client);
    }

    /// <summary>
    /// 注册「客户端请求领取邮件」协议：
    ///   mail_id 为空  -> 返回当前可领取的全局邮件（不标记领取）
    ///   mail_id 非空  -> 领取指定邮件（标记已领取并返回）
    /// 条件（等级 / 建角天数）由游戏的 Service.GetMailCondition 提供。
    /// </summary>
    internal void RegisterMailRequest(Service service)
    {
        RegisterRequest<ClientRequestMail, ClientRequestMailResult>(Consts.ClientRequestMail, async req =>
        {
            var result = new ClientRequestMailResult();
            var box = service.GlobalMailBox;
            if (box == null)
            {
                return Result<ClientRequestMailResult, string>.Ok(result);
            }

            var cond = service.GetMailCondition(UserId);
            if (cond == null)
            {
                return Result<ClientRequestMailResult, string>.Ok(result);
            }

            if (string.IsNullOrEmpty(req.MailId))
            {
                var mails = await box.GetUnclaimed(UserId, cond.Value.Level, cond.Value.CreateDays);
                foreach (var mail in mails)
                {
                    result.Mails.Add(ToMailInfo(mail));
                }
            }
            else
            {
                var mail = await box.GetClaimable(UserId, cond.Value.Level, cond.Value.CreateDays, req.MailId);
                if (mail == null)
                {
                    return Result<ClientRequestMailResult, string>.Err("mail not claimable");
                }

                // SADD 原子判定：并发/重复领取时只有第一次成功
                var claimedNow = await box.MarkClaimed(UserId, req.MailId);
                if (!claimedNow)
                {
                    return Result<ClientRequestMailResult, string>.Err("mail already claimed");
                }

                result.Mails.Add(ToMailInfo(mail));
            }

            return Result<ClientRequestMailResult, string>.Ok(result);
        });
    }

    private static MailInfo ToMailInfo(GlobalMail mail) => new()
    {
        MailId = mail.MailId,
        Title = mail.Title,
        Content = mail.Content,
        CreateTime = (ulong)mail.CreateTime,
        PopUp = mail.PopUp,
    };
}