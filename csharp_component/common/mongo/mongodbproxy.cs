using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class Mongodbproxy
	{
        private readonly MongoClient _client;
	
        public Mongodbproxy(String ip, short port)
		{
            var setting = new MongoClientSettings();
            setting.Server = new MongoServerAddress(ip, port);
            _client = new MongoClient(setting);
        }

        public Mongodbproxy(String url)
        {
            var mongo_url = new MongoUrl(url);
            _client = new MongoClient(mongo_url);
        }

        private MongoClient getMongoCLient()
        {
            return _client;
        }

        public void create_index(string db, string collection, string key, bool is_unique)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var builder = new IndexKeysDefinitionBuilder<MongoDB.Bson.BsonDocument>();
                var opt = new CreateIndexOptions
                {
                    Unique = is_unique
                };
                var indexModel = new CreateIndexModel<MongoDB.Bson.BsonDocument>(builder.Ascending(key), opt);
                _collection.Indexes.CreateOne(indexModel);
            }
            catch(System.Exception e)
            {
                Log.Log.err("create_index faild, {0}", e.Message);
            }
        }

        public async void check_int_guid(string db, string collection, long _guid)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = MongoDB.Bson.BsonDocument.Parse("{\"Guid\":\"__guid__\"}");
                var _query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);

                var c = await _collection.FindAsync<MongoDB.Bson.BsonDocument>(_query);
                if (c.MoveNext() && (c.Current == null || !c.Current.Any()))
                {
                    MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument { { "Guid", "__guid__" }, { "inside_guid", _guid } };
                    await _collection.InsertOneAsync(_d);
                }
            }
            catch (System.Exception e)
            {
                Log.Log.err("check_int_guid db: {0}, collection: {1}, inside_guid: {2}, faild: {3}", db, collection, _guid, e);
            }
        }

        public async ValueTask<bool> save(string db, string collection, byte[] bson_data) 
		{
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _d = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_data);
                await _collection.InsertOneAsync(_d);
            }
            catch(System.Exception e)
            {
                Log.Log.err("save data faild, {0}", e.Message);
                return false;
            }

            return true;
		}

        public async ValueTask<bool> update(string db, string collection, byte[] bson_query, byte[] bson_update, bool upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                var _bson_update = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_update);

                var _query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _update = new BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update);
                var options = new UpdateOptions() { IsUpsert = upsert };

                await _collection.UpdateOneAsync(_query, _update, options);
            }
            catch (System.Exception e)
            {
                Log.Log.err("update data faild, {0}", e.Message);
                return false;
            }

            return true;
		}

        public async ValueTask<MongoDB.Bson.BsonDocument> find_and_modify(string db, string collection, byte[] bson_query, byte[] bson_update, bool _new, bool _upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                var _bson_update = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_update);

                var _query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$set", _bson_update } };
                var _update = new BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update_impl);
                var options = new FindOneAndUpdateOptions<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>() 
                {        
                    ReturnDocument = _new ? ReturnDocument.After : ReturnDocument.Before,
                    IsUpsert = _upsert
                };

                var r = await _collection.FindOneAndUpdateAsync(_query, _update, options);

                return r;
            }
            catch (System.Exception e)
            {
                Log.Log.err("find_and_modify data faild, {0}", e.Message);
            }

            return null;
        }

        public async ValueTask<IAsyncCursor<MongoDB.Bson.BsonDocument>> find(string db, string collection, byte[] bson_query, int skip, int limit, string sort, bool _Ascending)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                var _opt = new FindOptions<MongoDB.Bson.BsonDocument>();
                if (skip > 0)
                {
                    _opt.Skip = skip;
                }
                if (limit > 0)
                {
                    _opt.Limit = limit;
                }
                if (!string.IsNullOrEmpty(sort))
                {
                    if (_Ascending)
                    {
                        _opt.Sort = Builders<MongoDB.Bson.BsonDocument>.Sort.Ascending(sort);
                    }
                    else
                    {
                        _opt.Sort = Builders<MongoDB.Bson.BsonDocument>.Sort.Descending(sort);
                    }
                }

                return await _collection.FindAsync<MongoDB.Bson.BsonDocument>(_bson_query, _opt);
            }
            catch (System.Exception e)
            {
                Log.Log.err("find faild, {0}", e.Message);
            }

            return null;
		}

        public async ValueTask<int> count(string db, string collection, byte[] bson_query)
        {
            long c = 0;

            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                c = await _collection.CountDocumentsAsync(_bson_query);
            }
            catch (System.Exception e)
            {
                Log.Log.err("count faild, {0}", e.Message);
                return 0;
            }

            return (int)c;
        }

		public async ValueTask<bool> remove(string db, string collection, byte[] bson_query)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                await _collection.DeleteOneAsync(_bson_query);
            }
            catch (System.Exception e)
            {
                Log.Log.err("remove faild, {0}", e.Message);
                return false;
            }

            return true;
		}

        public async ValueTask<long> get_guid(string db, string collection)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection);

            try
            {
                var _bson_query = new MongoDB.Bson.BsonDocument("Guid", "__guid__");
                var _query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$inc", new MongoDB.Bson.BsonDocument { { "inside_guid", 1 } } } };

                var c = await _collection.FindOneAndUpdateAsync<MongoDB.Bson.BsonDocument>(_query, _bson_update_impl);
                return c.GetValue("inside_guid").ToInt64();
            }
            catch (System.Exception e)
            {
                Log.Log.err("get_guid data db: {0}, collection: {1}, guid_key: {2} faild, {3}", db, collection, "inside_guid", e);
                return -1;
            }
        }
    }
}

