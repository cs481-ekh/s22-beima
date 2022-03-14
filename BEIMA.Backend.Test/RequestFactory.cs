﻿using BEIMA.Backend.Models;
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

        /// <summary>
        /// Builds a generic multipart http request from the given request method
        /// </summary>
        /// <param name="data">Json data to be sent in the request</param>
        /// <param name="files">Collection of files to be sent in the request</param>
        /// <returns></returns>
        public static HttpRequest CreateMultiPartHttpRequest(string data, FormFileCollection? files = null)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            var formKeys = new Dictionary<string, StringValues>();
            formKeys.Add("data", data);

            var formFiles = new FormFileCollection();  
            if(files != null)
            {
                foreach (var file in files)
                {
                    formFiles.Add(file);
                }
            }

            var form = new FormCollection(formKeys, formFiles);
            httpContext.Request.Form = form;
            return httpContext.Request;
        }
    }
}
