using BEIMA.Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        public static HttpRequest CreateMultiPartHttpRequest(string data)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            var formKeys = new Dictionary<string, StringValues>();
            formKeys.Add("data", data);

            var formFiles = new FormFileCollection();
            var fileStream = new ByteArrayContent(Encoding.ASCII.GetBytes("TestOne")).ReadAsStream();
            var photoStream = new ByteArrayContent(Encoding.ASCII.GetBytes("TestTwo")).ReadAsStream();
            formFiles.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));
            formFiles.Add(new FormFile(photoStream, 0, photoStream.Length, "photos", "photo.txt"));

            var form = new FormCollection(formKeys, formFiles);
            httpContext.Request.Form = form;
            return httpContext.Request;
        }
    }
}
