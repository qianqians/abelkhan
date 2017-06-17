using System;
using System.Collections;

namespace dbproxy
{
	public class mongodbproxy
	{
		public mongodbproxy(String ip, short port)
		{
            var setting = new MongoDB.Driver.MongoServerSettings ();
			setting.Server = new MongoDB.Driver.MongoServerAddress (ip, port);
            _mongoserver = new MongoDB.Driver.MongoServer(setting);
        }

		public bool save(string db, string collection, Hashtable json_data) 
		{
            var _db = _mongoserver.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument> (collection) as MongoDB.Driver.MongoCollection<MongoDB.Bson.BsonDocument>;

            MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument(json_data);
			_collection.Save(_d);

			return true;
		}

		public bool update(string db, string collection, Hashtable json_query, Hashtable json_update) 
		{
            var _db = _mongoserver.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.MongoCollection<MongoDB.Bson.BsonDocument>;

            MongoDB.Driver.QueryDocument  _query = new MongoDB.Driver.QueryDocument(json_query);
			MongoDB.Driver.UpdateDocument _update = new MongoDB.Driver.UpdateDocument(json_update);

			var ret = _collection.Update(_query, _update);

            return true;
		}

		public ArrayList find(string db, string collection, int skip, int limit, int batch_size, Hashtable json_query, Hashtable json_fields) 
		{
            var _db = _mongoserver.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.MongoCollection<MongoDB.Bson.BsonDocument>;

            MongoDB.Driver.QueryDocument _query = new MongoDB.Driver.QueryDocument(json_query);

            MongoDB.Driver.MongoCursor<MongoDB.Bson.BsonDocument> c = null;
            if (json_fields != null)
            {
                MongoDB.Driver.FieldsDocument _fields = new MongoDB.Driver.FieldsDocument(json_fields);
                c = _collection.FindAs<MongoDB.Bson.BsonDocument>(_query).SetSkip(skip).SetLimit(limit).SetBatchSize(batch_size).SetFields(_fields);
            }
            else
            {
                c = _collection.FindAs<MongoDB.Bson.BsonDocument>(_query).SetSkip(skip).SetLimit(limit).SetBatchSize(batch_size);
            }
            
            ArrayList _list = new ArrayList ();
            foreach (var data in c) 
			{
                var _data = data.ToHashtable();
                _data.Remove("_id");
                _list.Add(_data);
			}

            return _list;
		}

		public bool remove(string db, string collection, Hashtable json_query) {
            var _db = _mongoserver.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.MongoCollection<MongoDB.Bson.BsonDocument>;

            MongoDB.Driver.QueryDocument _query = new MongoDB.Driver.QueryDocument(json_query);
			_collection.Remove (_query);

			return true;
		}

		private MongoDB.Driver.MongoServer _mongoserver;

	}
}

