using MongoDB.Bson;
using System;
using System.Collections.Generic;

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

    public static class AvatarMgr
    {
        private static Dictionary<string, Func<IHostingData> > hosting_data_template = new();

        private static Dictionary<string, Avatar> avatar_sdk_uuid = new();
        private static Dictionary<string, Avatar> avatar_client_uuid = new();

        private static AvatarDataOriginalOptions opt;

        public static void init_data_original(Action<AvatarDataOriginalOptions> configureOptions)
        {
            configureOptions.Invoke(opt);
        }

        public static void add_hosting<T>() where T : IHostingData
        {
            var type_str = T.type();
            hosting_data_template.Add(type_str, T.create);
        }

        /*
         * load data from original
         */
        public static Avatar load_or_create(string sdk_uuid, string client_uuid)
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
