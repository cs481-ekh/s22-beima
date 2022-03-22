using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a delete request involving a single device.
    /// </summary>
    public static class DeleteDevice
    {
        /// <summary>
        /// Handles device delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device/{id}/delete")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var mongo = MongoDefinition.MongoInstance;
            var deviceId = new ObjectId(id);

            BsonDocument deviceDocument = mongo.GetDevice(deviceId);

            // Check that the device returned is not null.
            if (deviceDocument is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            // Return the device.
            var device = BsonSerializer.Deserialize<Device>(deviceDocument);
            if (!mongo.DeleteDevice(deviceId))
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            // Create a list of file uids
            List<string> filesToDelete = device.Files.Select(val => val.FileUid).ToList();
            if(device.Photo.FileUid != null)
            {
                filesToDelete.Add(device.Photo.FileUid);
            }

            // Delete files from storage
            var _storage = StorageDefinition.StorageInstance;
            foreach (var fileUid in filesToDelete)
            {
                await _storage.DeleteFile(fileUid);
            }

            return new OkResult();
        }
    }
}
