﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MsgPack.Serialization;

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

        public void create_index(string db, string collection, string key, bool is_unique)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var builder = new MongoDB.Driver.IndexKeysDefinitionBuilder<MongoDB.Bson.BsonDocument>();
                var opt = new MongoDB.Driver.CreateIndexOptions();
                opt.Unique = is_unique;
                var indexModel = new MongoDB.Driver.CreateIndexModel<MongoDB.Bson.BsonDocument>(builder.Ascending(key), opt);
                _collection.Indexes.CreateOne(indexModel);
            }
            catch(System.Exception e)
            {
                log.log.err("create_index faild, {0}", e.Message);
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }
        }

        public async void check_int_guid(string db, string collection, Int64 inside_guid, Int64 public_guid)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.BsonDocument.Parse("{\"Guid\":\"__guid__\"}");
                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);

                var c = await _collection.FindAsync<MongoDB.Bson.BsonDocument>(_query);
                if (c.MoveNext() && (c.Current == null || !c.Current.Any()))
                {
                    MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument { { "Guid", "__guid__" }, { "inside_guid", inside_guid }, { "public_guid", public_guid } };
                    await _collection.InsertOneAsync(_d);
                }
            }
            catch (System.Exception e)
            {
                log.log.err("check_int_guid db: {0}, collection: {1}, inside_guid: {2}, public_guid: {3} faild, {4}", db, collection, inside_guid, public_guid, e);
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }
        }

        public async Task<bool> save(string db, string collection, byte[] bson_data) 
		{
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument> (collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _d = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_data);
                await _collection.InsertOneAsync(_d);
            }
            catch(System.Exception e)
            {
                log.log.err("save data faild, {0}", e.Message);
                return false;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return true;
		}

        public async Task<bool> update(string db, string collection, byte[] bson_query, byte[] bson_update, bool upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                var _bson_update = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_update);

                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _update = new MongoDB.Driver.BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update);
                var options = new UpdateOptions() { IsUpsert = upsert };

                await _collection.UpdateOneAsync(_query, _update, options);
            }
            catch (System.Exception e)
            {
                log.log.err("update data faild, {0}", e.Message);
                return false;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return true;
		}

        public async Task<MongoDB.Bson.BsonDocument> find_and_modify(string db, string collection, byte[] bson_query, byte[] bson_update, bool _new, bool _upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                var _bson_update = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_update);

                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$set", _bson_update } };
                var _update = new MongoDB.Driver.BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update_impl);
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
                log.log.err("find_and_modify data faild, {0}", e.Message);
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return null;
        }

        public async Task<MongoDB.Bson.BsonArray> find(string db, string collection, byte[] bson_query, int skip, int limit, string sort, bool _Ascending)
        {
            var _list = new MongoDB.Bson.BsonArray();

            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

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

                var c = await _collection.FindAsync<MongoDB.Bson.BsonDocument>(_bson_query, _opt);

                do
                {
                    var _c = c.Current;

                    if (_c != null)
                    {
                        foreach (var data in _c)
                        {
                            data.Remove("_id");
                            _list.Add(data);
                        }
                    }
                } while (c.MoveNext());
            }
            catch (System.Exception e)
            {
                log.log.err("find faild, {0}", e.Message);
                return _list;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return _list;
		}

        public async Task<int> count(string db, string collection, byte[] bson_query)
        {
            long c = 0;

            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                c = await _collection.CountDocumentsAsync(_bson_query);
            }
            catch (System.Exception e)
            {
                log.log.err("count faild, {0}", e.Message);
                return 0;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return (int)c;
        }

		public async Task<bool> remove(string db, string collection, byte[] bson_query)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bson_query);
                await _collection.DeleteOneAsync(_bson_query);
            }
            catch (System.Exception e)
            {
                log.log.err("remove faild, {0}", e.Message);
                return false;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }

            return true;
		}

        public async Task<Int64> get_guid(string db, string collection, string guid_key)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var _bson_query = new MongoDB.Bson.BsonDocument("Guid", "__guid__");
                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$inc", new MongoDB.Bson.BsonDocument { { guid_key, 1 } } } };

                var c = await _collection.FindOneAndUpdateAsync<MongoDB.Bson.BsonDocument>(_query, _bson_update_impl);
                return c.GetValue(guid_key).ToInt64();
            }
            catch (System.Exception e)
            {
                log.log.err("get_guid data db: {0}, collection: {1}, guid_key: {2} faild, {3}", db, collection, guid_key, e);
                return -1;
            }
            finally
            {
                releaseMongoClient(_mongoclient);
            }
        }
    }
}

