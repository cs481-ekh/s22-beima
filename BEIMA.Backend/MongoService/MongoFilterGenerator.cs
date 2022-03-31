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
            if(value == null)
            {
                value = BsonNull.Value;
            }
            return Builders<BsonDocument>.Filter.Eq(key, value);
        }

        /// <summary>
        /// Combines multiple passed in filters. Will return one filter with the AND operation applied between all of the passed in filters.
        /// </summary>
        /// <param name="filters">A variable number of filters.</param>
        /// <returns>One filter that has the AND operation applied to all passed in filters.</returns>
        public static FilterDefinition<BsonDocument> CombineFilters(params FilterDefinition<BsonDocument>[] filters)
        {
            return Builders<BsonDocument>.Filter.And(filters);
        }
    }
}
