﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class mongodbproxy
	{
        private Func<MongoDB.Driver.MongoClient> createMongocLient;
        private List<MongoDB.Driver.MongoClient> client_pool = new List<MongoDB.Driver.MongoClient>();

		public mongodbproxy(String ip, short port)
		{
            createMongocLient = ()=>
            {
                var setting = new MongoDB.Driver.MongoClientSettings();
                setting.Server = new MongoDB.Driver.MongoServerAddress(ip, port);
                return new MongoDB.Driver.MongoClient(setting);
            };
        }

        public mongodbproxy(String url)
        {
            createMongocLient = () =>
            {
                var mongo_url = new MongoDB.Driver.MongoUrl(url);
                return new MongoDB.Driver.MongoClient(mongo_url);
            };
        }

        private MongoDB.Driver.MongoClient getMongoCLient()
        {
            lock(client_pool)
            {
                if (client_pool.Count > 0)
                {
                    var tmp = client_pool[0];
                    client_pool.Remove(tmp);
                    return tmp;
                }
            }

            return createMongocLient();
        }

        private void releaseMongoClient(MongoDB.Driver.MongoClient client)
        {
            lock (client_pool)
            {
                client_pool.Add(client);
            }
        }

        public bool save(string db, string collection, Hashtable json_data) 
		{
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument> (collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument(json_data);
			_collection.InsertOne(_d);

            releaseMongoClient(_mongoclient);

            return true;
		}

        public bool update(string db, string collection, Hashtable json_query, Hashtable json_update)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            var _update_impl = new Hashtable() { { "$set", json_update } };
            
            var _bson_query = new MongoDB.Bson.BsonDocument(json_query);
            var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
            var _bson_update_impl = new MongoDB.Bson.BsonDocument(_update_impl);
            var _update = new MongoDB.Driver.BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update_impl);

			_collection.UpdateOne(_query, _update);

            releaseMongoClient(_mongoclient);

            return true;
		}

		public ArrayList find(string db, string collection, Hashtable json_query) 
		{
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            var _bson_query = new MongoDB.Bson.BsonDocument(json_query);
            var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);

            var c = _collection.FindSync<MongoDB.Bson.BsonDocument>(_query);
            var _c = c.Current;

            ArrayList _list = new ArrayList ();
            if (_c != null)
            {
                foreach (var data in _c)
                {
                    var _data = data.ToHashtable();
                    _list.Add(_data);
                }
            }

            releaseMongoClient(_mongoclient);

            return _list;
		}

		public bool remove(string db, string collection, Hashtable json_query)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            var _bson_query = new MongoDB.Bson.BsonDocument(json_query);
            var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);

            _collection.DeleteOne(_query);

            releaseMongoClient(_mongoclient);

            return true;
		}

	}
}

