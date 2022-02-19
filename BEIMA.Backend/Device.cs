using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BEIMA.Backend
{
    /// <summary>
    /// Handles requests involving a single device.
    /// </summary>
    public static class Device
    {
        /// <summary>
        /// Handles device requests
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response correlated with the type of device request made.</returns>
        [FunctionName("Device")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device request.");
            ObjectResult response;

            // Process as device GET request for retrieving device information.
            if (string.Equals(req.Method, "get", StringComparison.OrdinalIgnoreCase))
            {
                string id = req.Query["id"];

                // Check if the id is a valid guid.
                if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out _))
                {
                    response = new BadRequestObjectResult("Invalid id.");
                    return response;
                }

                // Retrieve the device from the database.
                ObjectId oid = new ObjectId(id);
                BsonDocument doc = MongoDefinition.MongoInstance.GetDevice(oid);

                // Check that the device returned is not null.
                if (doc is null)
                {
                    response = new ObjectResult("Device could not be found.");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Return the device.
                response = new OkObjectResult(doc);

            }
            // Process as a device POST request for creating, updating, or deleting a device.
            else if (string.Equals(req.Method, "post", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: implement post operation
                response = new ObjectResult("Not Implemented.");
                response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
            else
            {
                response = new BadRequestObjectResult("Expected a GET or POST request.");
            }

            return response;
        }
    }
}
