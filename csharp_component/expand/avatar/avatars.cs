﻿using abelkhan;
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
    class DataAgent<T> : IDataAgent<T> where T : IHostingData
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
            var doc = new BsonDocument();

            foreach (var (name, data) in dataDict)
            {
                doc.Add(name, data.ToBsonDocument());
            }

            return doc;
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
                    var avatar = new Avatar();
                    
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

        private static Avatar create_avatar(bool _is_original)
        {
            var avatar = new Avatar();
            foreach (var (name, create) in hosting_data_create)
            {
                var ins = create();
                avatar.add_hosting_data(ins);
            }
            hub.hub.get_random_dbproxyproxy().getCollection(opt.DBName, opt.DBCollection).createPersistedObject(avatar.get_db_doc(), (result) =>
            {
                if (result != hub.dbproxyproxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    log.log.err("db create avatar faild:{0}", result.ToString());
                }
            });
            return avatar;
        }

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
            else
            {

            }
            return null;
        }

        public static Avatar get_current_avatar()
        {
            avatar_client_uuid.TryGetValue(hub.hub._gates.current_client_uuid, out var avatar);
            return avatar;
        }

        public static Avatar get_target_avatar(string sdk_uuid)
        {
            avatar_sdk_uuid.TryGetValue(sdk_uuid, out var avatar);
            return avatar;
        }
    }
}
