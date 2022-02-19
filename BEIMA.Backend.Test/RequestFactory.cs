using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace BEIMA.Backend.Test
{
    /// <summary>
    /// Helps build a custom http request object.
    /// </summary>
    public static class RequestFactory
    {
        /// <summary>
        /// Builds a dictionary to store the query string key and value.
        /// </summary>
        /// <param name="key">Query string key.</param>
        /// <param name="value">Query string value.</param>
        /// <returns>Dictionary holding query string parameters.</returns>
        private static Dictionary<string, StringValues> CreateDictionary(string key, string value)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key, value }
            };
            return qs;
        }

        /// <summary>
        /// Builds an http request object from the given parameters.
        /// </summary>
        /// <param name="queryStringKey">Query string key.</param>
        /// <param name="queryStringValue">Query string value associated with the key.</param>
        /// <returns>A new http request object.</returns>
        public static HttpRequest CreateHttpRequest(string queryStringKey, string queryStringValue)
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Query = new QueryCollection(CreateDictionary(queryStringKey, queryStringValue));
            return request;
        }
    }
}
