﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public async Task<bool> save(string db, string collection, byte[] msgpack_data) 
		{
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument> (collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_data, 0, msgpack_data.Length);
                _tmp.Position = 0;
                var table = serializer.Unpack(_tmp);

                MongoDB.Bson.BsonDocument _d = new MongoDB.Bson.BsonDocument(table);
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

        public async Task<bool> update(string db, string collection, byte[] msgpack_query, byte[] msgpack_update, bool upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_query, 0, msgpack_query.Length);
                _tmp.Position = 0;
                var table_query = serializer.Unpack(_tmp);
                var _bson_query = new MongoDB.Bson.BsonDocument(table_query);

                _tmp = new MemoryStream();
                _tmp.Write(msgpack_update, 0, msgpack_update.Length);
                _tmp.Position = 0;
                var table_update = serializer.Unpack(_tmp);
                var _bson_update = new MongoDB.Bson.BsonDocument(table_update);

                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$set", _bson_update } };
                var _update = new MongoDB.Driver.BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update_impl);
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

        public async Task<Hashtable> find_and_modify(string db, string collection, byte[] msgpack_query, byte[] msgpack_update, bool _new, bool _upsert)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_query, 0, msgpack_query.Length);
                _tmp.Position = 0;
                var table_query = serializer.Unpack(_tmp);
                var _bson_query = new MongoDB.Bson.BsonDocument(table_query);

                _tmp = new MemoryStream();
                _tmp.Write(msgpack_update, 0, msgpack_update.Length);
                _tmp.Position = 0;
                var table_update = serializer.Unpack(_tmp);
                var _bson_update = new MongoDB.Bson.BsonDocument(table_update);

                var _query = new MongoDB.Driver.BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(_bson_query);
                var _bson_update_impl = new MongoDB.Bson.BsonDocument { { "$set", _bson_update } };
                var _update = new MongoDB.Driver.BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(_bson_update_impl);
                var options = new FindOneAndUpdateOptions<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>() 
                {        
                    ReturnDocument = _new ? ReturnDocument.After : ReturnDocument.Before,
                    IsUpsert = _upsert
                };

                var r = await _collection.FindOneAndUpdateAsync(_query, _update, options);
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

        public async Task<ArrayList> find(string db, string collection, byte[] msgpack_query)
        {
            ArrayList _list = new ArrayList();

            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_query, 0, msgpack_query.Length);
                _tmp.Position = 0;
                var table_query = serializer.Unpack(_tmp);
                var _bson_query = new MongoDB.Bson.BsonDocument(table_query);

                var c = await _collection.FindAsync<MongoDB.Bson.BsonDocument>(_bson_query);

                do
                {
                    var _c = c.Current;

                    if (_c != null)
                    {
                        foreach (var data in _c)
                        {
                            var _data = data.ToHashtable();
                            _data.Remove("_id");
                            _list.Add(_data);
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

        public async Task<int> count(string db, string collection, byte[] msgpack_query)
        {
            long c = 0;

            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_query, 0, msgpack_query.Length);
                _tmp.Position = 0;
                var table_query = serializer.Unpack(_tmp);
                var _bson_query = new MongoDB.Bson.BsonDocument(table_query);

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

		public async Task<bool> remove(string db, string collection, byte[] msgpack_query)
        {
            var _mongoclient = getMongoCLient();
            var _db = _mongoclient.GetDatabase(db);
            var _collection = _db.GetCollection<MongoDB.Bson.BsonDocument>(collection) as MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument>;

            try
            {
                var serializer = MessagePackSerializer.Get<Hashtable>();

                var _tmp = new MemoryStream();
                _tmp.Write(msgpack_query, 0, msgpack_query.Length);
                _tmp.Position = 0;
                var table_query = serializer.Unpack(_tmp);
                var _bson_query = new MongoDB.Bson.BsonDocument(table_query);

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

	}
}
