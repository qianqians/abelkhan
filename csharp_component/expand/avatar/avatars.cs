using Abelkhan;
using Hub;
using MongoDB.Bson;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace avatar
{
    public interface IHostingData
    {
        public static virtual string Type()
        {
            return string.Empty;
        }

        public static virtual IHostingData Create()
        {
            return default(IHostingData);
        }

        public static virtual IHostingData Load(BsonDocument data)
        {
            return default(IHostingData);
        }

        public abstract BsonDocument Store();
    }

    public abstract class IDataAgent<T> where T : IHostingData
    {
        public T Data { get; set; }

        public abstract void write_back();
    }

    internal class DataAgent<T> : IDataAgent<T> where T : IHostingData
    {
        private readonly Avatar avatar;

        internal DataAgent(Avatar _avatar)
        {
            avatar = _avatar;
        }

        public override void write_back()
        {
            avatar.Datas[T.Type()] = Data;
            avatar.set_dirty();
        }
    }

    public class Avatar
    {
        private AvatarMgr avatarMgr;
        internal Avatar(AvatarMgr mgr)
        {
            avatarMgr = mgr;
        }

        private Dictionary<string, IHostingData> dataDict = new();
        internal Dictionary<string, IHostingData> Datas
        {
            get
            {
                return dataDict;
            }
        }

        public long LastActiveTime = Timerservice.Tick;
        public long Guid;
        public string SDKUUID;

        private string client_uuid;
        public string ClientUUID
        {
            set
            {
                avatarMgr.bind_new_client_uuid(this, value);
                client_uuid = value;
            }
            get
            {
                return client_uuid;
            }
        }

        private string bind_db_proxy_name = null;
        private DBProxyProxy.Collection Collection
        {
            get
            {
                DBProxyProxy bind_db_proxy;
                do
                {
                    if (string.IsNullOrEmpty(bind_db_proxy_name))
                    {
                        bind_db_proxy = Hub.Hub.get_random_dbproxyproxy();
                        bind_db_proxy_name = bind_db_proxy.db_name;
                        break;
                    }

                    bind_db_proxy = Hub.Hub.get_dbproxy(bind_db_proxy_name);
                    if (bind_db_proxy == null)
                    {
                        bind_db_proxy = Hub.Hub.get_random_dbproxyproxy();
                        bind_db_proxy_name = bind_db_proxy.db_name;
                    }
                } while (false);

                return bind_db_proxy.getCollection(avatarMgr.opt.DBName, avatarMgr.opt.DBCollection);

            }
        }
        private int _is_dirty = 0;
        internal void set_dirty()
        {
            var _dirty_state = Interlocked.Exchange(ref _is_dirty, 1);
            if (_dirty_state == 0)
            {
                Hub.Hub._timer.addticktime(avatarMgr.opt.StoreDelayTime, (tick) =>
                {
                    Interlocked.Exchange(ref _is_dirty, 0);

                    var query = new DBQueryHelper();
                    query.condition("guid", Guid);
                    var update = new UpdateDataHelper();
                    update.set(get_db_doc());
                    Collection.updataPersistedObject(query.query(), update.data(), false, (result) =>
                    {
                        if (result != DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                        {
                            Log.Log.err("avatar set_dirty updataPersistedObject faild:{0}!", result.ToString());
                        }
                    });
                });
            }
        }

        public event Action onDestory;
        internal void on_destory()
        {
            onDestory?.Invoke();
        }

        public Task<bool> transfer(string new_sdk_uuid)
        {
            var task = new TaskCompletionSource<bool>();

            var old_sdk_uuid = SDKUUID;
            SDKUUID = new_sdk_uuid;

            if (avatarMgr.transfer(old_sdk_uuid, new_sdk_uuid, this))
            {
                var query = new DBQueryHelper();
                query.condition("guid", Guid);
                var update = new UpdateDataHelper();
                update.set(get_db_doc());
                Collection.updataPersistedObject(query.query(), update.data(), false, (result) =>
                {
                    if (result != DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                    {
                        Log.Log.err("avatar set_dirty updataPersistedObject faild:{0}!", result.ToString());
                        task.SetResult(false);
                    }
                    else
                    {
                        task.SetResult(true);
                    }
                });
            }
            else
            {
                task.SetResult(false);
            }

            return task.Task;
        }

        public void add_hosting_data<T>(string type_name, T data) where T : IHostingData
        {
            dataDict.Add(type_name, data);
        }

        public IDataAgent<T> get_clone_hosting_data<T>() where T : IHostingData
        {
            try
            {
                if (dataDict.TryGetValue(T.Type(), out var data))
                {
                    DataAgent<T> agent = new(this)
                    {
                        Data = (T)T.Load(data.Store())
                    };
                    return agent;
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("avatar get_clone_hosting_data ex:{0}!", ex);
            }
            return default(IDataAgent<T>);
        }

        public IDataAgent<T> get_real_hosting_data<T>() where T : IHostingData
        {
            try
            {
                if (dataDict.TryGetValue(T.Type(), out var data))
                {
                    DataAgent<T> agent = new(this)
                    {
                        Data = (T)data
                    };
                    return agent;
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("avatar get_real_hosting_data ex:{0}!", ex);
            }
            finally
            {
                set_dirty();
            }
            return default(IDataAgent<T>);
        }

        internal BsonDocument get_db_doc()
        {
            var doc = new BsonDocument
            {
                { "guid", Guid },
                { "sdk_uuid", SDKUUID }
            };
            foreach (var (name, data) in dataDict)
            {
                doc.Add(name, data.Store());
            }

            return doc;
        }
    }

    public class AvatarDataDBOptions
    {
        public string DBName { get; set; }
        public string DBCollection { get; set; }
        public string GuidCollection { get; set; }
        public int StoreDelayTime { get; set; } 
    }

    public class AvtarLoadFromDBError : System.Exception
    {
        public string Error;
        public AvtarLoadFromDBError(string err)
        {
            Error = err;        
        }
    }

    public class AvatarMgr
    {
        private Dictionary<string, Func<IHostingData> > hosting_data_create = new();
        private Dictionary<string, Func<BsonDocument, IHostingData> > hosting_data_template = new();

        private Dictionary<long, Avatar> avatar_guid = new();
        public List<Avatar> Avatars
        {
            get
            {
                return avatar_guid.Values.ToList();
            }
        }
        private Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private Dictionary<string, Avatar> avatar_client_uuid = new();

        internal AvatarDataDBOptions opt = new();

        private static bool is_register = false;
        private static avatar_module _avatar_Module = new();
        private static avatar_caller _avatar_Caller = new();

        public int Count
        {
            get
            {
                return avatar_guid.Count;
            }
        }

        public void init()
        {
            Hub.Hub._timer.addticktime(5 * 60 * 1000, tick_clear_timeout_avatar);

            lock (_avatar_Module)
            {
                if (!is_register)
                {
                    _avatar_Module.on_get_remote_avatar += _avatar_Module_on_get_remote_avatar;
                    _avatar_Module.on_migrate_avatar += _avatar_Module_on_migrate_avatar;
                    is_register = true;
                }
            }

            Hub.Hub.on_migrate_client += Hub_on_migrate_client;
        }

        private async Task Hub_on_migrate_client(string client_uuid, string src_hub)
        {
            await migrate_from_remote(client_uuid, src_hub);
        }

        public void init_data_db_opt(Action<AvatarDataDBOptions> configureOptions)
        {
            configureOptions.Invoke(opt);
        }

        private void _avatar_Module_on_get_remote_avatar(string client_uuid)
        {
            var rsp = _avatar_Module.rsp as avatar_get_remote_avatar_rsp;

            if (avatar_client_uuid.TryGetValue(client_uuid, out var _avatar))
            {
                rsp.rsp(_avatar.Guid, _avatar.get_db_doc().ToBson());
            }
            else
            {
                rsp.err();
            }
        }

        private void _avatar_Module_on_migrate_avatar(string client_uuid)
        {
            var rsp = _avatar_Module.rsp as avatar_migrate_avatar_rsp;

            if (avatar_client_uuid.TryGetValue(client_uuid, out var _avatar))
            {
                rsp.rsp(_avatar.Guid, _avatar.get_db_doc().ToBson());

                avatar_client_uuid.Remove(_avatar.ClientUUID);
                avatar_sdk_uuid.Remove(_avatar.SDKUUID);
                avatar_guid.Remove(_avatar.Guid);
            }
            else
            {
                rsp.err();
            }
        }

        public void add_hosting<T>() where T : IHostingData
        {
            var type_str = T.Type();
            Log.Log.trace("add_hosting type:{0}", type_str);
            hosting_data_create.Add(type_str, T.Create);
            hosting_data_template.Add(type_str, T.Load);
        }

        private void tick_clear_timeout_avatar(long tick_time)
        {
            List<Avatar> timeout_avatar = new();
            foreach (var it in avatar_guid)
            {
                if ((it.Value.LastActiveTime + 30 * 60 * 1000) < Timerservice.Tick)
                {
                    timeout_avatar.Add(it.Value);
                }
            }
            foreach (var _avatar in timeout_avatar)
            {
                avatar_client_uuid.Remove(_avatar.ClientUUID);
                avatar_sdk_uuid.Remove(_avatar.SDKUUID);
                avatar_guid.Remove(_avatar.Guid);

                _avatar.on_destory();
            }

            Hub.Hub._timer.addticktime(5 * 60 * 1000, tick_clear_timeout_avatar);
        }

        internal bool transfer(string old_sdk_uuid, string new_sdk_uuid, Avatar avatar)
        {
            if (avatar_sdk_uuid.Remove(old_sdk_uuid))
            {
                avatar_sdk_uuid[new_sdk_uuid] = avatar;
                return true;
            }
            return false;
        }

        internal void bind_new_client_uuid(Avatar avatar, string new_client_uuid)
        {
            if (!string.IsNullOrEmpty(avatar.ClientUUID))
            {
                avatar_client_uuid.Remove(avatar.ClientUUID);
            }
            avatar_client_uuid[new_client_uuid] = avatar;
        }

        public Task<Avatar> load_from_db(string sdk_uuid)
        {
            var task = new TaskCompletionSource<Avatar>();

            var query = new DBQueryHelper();
            query.condition("sdk_uuid", sdk_uuid);
            Hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).getObjectInfo(query.query(), (value) => {
                if (value.Count > 1)
                {
                    throw new AvtarLoadFromDBError($"repeated sdk_uuid:{sdk_uuid}");
                }
                else if (value.Count == 1)
                {
                    var avatar = new Avatar(this);
                    
                    var doc = value[0] as BsonDocument;
                    foreach (var (name, load) in hosting_data_template)
                    {
                        var sub_doc = doc.GetValue(name) as BsonDocument;
                        var ins = load(sub_doc);
                        avatar.add_hosting_data(name, ins);
                    }
                    avatar.SDKUUID = sdk_uuid;
                    avatar.Guid = doc.GetValue("guid").AsInt64;

                    task.SetResult(avatar);
                }
                else
                {
                    task.SetResult(null);
                }
            }, () => { });

            return task.Task;
        }

        private Task<long> get_guid()
        {
            var task = new TaskCompletionSource<long>();

            Hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.GuidCollection).getGuid((result, guid) =>
            {
                if (result == Hub.DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    task.SetResult(guid);
                }
            });

            return task.Task;
        }

        public async Task<Avatar> create_avatar(string sdk_uuid)
        {
            var avatar = new Avatar(this);
            avatar.SDKUUID = sdk_uuid;
            avatar.Guid = await get_guid();
            foreach (var (name, create) in hosting_data_create)
            {
                var ins = create();
                avatar.add_hosting_data(name, ins);
            }
            Hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).createPersistedObject(avatar.get_db_doc(), (result) =>
            {
                if (result != Hub.DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    Log.Log.err("db create avatar faild:{0}", result.ToString());
                }
            });
            return avatar;
        }

        public Task<Avatar> load_from_remote(string client_uuid, string other_hub)
        {
            var task = new TaskCompletionSource<Avatar>();

            _avatar_Caller.get_hub(other_hub).get_remote_avatar(client_uuid).callBack((guid, doc_bin) =>
            {
                var avatar = new Avatar(this);

                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(doc_bin);
                foreach (var (name, load) in hosting_data_template)
                {
                    var sub_doc = doc.GetValue(name) as BsonDocument;
                    var ins = load(sub_doc);
                    avatar.add_hosting_data(name, ins);
                }
                avatar.SDKUUID = doc.GetValue("sdk_uuid").AsString;
                avatar.Guid = guid;

                avatar_guid[guid] = avatar;

                avatar_sdk_uuid[avatar.SDKUUID] = avatar;
                avatar_client_uuid[client_uuid] = avatar;

                task.SetResult(avatar);

            }, () =>
            {
                Log.Log.err("load from remote error!");
                task.SetResult(null);

            }).timeout(3000, () =>
            {
                Log.Log.err("load from remote timeout!");
                task.SetResult(null);
            });

            return task.Task;
        }

        public Task<Avatar> migrate_from_remote(string client_uuid, string other_hub)
        {
            var task = new TaskCompletionSource<Avatar>();

            _avatar_Caller.get_hub(other_hub).migrate_avatar(client_uuid).callBack((guid, doc_bin) =>
            {
                var avatar = new Avatar(this);

                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(doc_bin);
                foreach (var (name, load) in hosting_data_template)
                {
                    var sub_doc = doc.GetValue(name) as BsonDocument;
                    var ins = load(sub_doc);
                    avatar.add_hosting_data(name, ins);
                }
                avatar.SDKUUID = doc.GetValue("sdk_uuid").AsString;
                avatar.Guid = guid;

                avatar_guid[guid] = avatar;

                avatar_sdk_uuid[avatar.SDKUUID] = avatar;
                avatar_client_uuid[client_uuid] = avatar;

                task.SetResult(avatar);

            }, () =>
            {
                Log.Log.err("load from remote error!");
                task.SetResult(null);

            }).timeout(3000, () =>
            {
                Log.Log.err("load from remote timeout!");
                task.SetResult(null);
            });

            return task.Task;
        }

        public async Task<Avatar> load_or_create(string sdk_uuid, string client_uuid)
        {
            if(!avatar_sdk_uuid.TryGetValue(sdk_uuid, out var avatar))
            {
                avatar = await load_from_db(sdk_uuid);
                if (avatar == null)
                {
                    avatar = await create_avatar(sdk_uuid);
                }
            }

            bind_avatar(avatar, client_uuid);

            return avatar;
        }

        public void bind_avatar(Avatar avatar, string client_uuid)
        {
            avatar_guid[avatar.Guid] = avatar;
            avatar_sdk_uuid[avatar.SDKUUID] = avatar;

            if (!string.IsNullOrEmpty(client_uuid))
            {
                avatar.ClientUUID = client_uuid;
                avatar_client_uuid[client_uuid] = avatar;
            }
        }

        public Avatar get_current_avatar()
        {
            avatar_client_uuid.TryGetValue(Hub.Hub._gates.current_client_uuid, out var avatar);
            if (avatar != null)
            {
                avatar.LastActiveTime = Timerservice.Tick;
            }
            return avatar;
        }

        public Avatar get_avatar(string client_uuid)
        {
            avatar_client_uuid.TryGetValue(client_uuid, out var avatar);
            if (avatar != null)
            {
                avatar.LastActiveTime = Timerservice.Tick;
            }
            return avatar;
        }

        public Avatar get_target_avatar(long guid)
        {
            avatar_guid.TryGetValue(guid, out var avatar);
            if (avatar != null)
            {
                avatar.LastActiveTime = Timerservice.Tick;
            }
            return avatar;
        }

        public Avatar get_target_avatar(string sdk_uuid)
        {
            avatar_sdk_uuid.TryGetValue(sdk_uuid, out var avatar);
            if (avatar != null)
            {
                avatar.LastActiveTime = Timerservice.Tick;
            }
            return avatar;
        }
    }
}
