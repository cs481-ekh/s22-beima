using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BEIMA.Backend.MongoService
{
    public sealed class MongoConnector
    {
        public ServerType CurrentServerType { get; }

        private MongoClient client = null;
        private static MongoConnector instance = null;
        private readonly string dbName = Environment.GetEnvironmentVariable("DatabaseName");
        private readonly string deviceCollection = Environment.GetEnvironmentVariable("DeviceCollectionName");

        //Private constructor, used for singleton pattern
        private MongoConnector()
        {
            string credentials;

            //TODO: implement check to see if using local or cloud database
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

        //Singleton design pattern
        public static MongoConnector Instance
        {
            get { return instance ??= new MongoConnector(); }
        }

        /*
         * Checks if the client has been instantiated.
         * Parameter: none
         * Returns: true if client is not null, false if client is null.
         */
        public bool IsConnected()
        {
            return (client != null);
        }

        /*
         * Inserts a device into the "devices" collection
         * Parameter: BsonDocument that contains the fully formed device document (including all required and optional fields)
         * Returns: true if success, false if failed
         */
        public ObjectId? InsertDevice(BsonDocument doc)
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                devices.InsertOne(doc);
                return (ObjectId) doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /*
         * Gets a device from the "devices" collection, given an objectID.
         * Parameter: objectId, corresponds to the "_id" field for a given document inside of MongoDB
         * Returns: BsonDocument that was requested
         */
        public BsonDocument GetDevice(ObjectId objectId)
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }

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

        /*
         * Deletes from the "devices" collection, given the objectID.
         * Parameter: objectId, corresponds to the "_id" field for a given document inside of MongoDB
         * Returns: true if successful, false if not successful
         */
        public bool DeleteDevice(ObjectId objectId)
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                var result = devices.DeleteOne(filter);
                return result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        /*
         * Updates a device in the "devices" collection, given a fully formed updated device.
         * Parameters: BsonDocument containing the updated BsonDocument.
         * Returns: true if successful, false if unsuccessful
         */
        public bool UpdateDevice(BsonDocument doc)
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }

            ObjectId objectId = (ObjectId)doc["_id"];
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var devices = db.GetCollection<BsonDocument>(deviceCollection);
                var result = devices.ReplaceOne(filter, doc);
                return result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
