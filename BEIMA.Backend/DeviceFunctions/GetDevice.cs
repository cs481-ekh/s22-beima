using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;

namespace BEIMA.Backend
{
    /// <summary>
    /// Handles GET requests involving a single device.
    /// </summary>
    public static class GetDevice
    {
        /// <summary>
        /// Handles a device GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the device information.</returns>
        [FunctionName("GetDevice")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device get request.");

            // Process as device GET request for retrieving device information.
            if (string.Equals(req.Method, "get", StringComparison.OrdinalIgnoreCase))
            {
                // Check if the id is valid.
                if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
                {
                    return new BadRequestObjectResult(Resources.InvalidIdMessage);
                }

                // Retrieve the device from the database.
                ObjectId oid = new ObjectId(id);
                BsonDocument doc = MongoDefinition.MongoInstance.GetDevice(oid);

                // Check that the device returned is not null.
                if (doc is null)
                {
                    return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
                }

                // Return the device.
                var dotNetObj = BsonTypeMapper.MapToDotNetValue(doc);
                return new OkObjectResult(dotNetObj);
            }
            else
            {
                return new BadRequestObjectResult("Expected a GET request.");
            }
        }
    }
}
