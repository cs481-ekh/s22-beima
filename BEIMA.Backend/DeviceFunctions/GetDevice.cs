using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Threading.Tasks;

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
        public static async Task<IActionResult> Run(
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
                var mongo = MongoDefinition.MongoInstance;
                ObjectId oid = new ObjectId(id);
                BsonDocument deviceDocument = mongo.GetDevice(oid);

                // Check that the device returned is not null.
                if (deviceDocument is null)
                {
                    return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
                }

                // Return the device.
                var device = BsonSerializer.Deserialize<Device>(deviceDocument);

                var _storage = StorageDefinition.StorageInstance;
                if (device.Photo.FileUid != null)
                {
                    var presignedUrl = await _storage.GetPresignedURL(device.Photo.FileUid);
                    device.Photo.FileUrl = presignedUrl;
                }                

                // Add url to every file
                foreach (var file in device.Files)
                {
                    var url = await _storage.GetPresignedURL(file.FileUid);
                    file.FileUrl = url;
                }

                return new OkObjectResult(device);
            }
            else
            {
                return new BadRequestObjectResult("Expected a GET request.");
            }
        }
    }
}
