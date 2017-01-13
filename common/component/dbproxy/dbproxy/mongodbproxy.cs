using System;
using System.Collections;
using System.Threading;

namespace dbproxy
{
	public class mongodbproxy
	{
		public mongodbproxy(String ip, short port, String db, String collection)
		{
			var setting = new MongoDB.Driver.MongoServerSettings ();
			setting.Server = new MongoDB.Driver.MongoServerAddress (ip, port);
			_mongoserver = new MongoDB.Driver.MongoServer(setting);
			var _db = _mongoserver.GetDatabase(db);
			_collection = _db.GetCollection<MongoDB.Bson.BsonDocument> (collection);
		}

		public bool save(Hashtable json_data) 
		{
			MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument(json_data);
			_collection.Save(_d);

			return true;
		}

		public bool update(Hashtable json_query, Hashtable json_update) 
		{
			MongoDB.Driver.QueryDocument  _query = new MongoDB.Driver.QueryDocument(json_query);
			MongoDB.Driver.UpdateDocument _update = new MongoDB.Driver.UpdateDocument(json_update);

			var ret = _collection.Update(_query, _update);

			return true;
		}

		public ArrayList find(int skip, int limit, int batch_size, Hashtable json_query, Hashtable json_fields) 
		{
			MongoDB.Driver.QueryDocument _query = new MongoDB.Driver.QueryDocument(json_query);
			MongoDB.Driver.FieldsDocument _fields = new MongoDB.Driver.FieldsDocument(json_fields);

			var c = _collection.FindAs<MongoDB.Bson.BsonDocument>(_query).SetSkip(skip).SetLimit(limit).SetBatchSize(batch_size).SetFields(_fields);

			ArrayList _list = new ArrayList ();
			foreach (var data in c) 
			{
				_list.Add(data.ToHashtable());
			}

			return _list;
		}

		public bool remove(Hashtable json_query) {
			MongoDB.Driver.QueryDocument _query = new MongoDB.Driver.QueryDocument(json_query);
			_collection.Remove (_query);

			return true;
		}

		private MongoDB.Driver.MongoServer _mongoserver;
		private MongoDB.Driver.MongoCollection<MongoDB.Bson.BsonDocument> _collection;

	}
}

