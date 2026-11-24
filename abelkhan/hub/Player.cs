using System.Collections.Concurrent;
using consts;
using core;
using engine;
using hub.inventory;
using MongoDB.Bson;

namespace hub;

public abstract partial class Player(
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

    protected abstract string MongoDbName();
    protected abstract string MongoCollectionName();
    
    public abstract override BsonDocument FullInfo();
    public abstract override BsonDocument ClientInfo();

    public virtual async void Save()
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
}
