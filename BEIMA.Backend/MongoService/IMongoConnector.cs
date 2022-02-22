﻿using MongoDB.Bson;
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
    }
}