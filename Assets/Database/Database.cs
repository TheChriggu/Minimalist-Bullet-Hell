using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

public class Database
{
    MongoClient client;
    IMongoDatabase database;

    public Database()
    {
        client = new MongoClient("mongodb://127.0.0.1:27017");
        database = client.GetDatabase("BulletHell");
    }

    public void Save<T>(T data)
    {
        var collection = database.GetCollection<T>("data");
        collection.InsertOne(data);
    }

    public SaveListContainer Load(string id)
    {
        var collection = database.GetCollection<SaveListContainer>("data");
        return collection.Find(Builders<SaveListContainer>.Filter.Eq("id", id)).First();//Limit(1).ToList()[0];
    }
}
