using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// This class abstracts basic CRUD operations for MongoDB. It is implemented as a 
    /// singleton class, so to instantiate, use this syntax: "var mongo = MongoConnector.Instance;".
    /// </summary>
    public sealed class MongoConnector : IMongoConnector
    {
        //Public class members
        public ServerType CurrentServerType { get; }

        //Contains instance variables
        private readonly MongoClient client = null;
        private static readonly Lazy<MongoConnector> instance = new(() => new MongoConnector());

        //Environment variables
        private readonly string dbName = Environment.GetEnvironmentVariable("DatabaseName");
        private readonly string deviceCollection = Environment.GetEnvironmentVariable("DeviceCollectionName");

        //Singleton design pattern, used to get an instance of the MongoConnector
        public static MongoConnector Instance { get { return instance.Value; } }

        //Private constructor, used for singleton pattern. Cannot be called externally.
        private MongoConnector()
        {
            string credentials;

            if (Environment.GetEnvironmentVariable("CurrentEnv") == "dev-local")
            {
                CurrentServerType = ServerType.Local;
                credentials = Environment.GetEnvironmentVariable("LocalMongoConnection");
            }
            else
            {
                CurrentServerType = ServerType.Cloud;
                credentials = Environment.GetEnvironmentVariable("AzureCosmosConnection");
            }
            client = new MongoClient(credentials);
        }

        /// <summary>
        /// Checks if the client has been instantiated.
        /// </summary>
        /// <returns>true if client is not null, false if client is null.</returns>
        public bool IsConnected()
        {
            return (client != null);
        }

        private void CheckIsConnected()
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }
        }

        /// <summary>
        /// Inserts a device into the "devices" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDevice(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                devices.InsertOne(doc);
                return (ObjectId)doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetDevice(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                return devices.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDevices()
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Empty;

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                var docs = devices.Find(filter).ToList();
                return docs;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes from the "devices" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDevice(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                var result = devices.DeleteOne(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a device in the "devices" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDevice(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                ObjectId objectId = (ObjectId)doc["_id"];
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                var result = devices.ReplaceOne(filter, doc);
                if (result.ModifiedCount > 0)
                {
                    return doc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
