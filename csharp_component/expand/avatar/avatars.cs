using Abelkhan;
using Hub;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace avatar
{
    public interface IHostingData
    {
        public static virtual string type()
        {
            return string.Empty;
        }

        public static virtual IHostingData create()
        {
            return default(IHostingData);
        }

        public static virtual IHostingData load(BsonDocument data)
        {
            return default(IHostingData);
        }

        public abstract BsonDocument store();
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
            avatar.Datas[T.type()] = Data;
            avatar.set_dirty();
        }
    }

    public class Avatar
    {
        private Dictionary<string, IHostingData> dataDict = new();
        internal Dictionary<string, IHostingData> Datas
        {
            get
            {
                return dataDict;
            }
        }

        public long Guid;
        public string SDKUUID;
        public string ClientUUID;

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

                return bind_db_proxy.getCollection(AvatarMgr.opt.DBName, AvatarMgr.opt.DBCollection);

            }
        }
        private int _is_dirty = 0;
        internal void set_dirty()
        {
            var _dirty_state = Interlocked.Exchange(ref _is_dirty, 1);
            if (_dirty_state == 0)
            {
                Hub.Hub._timer.addticktime(AvatarMgr.opt.StoreDelayTime, (tick) =>
                {
                    Interlocked.Exchange(ref _is_dirty, 0);

                    var query = new DBQueryHelper();
                    query.condition("guid", Guid);
                    Collection.updataPersistedObject(query.query(), get_db_doc(), false, (result) =>
                    {
                        if (result != DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                        {
                            ; Log.Log.err("avatar set_dirty updataPersistedObject faild:{0}!", result.ToString());
                        }
                    });
                });
            }
        }

        public void add_hosting_data<T>(T data) where T : IHostingData
        {
            dataDict.Add(T.type(), data);
        }

        public IDataAgent<T> get_clone_hosting_data<T>() where T : IHostingData
        {
            try
            {
                if (dataDict.TryGetValue(T.type(), out var data))
                {
                    DataAgent<T> agent = new(this)
                    {
                        Data = (T)T.load(data.store())
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
                if (dataDict.TryGetValue(T.type(), out var data))
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
                doc.Add(name, data.ToBsonDocument());
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

    public static class AvatarMgr
    {
        private static Dictionary<string, Func<IHostingData> > hosting_data_create = new();
        private static Dictionary<string, Func<BsonDocument, IHostingData> > hosting_data_template = new();

        private static Dictionary<long, Avatar> avatar_guid = new();
        private static Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private static Dictionary<string, Avatar> avatar_client_uuid = new();

        internal static AvatarDataDBOptions opt = new();

        private static avatar_module _avatar_Module = new();
        private static avatar_caller _avatar_Caller = new();

        public static void init_data_db_opt(Action<AvatarDataDBOptions> configureOptions)
        {
            configureOptions.Invoke(opt);

            _avatar_Module.on_get_remote_avatar += _avatar_Module_on_get_remote_avatar;
        }

        private static void _avatar_Module_on_get_remote_avatar(long guid)
        {
            var rsp = _avatar_Module.rsp as avatar_get_remote_avatar_rsp;

            if (avatar_guid.TryGetValue(guid, out var _avatar))
            {
                rsp.rsp(_avatar.get_db_doc().ToBson());
            }
            else
            {
                rsp.err();
            }
        }

        public static void add_hosting<T>() where T : IHostingData
        {
            var type_str = T.type();
            hosting_data_create.Add(type_str, T.create);
            hosting_data_template.Add(type_str, T.load);
        }

        private static Task<Avatar> load_from_db(string sdk_uuid)
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
                    var avatar = new Avatar();
                    
                    var doc = value[0] as BsonDocument;
                    foreach (var (name, load) in hosting_data_template)
                    {
                        var sub_doc = doc.GetValue(name) as BsonDocument;
                        var ins = load(sub_doc);
                        avatar.add_hosting_data(ins);
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

        private static Task<long> get_guid()
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

        private static async Task<Avatar> create_avatar(string sdk_uuid)
        {
            var avatar = new Avatar();
            avatar.SDKUUID = sdk_uuid;
            avatar.Guid = await get_guid();
            foreach (var (name, create) in hosting_data_create)
            {
                var ins = create();
                avatar.add_hosting_data(ins);
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

        public static Task<Avatar> load_from_remote(long guid, string client_uuid, string other_hub)
        {
            var task = new TaskCompletionSource<Avatar>();

            _avatar_Caller.get_hub(other_hub).get_remote_avatar(guid).callBack((doc_bin) =>
            {

                var avatar = new Avatar();

                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(doc_bin);
                foreach (var (name, load) in hosting_data_template)
                {
                    var sub_doc = doc.GetValue(name) as BsonDocument;
                    var ins = load(sub_doc);
                    avatar.add_hosting_data(ins);
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

        public static async Task<Avatar> load_or_create(string sdk_uuid, string client_uuid)
        {
            var avatar = await load_from_db(sdk_uuid);
            if (avatar == null)
            {
                avatar = await create_avatar(sdk_uuid);
            }
            avatar.ClientUUID = client_uuid;

            avatar_guid[avatar.Guid] = avatar;

            avatar_sdk_uuid[sdk_uuid] = avatar;
            avatar_client_uuid[client_uuid] = avatar;

            return avatar;
        }

        public static Avatar get_current_avatar()
        {
            avatar_client_uuid.TryGetValue(Hub.Hub._gates.current_client_uuid, out var avatar);
            return avatar;
        }

        public static Avatar get_target_avatar(long guid)
        {
            avatar_guid.TryGetValue(guid, out var avatar);
            return avatar;
        }
    }
}
