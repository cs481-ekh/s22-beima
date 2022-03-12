using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a delete request involving a single device.
    /// </summary>
    public class DeleteDevice
    {
        private readonly IStorageProvider _storage;

        /// <summary>
        /// Constructor for the DeleteDevice Function
        /// </summary>
        /// <param name="storage">Depedency injected storage provider</param>
        public DeleteDevice(IStorageProvider storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Handles device delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteDevice")]
        public async Task<IActionResult> Run(
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
            var deviceDocument = mongo.GetDevice(deviceId);
            if(deviceDocument == null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            var device = BsonSerializer.Deserialize<Device>(deviceDocument);

            // Remove Files
            foreach (var file in device.Files)
            {
                await _storage.DeleteFile(file.FileUid);
                // Log to file if delete failed
            }
            foreach (var photo in device.Photos)
            {
                await _storage.DeleteFile(photo.FileUid);
                // Log to file if delete failed
            }

            if (!mongo.DeleteDevice(deviceId))
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            return new OkResult();
        }
    }
}
