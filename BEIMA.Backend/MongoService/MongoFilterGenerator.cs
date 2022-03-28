using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// Generates filters to be passed into MongoConnector.GetFiltered methods.
    /// </summary>
    public static class MongoFilterGenerator
    {
        /// <summary>
        /// Gets an equals (==) filter given a key and value in a BsonDocument.
        /// </summary>
        /// <param name="key">Key of BsonDocument property</param>
        /// <param name="value">Value of BsonDocument property.</param>
        /// <returns>An "equals" filter of type FilterDefinition for BsonDocument</returns>
        public static FilterDefinition<BsonDocument> GetEqualsFilter(string key, dynamic value)
        {
            return Builders<BsonDocument>.Filter.Eq(key, value);
        }
    }
}
