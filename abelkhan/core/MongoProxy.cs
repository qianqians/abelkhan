using MongoDB.Driver;

namespace core;

public class MongoProxy
{
    private readonly MongoClient _client;

    public MongoProxy(string ip, short port)
	{
        var setting = new MongoClientSettings
        {
            Server = new MongoServerAddress(ip, port)
        };
        _client = new MongoClient(setting);
    }

    public MongoProxy(string url)
    {
        var mongoUrl = new MongoUrl(url);
        _client = new MongoClient(mongoUrl);
    }

    private MongoClient GetMongoClient()
    {
        return _client;
    }

    public void CreateIndex(string dbName, string collectionName, string key, bool isUnique)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        try
        {
            var builder = new IndexKeysDefinitionBuilder<MongoDB.Bson.BsonDocument>();
            var opt = new CreateIndexOptions
            {
                Unique = isUnique
            };
            var indexModel = new CreateIndexModel<MongoDB.Bson.BsonDocument>(builder.Ascending(key), opt);
            collection.Indexes.CreateOne(indexModel);
        }
        catch(Exception e)
        {
            Log.Error("create_index failed, {0}", e.Message);
        }
    }

    public async void CheckIntGuid(string dbName, string collectionName, long guid)
    {
        try
        {
            var mongoClient = GetMongoClient();
            var db = mongoClient.GetDatabase(dbName);
            var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

            var bsonQuery = MongoDB.Bson.BsonDocument.Parse("{\"Guid\":\"__guid__\"}");
            var query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(bsonQuery);

            var c = await collection.FindAsync<MongoDB.Bson.BsonDocument>(query);
            if (await c.MoveNextAsync() && (c.Current == null || !c.Current.Any()))
            {
                MongoDB.Bson.BsonDocument d = new MongoDB.Bson.BsonDocument { { "Guid", "__guid__" }, { "inside_guid", guid } };
                await collection.InsertOneAsync(d);
            }
        }
        catch (Exception e)
        {
            Log.Error("check_int_guid db: {0}, collection: {1}, inside_guid: {2}, failed: {3}", dbName, collectionName, guid, e);
        }
    }

    public async ValueTask<bool> Save(string dbName, string collectionName, byte[] bsonData) 
	{
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var d = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonData);
        await collection.InsertOneAsync(d);

        return true;
	}

    public async ValueTask<bool> Update(string dbName, string collectionName, byte[] bsonQuery, byte[] bsonUpdate, bool upsert)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonQuery);
        var bsonUpdateDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonUpdate);

        var query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(bsonQueryDoc);
        var update = new BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(bsonUpdateDoc);
        var options = new UpdateOptions() { IsUpsert = upsert };

        await collection.UpdateOneAsync(query, update, options);

        return true;
	}

    public async ValueTask<MongoDB.Bson.BsonDocument> FindAndModify(string dbName, string collectionName, byte[] bsonQuery, byte[] bsonUpdate, bool isNew, bool upsert)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonQuery);
        var bsonUpdateDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonUpdate);

        var query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(bsonQueryDoc);
        var bsonUpdateImpl = new MongoDB.Bson.BsonDocument { { "$set", bsonUpdateDoc } };
        var update = new BsonDocumentUpdateDefinition<MongoDB.Bson.BsonDocument>(bsonUpdateImpl);
        var options = new FindOneAndUpdateOptions<MongoDB.Bson.BsonDocument, MongoDB.Bson.BsonDocument>()
        {
            ReturnDocument = isNew ? ReturnDocument.After : ReturnDocument.Before,
            IsUpsert = upsert
        };

        var r = await collection.FindOneAndUpdateAsync(query, update, options);

        return r;
    }

    public async ValueTask<IAsyncCursor<MongoDB.Bson.BsonDocument>> Find(string dbName, string collectionName, byte[] bsonQuery, int skip, int limit, string sort, bool ascending)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonQuery);
        var opt = new FindOptions<MongoDB.Bson.BsonDocument>();
        if (skip > 0)
        {
            opt.Skip = skip;
        }
        if (limit > 0)
        {
            opt.Limit = limit;
        }
        if (!string.IsNullOrEmpty(sort))
        {
            opt.Sort = ascending ? Builders<MongoDB.Bson.BsonDocument>.Sort.Ascending(sort) : Builders<MongoDB.Bson.BsonDocument>.Sort.Descending(sort);
        }

        return await collection.FindAsync<MongoDB.Bson.BsonDocument>(bsonQueryDoc, opt);
    }

    public async ValueTask<int> Count(string dbName, string collectionName, byte[] bsonQuery)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonQuery);
        return (int)(await collection.CountDocumentsAsync(bsonQueryDoc));
    }

	public async ValueTask<bool> Remove(string dbName, string collectionName, byte[] bsonQuery)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(bsonQuery);
        await collection.DeleteOneAsync(bsonQueryDoc);

        return true;
	}

    public async ValueTask<long> get_guid(string dbName, string collectionName)
    {
        var mongoClient = GetMongoClient();
        var db = mongoClient.GetDatabase(dbName);
        var collection = db.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);

        var bsonQueryDoc = new MongoDB.Bson.BsonDocument("Guid", "__guid__");
        var query = new BsonDocumentFilterDefinition<MongoDB.Bson.BsonDocument>(bsonQueryDoc);
        var bsonUpdateImpl = new MongoDB.Bson.BsonDocument { { "$inc", new MongoDB.Bson.BsonDocument { { "inside_guid", 1 } } } };

        var c = await collection.FindOneAndUpdateAsync<MongoDB.Bson.BsonDocument>(query, bsonUpdateImpl);
        return c.GetValue("inside_guid").ToInt64();
    }
}

