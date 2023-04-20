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

    /*
     * original date agent
     * write back data to memory
     */
    public class DataAgent<T> : IDataAgent<T> where T : IHostingData
    {
        public override void write_back()
        {

        }
    }

    /*
     * remote date agent
     * write back data to original
     */
    public class RemoteDataAgent<T> : IDataAgent<T> where T : IHostingData
    {
        public override void write_back()
        {

        }
    }

    public class Avatar
    {
        private Dictionary<string, IHostingData> dataDict = new();

        public long Guid;
        public string ClientUUID;

        private bool is_original;

        public Avatar(bool _is_original)
        {
            is_original = _is_original;
        }

        public void add_hosting_data<T>(T data) where T : IHostingData
        {
            dataDict.Add(T.type(), data);
        }

        public IDataAgent<T> get_clone_hosting_data<T>() where T : IHostingData
        {
            if (dataDict.TryGetValue(T.type(), out var data))
            {
                IDataAgent<T> agent = null;
                if (is_original)
                {
                    agent = new DataAgent<T>();
                }
                else
                {
                    agent = new RemoteDataAgent<T>();
                }
                
                agent.Data = (T)data;
                return agent;
            }
            return default(IDataAgent<T>);
        }
    }

    public class AvatarDataOriginalOptions
    {
        public enum EDataOriginal
        {
            DB = 0,
            OtherHub = 1,
        }

        public EDataOriginal DataOriginal { get; set; }
        public string DBName { get; set; }
        public string DBCollection { get; set; }
        public string HubName { get; set; }
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

        private static Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private static Dictionary<string, Avatar> avatar_client_uuid = new();

        private static AvatarDataOriginalOptions opt = new();

        public static void init_data_original(Action<AvatarDataOriginalOptions> configureOptions)
        {
            configureOptions.Invoke(opt);
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
            hub.hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).getObjectInfo(query.query(), (value) => {
                if (value.Count > 1)
                {
                    throw new AvtarLoadFromDBError($"repeated sdk_uuid:{sdk_uuid}");
                }
                else if (value.Count == 1)
                {
                    var avatar = new Avatar(true);
                    
                    var doc = value[0] as BsonDocument;
                    foreach (var (name, load) in hosting_data_template)
                    {
                        var sub_doc = doc.GetValue(name) as BsonDocument;
                        var ins = load(sub_doc);
                        avatar.add_hosting_data(ins);
                    }

                    task.SetResult(avatar);
                }
                else
                {
                    task.SetResult(null);
                }
            }, () => { });

            return task.Task;
        }

        public static Avatar create_avatar(bool _is_original)
        {
            var avatar = new Avatar(true);
            foreach (var (name, create) in hosting_data_create)
            {
                var ins = create();
                avatar.add_hosting_data(ins);
            }
            return avatar;
        }

        /*
         * load data from original
         */
        public static async Task<Avatar> load_or_create(string sdk_uuid, string client_uuid)
        {
            if (opt.DataOriginal == AvatarDataOriginalOptions.EDataOriginal.DB)
            {
                var avatar = await load_from_db(sdk_uuid);
                if (avatar == null)
                {
                    avatar = create_avatar(true);
                }
                avatar_sdk_uuid[sdk_uuid] = avatar;
                avatar_client_uuid[client_uuid] = avatar;
                return avatar;
            }
            return null;
        }

        public static Avatar get_current_avatar()
        {
            avatar_client_uuid.TryGetValue(hub.hub._gates.current_client_uuid, out var avatar);
            return avatar;
        }
    }
}
