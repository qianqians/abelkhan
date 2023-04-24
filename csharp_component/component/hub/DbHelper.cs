using MongoDB.Bson;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Abelkhan
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
        private readonly List<KeyValuePair<string, BsonValue> > query_condition = new ();

        public DBQueryHelper condition(string key, long t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, t));
            return this;
        }

        public DBQueryHelper condition(string key, int t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, t));
            return this;
        }

        public DBQueryHelper condition(string key, uint t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, t));
            return this;
        }

        public DBQueryHelper condition(string key, float t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, t));
            return this;
        }

        public DBQueryHelper condition(string key, double t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, t));
            return this;
        }

        public DBQueryHelper condition(string key, string v)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, v));
            return this;
        }

        public void elemListMatchEq(string key, long t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void elemListMatchEq(string key, int t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void elemListMatchEq(string key, uint t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void elemListMatchEq(string key, float t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void elemListMatchEq(string key, double t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void elemListMatchEq(string key, string t)
        {
            var _condition = new BsonDocument("$eq", t);
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$elemMatch", _condition)));
        }

        public void lte(string key, long t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$lte", t)));
        }

        public void lte(string key, int t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$lte", t)));
        }

        public void lte(string key, uint t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$lte", t)));
        }

        public void lte(string key, float t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$lte", t)));
        }

        public void lte(string key, double t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$lte", t)));
        }

        public void gte(string key, long t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$gte", t)));
        }

        public void gte(string key, int t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$gte", t)));
        }

        public void gte(string key, uint t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$gte", t)));
        }

        public void gte(string key, float t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$gte", t)));
        }

        public void gte(string key, double t)
        {
            query_condition.Add(new KeyValuePair<string, BsonValue>(key, new BsonDocument("$gte", t)));
        }

        public BsonDocument query()
        {
            var _condition = new BsonArray();
            foreach (var c in query_condition)
            {
                _condition.Add(new BsonDocument(c.Key, c.Value));
            }
            BsonDocument _query = new()
            {
                { "$and", _condition }
            };

            return _query;
        }

        public bool empty()
        {
            return query_condition.Count == 0;
        }
    }
}