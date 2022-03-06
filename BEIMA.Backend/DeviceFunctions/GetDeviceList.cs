using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Collections.Generic;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a get request involving multiple devices.
    /// </summary>
    public static class GetDeviceList
    {
        /// <summary>
        /// Handles a device GET list request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the device information.</returns>
        [FunctionName("GetDeviceList")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device-list")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device list request.");

            var mongo = MongoDefinition.MongoInstance;
            var devices = mongo.GetAllDevices();
            var dotNetObjList = new List<object>();
            foreach (var device in devices)
            {
                var dotNetObj = BsonTypeMapper.MapToDotNetValue(device);
                dotNetObjList.Add(dotNetObj);
            }

            return new OkObjectResult(dotNetObjList);
        }
    }
}
