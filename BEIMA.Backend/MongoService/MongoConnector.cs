using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly string beimaDb = Environment.GetEnvironmentVariable("DatabaseName");
        private readonly string deviceCollection = Environment.GetEnvironmentVariable("DeviceCollectionName");
        private readonly string deviceTypeCollection = Environment.GetEnvironmentVariable("DeviceTypeCollectionName");
        private readonly string buildingCollection = Environment.GetEnvironmentVariable("BuildingCollectionName");
        private readonly string userCollection = Environment.GetEnvironmentVariable("UserCollectionName");

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

        /// <summary>
        /// Private internal method for checking if mongo is connected, will throw an exception if it is not connected.
        /// </summary>
        /// <exception cref="Exception">Throws exception if Mongo client is not connected.</exception>
        private void CheckIsConnected()
        {
            if (!IsConnected())
            {
                throw new Exception("MongoConnector is not currently connected");
            }
        }

        private ObjectId? Insert(BsonDocument doc, string dbName, string collectionName)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                collection.InsertOne(doc);
                return (ObjectId)doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return null;
            }
        }

        private BsonDocument Get(ObjectId objectId, string dbName, string collectionName)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                return collection.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return null;
            }
        }

        private List<BsonDocument> GetAll(string dbName, string collectionName)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Empty;

            try
            {
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                var docs = collection.Find(filter).ToList();
                return docs;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return null;
            }
        }

        private bool Delete(ObjectId objectId, string dbName, string collectionName)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                var result = collection.DeleteOne(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return false;
            }
        }

        private BsonDocument Update(BsonDocument doc, string dbName, string collectionName)
        {
            CheckIsConnected();

            try
            {
                ObjectId objectId = (ObjectId)doc["_id"];
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                var result = collection.ReplaceOne(filter, doc);
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
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return null;
            }
        }

        #region Device Methods
        /// <summary>
        /// Inserts a device into the "devices" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDevice(BsonDocument doc)
        {
            return Insert(doc, beimaDb, deviceCollection);
        }

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetDevice(ObjectId objectId)
        {
            return Get(objectId, beimaDb, deviceCollection);
        }

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDevices()
        {
            return GetAll(beimaDb, deviceCollection);
        }

        /// <summary>
        /// Deletes from the "devices" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDevice(ObjectId objectId)
        {
            return Delete(objectId, beimaDb, deviceCollection);
        }

        /// <summary>
        /// Updates a device in the "devices" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDevice(BsonDocument doc)
        {
            return Update(doc, beimaDb, deviceCollection);
        }
        #endregion

        #region DeviceType Methods
        /// <summary>
        /// Gets a device type from the "deviceTypes" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetDeviceType(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var deviceTypes = db.GetCollection<BsonDocument>(deviceTypeCollection);
                return deviceTypes.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a device type from the "deviceTypes" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDeviceTypes()
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Empty;

            try
            {
                var db = client.GetDatabase(dbName);
                var deviceTypes = db.GetCollection<BsonDocument>(deviceTypeCollection);
                var docs = deviceTypes.Find(filter).ToList();
                return docs;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Inserts a device type into the "deviceTypes" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device type document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDeviceType(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var deviceTypes = db.GetCollection<BsonDocument>(deviceTypeCollection);
                deviceTypes.InsertOne(doc);
                return (ObjectId)doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes from the "deviceTypes" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDeviceType(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var deviceTypes = db.GetCollection<BsonDocument>(deviceTypeCollection);
                var result = deviceTypes.DeleteOne(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a device type in the "deviceTypes" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDeviceType(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                ObjectId objectId = (ObjectId)doc["_id"];
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var db = client.GetDatabase(dbName);
                var deviceTypes = db.GetCollection<BsonDocument>(deviceTypeCollection);
                var result = deviceTypes.ReplaceOne(filter, doc);
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
        #endregion

        #region Building Methods

        /// <summary>
        /// Gets a Building from the "buildings" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetBuilding(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var buildings = db.GetCollection<BsonDocument>(buildingCollection);
                return buildings.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets all buildings from the "buildings" collection.
        /// </summary>
        /// <returns>List of all building BsonDocuments</returns>
        public List<BsonDocument> GetAllBuildings()
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Empty;

            try
            {
                var db = client.GetDatabase(dbName);
                var buildings = db.GetCollection<BsonDocument>(buildingCollection);
                var docs = buildings.Find(filter).ToList();
                return docs;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Inserts a building into the "buildings" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed building document</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertBuilding(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var buildings = db.GetCollection<BsonDocument>(buildingCollection);
                buildings.InsertOne(doc);
                return (ObjectId)doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes from the "buildings" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteBuilding(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var buildings = db.GetCollection<BsonDocument>(buildingCollection);
                var result = buildings.DeleteOne(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a building in the "buildings" collection, given a fully formed updated building.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateBuilding(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                ObjectId objectId = (ObjectId)doc["_id"];
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var db = client.GetDatabase(dbName);
                var buildings = db.GetCollection<BsonDocument>(buildingCollection);
                var result = buildings.ReplaceOne(filter, doc);
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

        #endregion

        #region User Methods

        /// <summary>
        /// Gets a User from the "users" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetUser(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var users = db.GetCollection<BsonDocument>(userCollection);
                return users.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets all users from the "users" collection.
        /// </summary>
        /// <returns>List of all user BsonDocuments</returns>
        public List<BsonDocument> GetAllUsers()
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Empty;

            try
            {
                var db = client.GetDatabase(dbName);
                var users = db.GetCollection<BsonDocument>(userCollection);
                var docs = users.Find(filter).ToList();
                return docs;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Inserts a user into the "users" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed user document</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertUser(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var users = db.GetCollection<BsonDocument>(userCollection);
                users.InsertOne(doc);
                return (ObjectId)doc["_id"];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes from the "users" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteUser(ObjectId objectId)
        {
            CheckIsConnected();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            try
            {
                var db = client.GetDatabase(dbName);
                var users = db.GetCollection<BsonDocument>(userCollection);
                var result = users.DeleteOne(filter);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a user in the "users" collection, given a fully formed updated user.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateUser(BsonDocument doc)
        {
            CheckIsConnected();

            try
            {
                ObjectId objectId = (ObjectId)doc["_id"];
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var db = client.GetDatabase(dbName);
                var users = db.GetCollection<BsonDocument>(userCollection);
                var result = users.ReplaceOne(filter, doc);
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

        #endregion
    }
}
