using abelkhan;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
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

        public BsonDocument store();
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
        public string ClientUUID;

        public void add_hosting_data<T>(T data) where T : IHostingData
        {
            dataDict.Add(T.type(), data);
        }

        public IDataAgent<T> get_clone_hosting_data<T>() where T : IHostingData
        {
            if (dataDict.TryGetValue(T.type(), out var data))
            {
                DataAgent<T> agent = new(this);
                agent.Data = (T)T.load(data.store());
                return agent;
            }
            return default(IDataAgent<T>);
        }

        public IDataAgent<T> get_real_hosting_data<T>() where T : IHostingData
        {
            if (dataDict.TryGetValue(T.type(), out var data))
            {
                DataAgent<T> agent = new(this);
                agent.Data = (T)data;
                return agent;
            }
            return default(IDataAgent<T>);
        }

        internal BsonDocument get_db_doc()
        {
            var doc = new BsonDocument
            {
                { "guid", Guid }
            };
            foreach (var (name, data) in dataDict)
            {
                doc.Add(name, data.ToBsonDocument());
            }

            return doc;
        }
    }

    public class AvatarDataOriginalOptions
    {
        public string DBName { get; set; }
        public string DBCollection { get; set; }
        public string GuidCollection { get; set; }
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

        private static AvatarDataOriginalOptions opt = new();

        private static avatar_module _avatar_Module = new();
        private static avatar_caller _avatar_Caller = new();

        public static void init_data_original(Action<AvatarDataOriginalOptions> configureOptions)
        {
            configureOptions.Invoke(opt);

            _avatar_Module.on_get_remote_avatar += _avatar_Module_on_get_remote_avatar;
        }

        private static void _avatar_Module_on_get_remote_avatar(string sdk_uuid)
        {
            var rsp = _avatar_Module.rsp as avatar_get_remote_avatar_rsp;

            if (avatar_sdk_uuid.TryGetValue(sdk_uuid, out var _avatar))
            {
                rsp.rsp(_avatar.ToBson());
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
            hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).getObjectInfo(query.query(), (value) => {
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

            hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.GuidCollection).getGuid((result, guid) =>
            {
                if (result == hub.DBproxyproxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    task.SetResult(guid);
                }
            });

            return task.Task;
        }

        private static async Task<Avatar> create_avatar()
        {
            var avatar = new Avatar();
            avatar.Guid = await get_guid();
            foreach (var (name, create) in hosting_data_create)
            {
                var ins = create();
                avatar.add_hosting_data(ins);
            }
            hub.Hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).createPersistedObject(avatar.get_db_doc(), (result) =>
            {
                if (result != hub.DBproxyproxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    log.Log.err("db create avatar faild:{0}", result.ToString());
                }
            });
            return avatar;
        }

        public static Task<Avatar> load_from_remote(string sdk_uuid, string client_uuid, string other_hub)
        {
            var task = new TaskCompletionSource<Avatar>();

            _avatar_Caller.get_hub(other_hub).get_remote_avatar(sdk_uuid).callBack((doc_bin) =>
            {

                var avatar = new Avatar();

                var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(doc_bin);
                foreach (var (name, load) in hosting_data_template)
                {
                    var sub_doc = doc.GetValue(name) as BsonDocument;
                    var ins = load(sub_doc);
                    avatar.add_hosting_data(ins);
                }
                avatar.Guid = doc.GetValue("guid").AsInt64;

                avatar_guid[avatar.Guid] = avatar;

                avatar_sdk_uuid[sdk_uuid] = avatar;
                avatar_client_uuid[client_uuid] = avatar;

                task.SetResult(avatar);

            }, () =>
            {
                log.Log.err("load from remote error!");
                task.SetResult(null);

            }).timeout(3000, () =>
            {
                log.Log.err("load from remote timeout!");
                task.SetResult(null);
            });

            return task.Task;
        }

        public static async Task<Avatar> load_or_create(string sdk_uuid, string client_uuid)
        {
            var avatar = await load_from_db(sdk_uuid);
            if (avatar == null)
            {
                avatar = await create_avatar();
            }
            avatar.ClientUUID = client_uuid;

            avatar_guid[avatar.Guid] = avatar;

            avatar_sdk_uuid[sdk_uuid] = avatar;
            avatar_client_uuid[client_uuid] = avatar;

            return avatar;
        }

        public static Avatar get_current_avatar()
        {
            avatar_client_uuid.TryGetValue(hub.Hub._gates.current_client_uuid, out var avatar);
            return avatar;
        }

        public static Avatar get_target_avatar(long guid)
        {
            avatar_guid.TryGetValue(guid, out var avatar);
            return avatar;
        }

        internal static Avatar get_target_avatar(string sdk_uuid)
        {
            avatar_sdk_uuid.TryGetValue(sdk_uuid, out var avatar);
            return avatar;
        }
    }
}
