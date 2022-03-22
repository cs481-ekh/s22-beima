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

        #region Base CRUD Methods

        /// <summary>
        /// Inserts a BsonDocument into the given database/collection.
        /// </summary>
        /// <param name="doc">BsonDocument to insert.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
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

        /// <summary>
        /// Gets a single BsonDocument from the given database/collection, given an ObjectId.
        /// </summary>
        /// <param name="objectId">ObjectId of the object to be retrieved ("_id" field).</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>BsonDocument that was requested</returns>
        private BsonDocument Get(ObjectId objectId, string dbName, string collectionName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
            return GetFiltered(filter, dbName, collectionName)?.FirstOrDefault();
        }

        /// <summary>
        /// Gets a single BsonDocument from the given database/collection, given an ObjectId.
        /// </summary>
        /// <param name="filter">The filter that is being applied.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>BsonDocument that was requested</returns>
        private List<BsonDocument> GetFiltered(FilterDefinition<BsonDocument> filter, string dbName, string collectionName)
        {
            CheckIsConnected();

            try
            {
                var db = client.GetDatabase(dbName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                return collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{dbName} {collectionName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all documents from the given database/collection.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>List of BsonDocuments that was requested</returns>
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

        /// <summary>
        /// Deletes one document given an ObjectId, from the given database/collection.
        /// </summary>
        /// <param name="objectId">The ObjectId of the object to delete. ("_id" field)</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>true if successful, false if not successful</returns>
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

        /// <summary>
        /// Replaces a single document given the updated BsonDocument, inside the given database/collection.
        /// </summary>
        /// <param name="doc">The fully formed updated BsonDocument.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>true if successful, false if not successful</returns>
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

        #endregion

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
        /// Gets a list of Device BsonDocuments using the passed in filter.
        /// </summary>
        /// <param name="filter">The filter to be applied.</param>
        /// <returns>List of BsonDocuments</returns>
        public List<BsonDocument> GetFilteredDevices(FilterDefinition<BsonDocument> filter)
        {
            return GetFiltered(filter, beimaDb, deviceCollection);
        }

        /// <summary>
        /// Gets all devices from the "devices" collection.
        /// </summary>
        /// <returns>List of BsonDocuments that was requested</returns>
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
            return Get(objectId, beimaDb, deviceTypeCollection);
        }

        /// <summary>
        /// Gets a device type from the "deviceTypes" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDeviceTypes()
        {
            return GetAll(beimaDb, deviceTypeCollection);
        }

        /// <summary>
        /// Inserts a device type into the "deviceTypes" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device type document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDeviceType(BsonDocument doc)
        {
            return Insert(doc, beimaDb, deviceTypeCollection);
        }

        /// <summary>
        /// Deletes from the "deviceTypes" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDeviceType(ObjectId objectId)
        {
            return Delete(objectId, beimaDb, deviceTypeCollection);
        }

        /// <summary>
        /// Updates a device type in the "deviceTypes" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDeviceType(BsonDocument doc)
        {
            return Update(doc, beimaDb, deviceTypeCollection);
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
            return Get(objectId, beimaDb, buildingCollection);
        }

        /// <summary>
        /// Gets all buildings from the "buildings" collection.
        /// </summary>
        /// <returns>List of all building BsonDocuments</returns>
        public List<BsonDocument> GetAllBuildings()
        {
            return GetAll(beimaDb, buildingCollection);
        }

        /// <summary>
        /// Inserts a building into the "buildings" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed building document</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertBuilding(BsonDocument doc)
        {
            return Insert(doc, beimaDb, buildingCollection);
        }

        /// <summary>
        /// Deletes from the "buildings" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteBuilding(ObjectId objectId)
        {
            return Delete(objectId, beimaDb, buildingCollection);
        }

        /// <summary>
        /// Updates a building in the "buildings" collection, given a fully formed updated building.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateBuilding(BsonDocument doc)
        {
            return Update(doc, beimaDb, buildingCollection);
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
            return Get(objectId, beimaDb, userCollection);
        }

        /// <summary>
        /// Gets all users from the "users" collection.
        /// </summary>
        /// <returns>List of all user BsonDocuments</returns>
        public List<BsonDocument> GetAllUsers()
        {
            return GetAll(beimaDb, userCollection);
        }

        /// <summary>
        /// Inserts a user into the "users" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed user document</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertUser(BsonDocument doc)
        {
            return Insert(doc, beimaDb, userCollection);
        }

        /// <summary>
        /// Deletes from the "users" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteUser(ObjectId objectId)
        {
            return Delete(objectId, beimaDb, userCollection);
        }

        /// <summary>
        /// Updates a user in the "users" collection, given a fully formed updated user.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateUser(BsonDocument doc)
        {
            return Update(doc, beimaDb, userCollection);
        }

        #endregion

        public FilterDefinition<BsonDocument> GetEqualsFilter(string key, dynamic value)
        {
            return Builders<BsonDocument>.Filter.Eq(key, value);
        }
    }
}
