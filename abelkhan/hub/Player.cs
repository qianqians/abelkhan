using System.Collections.Concurrent;
using consts;
using core;
using engine;
using hub.inventory;
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

    private IBag? _bag;

    /// <summary>默认背包容量。</summary>
    public const int DefaultBagCapacity = 100;

    /// <summary>
    /// 物品配置表。建议在首次访问 <see cref="Bag"/> 之前设置（<see cref="Service"/> 创建玩家后会自动注入）。
    /// 为 null 时允许任意 item_id，按无上限堆叠处理。
    /// </summary>
    public IItemCatalog? ItemCatalog { get; set; }

    /// <summary>背包容量，首次访问 <see cref="Bag"/> 时生效。</summary>
    public int BagCapacity { get; set; } = DefaultBagCapacity;

    /// <summary>
    /// 玩家背包。默认使用固定长度数组实现（<see cref="ArrayBag"/>）；
    /// 读档时可用 <see cref="RestoreBag"/> 换成从存档恢复的实例。
    /// </summary>
    public IBag Bag
    {
        get
        {
            if (_bag == null)
            {
                var bag = CreateBag();
                bag.Changed += OnBagChanged;
                _bag = bag;
            }

            return _bag!;
        }
    }

    /// <summary>背包发生变化时触发，游戏可在这里把变更同步给客户端。</summary>
    public event Action<IReadOnlyList<BagSlotChange>>? BagChanged;

    /// <summary>创建背包；需要换存储实现（例如分页背包）时重写。</summary>
    protected virtual IBag CreateBag() => new ArrayBag(BagCapacity, ItemCatalog);

    /// <summary>用已有背包替换当前背包（读档 / 数据迁移时使用）。</summary>
    public void RestoreBag(IBag bag)
    {
        ArgumentNullException.ThrowIfNull(bag);
        if (ReferenceEquals(_bag, bag))
        {
            return;
        }

        if (_bag != null)
        {
            _bag.Changed -= OnBagChanged;
        }

        _bag = bag;
        _bag.Changed += OnBagChanged;
    }

    /// <summary>把背包序列化成可放进 FullInfo() / ClientInfo() 的 BSON 文档。</summary>
    protected BsonDocument BagInfo() => BagBson.ToBson(Bag);

    /// <summary>
    /// 根据 item_id / 数量创建物品。游戏可以重写以附加绑定、品质、过期时间等实例属性。
    /// 返回 null 表示该附件无法创建（视为配置错误，邮件领取会整体失败）。
    /// </summary>
    protected virtual IItem? CreateItem(string itemId, long count)
    {
        if (ItemCatalog is { } catalog)
        {
            return catalog.TryGet(itemId, out var definition) ? definition.CreateStack(count) : null;
        }

        return new ItemStack(itemId, count);
    }

    private void OnBagChanged(IReadOnlyList<BagSlotChange> changes) => BagChanged?.Invoke(changes);

    /// <summary>
    /// 把邮件附件放进背包。<see cref="IBag.AddRange"/> 是原子的：要么全部放入，要么背包保持原样。
    /// 数量 &lt;= 0 的附件会被跳过。
    /// </summary>
    protected BagResult GrantMailAttachments(GlobalMail mail)
    {
        var items = new List<IItem>(mail.Attachments.Count);
        foreach (var attachment in mail.Attachments)
        {
            if (attachment.Count <= 0)
            {
                continue;
            }

            IItem? item;
            try
            {
                item = CreateItem(attachment.ItemId, attachment.Count);
            }
            catch (Exception e)
            {
                return BagResult.Fail(BagError.InvalidArgument,
                    $"邮件附件 {attachment.ItemId} 配置非法: {e.Message}");
            }

            if (item == null)
            {
                return BagResult.Fail(BagError.UnknownItem,
                    $"邮件附件 {attachment.ItemId} 无法创建");
            }

            items.Add(item);
        }

        return items.Count == 0 ? BagResult.Success() : Bag.AddRange(items);
    }

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
    ///   mail_id 非空  -> 领取指定邮件（标记已领取、附件原子放入背包并返回）
    /// 条件（等级 / 建角天数）由游戏的 Service.GetMailCondition 提供。
    /// 附件发放失败（背包满 / 配置错误）时会回滚已领取标记，避免玩家丢邮件。
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

                // 附件发放失败（背包满 / 附件配置错误）时回滚认领标记，
                // 否则玩家会因为一次失败的领取永久丢掉这封邮件。
                BagResult grant = default;
                try
                {
                    grant = GrantMailAttachments(mail);
                }
                catch (Exception e)
                {
                    await box.UnmarkClaimed(UserId, req.MailId);
                    Log.Error("claim mail:{0} user:{1} grant items failed! {2}", req.MailId, UserId, e);
                    return Result<ClientRequestMailResult, string>.Err("mail claim failed");
                }

                if (grant.Failed)
                {
                    await box.UnmarkClaimed(UserId, req.MailId);
                    return Result<ClientRequestMailResult, string>.Err($"mail claim failed: {grant.Message}");
                }

                result.Mails.Add(ToMailInfo(mail));
            }

            return Result<ClientRequestMailResult, string>.Ok(result);
        });
    }

    private static MailInfo ToMailInfo(GlobalMail mail)
    {
        var info = new MailInfo
        {
            MailId = mail.MailId,
            Title = mail.Title,
            Content = mail.Content,
            Sender = mail.Sender,
            CreateTime = (ulong)mail.CreateTime,
            PopUp = mail.PopUp,
        };

        foreach (var attachment in mail.Attachments)
        {
            info.Attachments.Add(new MailAttachment
            {
                ItemId = attachment.ItemId,
                Count = (uint)Math.Max(0, attachment.Count),
            });
        }

        return info;
    }
}