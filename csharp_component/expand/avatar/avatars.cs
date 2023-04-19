using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

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

        public void load(BsonDocument data);

        public BsonDocument store();
    }

    public class IDataAgent<T> where T : IHostingData
    {
        public T Data { get; set; }

        public void write_back()
        {
        }
    }

    public class Avatar
    {
        private Dictionary<string, IHostingData> dataDict = new();

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
                var agent = new IDataAgent<T>();
                agent.Data = (T)data;
                return agent;
            }
            return default(IDataAgent<T>);
        }
    }

    public static class AvatarMgr
    {
        private static Dictionary<string, Func<IHostingData>> hosting_data_template = new();

        private static Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private static Dictionary<string, Avatar> avatar_client_uuid = new();

        public static void add_hosting<T>() where T : IHostingData
        {
            var type_str = T.type();
            hosting_data_template.Add(type_str, T.create);
        }

        /*
         * 从dbproxy拉取数据
         */
        public static Avatar load_or_create(string sdk_uuid, string client_uuid)
        {
            return null;
        }

        /*
         * 从data_src节点拉取数据
         */
        public static Avatar load_or_create(string sdk_uuid, string client_uuid, string data_src)
        {
            return null;
        }

        public static Avatar get_current_avatar(string client_uuid)
        {
            avatar_client_uuid.TryGetValue(client_uuid, out var avatar);
            return avatar;
        }
    }
}
