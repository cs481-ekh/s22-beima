using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// This interface abstracts basic CRUD operations for MongoDB.
    /// </summary>
    public interface IMongoConnector
    {
        public static MongoConnector Instance { get; }

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetDevice(ObjectId objectId);

        /// <summary>
        /// Gets a device from the "devices" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDevices();

        /// <summary>
        /// Inserts a device into the "devices" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDevice(BsonDocument doc);

        /// <summary>
        /// Deletes from the "devices" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDevice(ObjectId objectId);

        /// <summary>
        /// Updates a device in the "devices" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDevice(BsonDocument doc);

        /// <summary>
        /// Gets a device type from the "deviceTypes" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetDeviceType(ObjectId objectId);

        /// <summary>
        /// Gets a device from the "deviceTypes" collection, given an objectID.
        /// </summary>
        /// <returns>BsonDocument that was requested</returns>
        public List<BsonDocument> GetAllDeviceTypes();

        /// <summary>
        /// Inserts a device into the "deviceTypes" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed device type document (including all required and optional fields)</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertDeviceType(BsonDocument doc);

        /// <summary>
        /// Deletes from the "deviceTypes" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteDeviceType(ObjectId objectId);

        /// <summary>
        /// Updates a device in the "deviceTypes" collection, given a fully formed updated device.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>true if successful, false if unsuccessful</returns>
        public BsonDocument UpdateDeviceType(BsonDocument doc);

        /// <summary>
        /// Gets a Building from the "buildings" collection, given an objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>BsonDocument that was requested</returns>
        public BsonDocument GetBuilding(ObjectId objectId);

        /// <summary>
        /// Gets all buildings from the "buildings" collection.
        /// </summary>
        /// <returns>List of all building BsonDocuments</returns>
        public List<BsonDocument> GetAllBuildings();

        /// <summary>
        /// Inserts a building into the "buildings" collection
        /// </summary>
        /// <param name="doc">BsonDocument that contains the fully formed building document</param>
        /// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
        public ObjectId? InsertBuilding(BsonDocument doc);

        /// <summary>
        /// Deletes from the "buildings" collection, given the objectID.
        /// </summary>
        /// <param name="objectId">Corresponds to the "_id" field for a given document inside of MongoDB</param>
        /// <returns>true if successful, false if not successful</returns>
        public bool DeleteBuilding(ObjectId objectId);

        /// <summary>
        /// Gets all devices that match the parameter filter's criteria
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        
        public List<BsonDocument> GetFilteredDevices(FilterDefinition<BsonDocument> filter);
        /// Updates a building in the "buildings" collection, given a fully formed updated building.
        /// </summary>
        /// <param name="doc">BsonDocument containing the updated BsonDocument.</param>
        /// <returns>The updated BsonDocument, or null if nothing was updated.</returns>
        public BsonDocument UpdateBuilding(BsonDocument doc);
    }
}
