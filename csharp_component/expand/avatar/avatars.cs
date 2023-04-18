using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace avatar
{
    public interface IHostingDataInterface
    {
        public static virtual string type()
        {
            return string.Empty;
        }

        public void load(BsonDocument data);

        public BsonDocument store();
    }

    public class Avatar
    {
        private Dictionary<string, IHostingDataInterface> dataDict = new();

        public long Guid;
        public string ClientUUID;

        public void add_hosting_data<T>(T data) where T : IHostingDataInterface
        {
            dataDict.Add(T.type(), data);
        }

        public T hosting_data<T>() where T : IHostingDataInterface
        {
            if (dataDict.TryGetValue(T.type(), out var data))
            {
                return (T)data;
            }
            return default(T);
        }
    }

    public class AvatarMgr
    {
        private Dictionary<string, Type> hosting_data_template = new();

        private Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private Dictionary<string, Avatar> avatar_client_uuid = new();

        public AvatarMgr add_hosting(Type t) 
        {
            var type_str_method = t.GetMethod("type", BindingFlags.Public | BindingFlags.Static);
            var type_str = (string)type_str_method?.Invoke(null, null);
            hosting_data_template.Add(type_str, t);
            return this;
        }

        public Avatar load_or_create(string sdk_uuid, string client_uuid, string data_src = "db")
        {
            return null;
        }
    }
}
