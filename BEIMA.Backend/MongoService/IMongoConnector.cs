using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
