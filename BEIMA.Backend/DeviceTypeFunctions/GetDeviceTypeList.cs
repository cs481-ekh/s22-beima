using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Collections.Generic;

namespace BEIMA.Backend.DeviceTypeFunctions
{
    /// <summary>
    /// Handles a get request involving multiple device types.
    /// </summary>
    public static class GetDeviceTypeList
    {
        /// <summary>
        /// Handles a device type GET list request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the device type list.</returns>
        [FunctionName("GetDeviceTypeList")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device-type-list")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device type get list request.");

            var mongo = MongoDefinition.MongoInstance;
            var deviceTypes = mongo.GetAllDeviceTypes();
            var dotNetObjList = new List<object>();
            foreach (var deviceType in deviceTypes)
            {
                var dotNetObj = BsonTypeMapper.MapToDotNetValue(deviceType);
                dotNetObjList.Add(dotNetObj);
            }

            return new OkObjectResult(dotNetObjList);
        }
    }
}
