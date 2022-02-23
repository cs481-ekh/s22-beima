using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BEIMA.Backend.Test
{
    /// <summary>
    /// Helps build a custom http request object.
    /// </summary>
    public static class RequestFactory
    {
        /// <summary>
        /// Builds a generic http request from the given request method.
        /// </summary>
        /// <param name="requestMethod">The http request method (GET or POST)</param>
        /// <returns>A new http request object.</returns>
        public static HttpRequest CreateHttpRequest(string requestMethod)
        {
            Assume.That(requestMethod, Is.EqualTo("GET").Or.EqualTo("POST").IgnoreCase);

            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Method = requestMethod;
            return request;
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
    }
}
