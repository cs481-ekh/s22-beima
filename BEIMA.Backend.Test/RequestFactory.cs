using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace BEIMA.Backend.Test
{
    /// <summary>
    /// Helps build a custom http request object.
    /// </summary>
    public static class RequestFactory
    {
        public enum RequestMethod
        {
            GET,
            POST
        }

        /// <summary>
        /// Builds a generic http request from the given request method.
        /// </summary>
        /// <param name="requestMethod">The http request method (GET or POST)</param>
        /// <returns>A new http request object.</returns>
        public static HttpRequest CreateHttpRequest(RequestMethod requestMethod, Dictionary<string, StringValues>? query = null, string body = "")
        {
            var reqMock = new Mock<HttpRequest>();
            reqMock.Setup(req => req.Method).Returns(requestMethod.ToString());

            if (query != null)
            {
                reqMock.Setup(req => req.Query).Returns(new QueryCollection(query));
            }

            if (requestMethod == RequestMethod.POST && !string.IsNullOrEmpty(body))
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(body);
                writer.Flush();
                stream.Position = 0;
                reqMock.Setup(req => req.Body).Returns(stream);
            }
            return reqMock.Object;
        }
    }
}
