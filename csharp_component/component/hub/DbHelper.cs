using MongoDB.Bson;
using System.Collections.Generic;

namespace abelkhan
{
    public class SaveDataHelper
    {
        public class SaveDataHelperException : Exception
        {
            public SaveDataHelperException(string err_info) : base(err_info)
            {
            }
        }

        private BsonDocument bson_data = null;
        private Dictionary<string, string> set_data = new Dictionary<string, string>();

        public SaveDataHelper set<T>(string key, T t)
        {
            if (bson_data != null)
            {
                throw new SaveDataHelperException("repeat set value json_data is set!");
            }
            set_data.Add(key, t.ToString());
            return this;
        }

        public SaveDataHelper set(string key, string v)
        {
            if (bson_data != null)
            {
                throw new SaveDataHelperException("repeat set value json_data is set!");
            }
            set_data.Add(key, "\"" + v + "\"");
            return this;
        }

        public SaveDataHelper set<T>(T t)
        {
            if (set_data.Count > 0)
            {
                throw new SaveDataHelperException("repeat set value set_data is set!");
            }
            bson_data = t.ToBsonDocument();
            return this;
        }

        public BsonDocument data()
        {
            BsonDocument _data;

            if (set_data.Count > 0)
            {
                _data = new BsonDocument(set_data);
            }
            else if (bson_data != null)
            {
                _data = bson_data;
            }
            else
            {
                throw new SaveDataHelperException("empty document!");
            }

            return _data;
        }
    }

    public class UpdateDataHelper
    {
        public class UpdateDataHelperException : Exception
        {
            public UpdateDataHelperException(string err_info) : base(err_info)
            {
            }
        }

        private BsonDocument bson_data = null;
        private Dictionary<string, string> set_data = new Dictionary<string, string>();
        private Dictionary<string, string> inc_data = new Dictionary<string, string>();

        public UpdateDataHelper set<T>(string key, T t)
        {
            if (bson_data != null)
            {
                throw new UpdateDataHelperException("repeat set value json_data is set!");
            }
            set_data.Add(key, t.ToString());
            return this;
        }

        public UpdateDataHelper set(string key, string v)
        {
            if (bson_data != null)
            {
                throw new UpdateDataHelperException("repeat set value json_data is set!");
            }
            set_data.Add(key, "\"" + v + "\"");
            return this;
        }

        public UpdateDataHelper set<T>(T t)
        {
            if (set_data.Count > 0)
            {
                throw new UpdateDataHelperException("repeat set value set_data is set!");
            }
            bson_data = t.ToBsonDocument();
            return this;
        }

        public void inc<T>(string key, T t)
        {
            inc_data.Add(key, t.ToString());
        }

        public BsonDocument data()
        {
            BsonDocument _data = new();

            if (set_data.Count > 0)
            {
                var _bson_set_data = new BsonDocument(set_data);
                _data.Add("$set", _bson_set_data);
            }
            else if (bson_data != null)
            {
                _data.Add("$set", bson_data);
            }

            if (inc_data.Count > 0)
            {
                var _bson_inc_data = new BsonDocument(inc_data);
                _data.Add("$inc", _bson_inc_data);
            }

            return _data;
        }

        public bool empty()
        {
            return inc_data.Count == 0 && set_data.Count == 0 && bson_data == null;
        }
    }


    public class DBQueryHelper
    {
        private readonly List<KeyValuePair<string, string> > query_condition = new ();

        public DBQueryHelper condition<T>(string key, T t)
        {
            query_condition.Add(KeyValuePair.Create(key, t.ToString()));
            return this;
        }

        public DBQueryHelper condition(string key, string v)
        {
            query_condition.Add(KeyValuePair.Create(key, "\"" + v + "\""));
            return this;
        }

        public void gte<T>(string key, T t)
        {
            query_condition.Add(KeyValuePair.Create(key, "{\"$gte\":" + t.ToString() + "}"));
        }

        public void lte<T>(string key, T t)
        {
            query_condition.Add(KeyValuePair.Create(key, "{\"$lte\":" + t.ToString() + "}"));
        }

        public BsonDocument query()
        {
            BsonDocument _query= new()
            {
                { "$and", query_condition.ToBsonDocument() }
            };

            return _query;
        }

        public bool empty()
        {
            return query_condition.Count == 0;
        }
    }
}