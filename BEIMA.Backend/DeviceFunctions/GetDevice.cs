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

namespace BEIMA.Backend
{
    /// <summary>
    /// Handles GET requests involving a single device.
    /// </summary>
    public class GetDevice
    {

        private readonly IStorageProvider _storage;

        /// <summary>
        /// Constructor for the GetDevice Function
        /// </summary>
        /// <param name="storage">Depedency injected storage provider</param>
        public GetDevice(IStorageProvider storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Handles a device GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the device information.</returns>
        [FunctionName("GetDevice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device get request.");
            
            // Check if the id is valid.
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Retrieve the device from the database.
            ObjectId oid = new ObjectId(id);
            BsonDocument deviceDocument = MongoDefinition.MongoInstance.GetDevice(oid);

            // Check that the device returned is not null.
            if (deviceDocument is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            // Get device object.
            var device = BsonSerializer.Deserialize<Device>(deviceDocument);

            // Add url to every photo
            foreach(var photo in device.Photos)
            {
                var presignedUrl = await _storage.GetPresignedURL(photo.FileUid);
                photo.Url = presignedUrl;
            }

            // Add url to every file
            foreach (var file in device.Files)
            {
                var presignedUrl = await _storage.GetPresignedURL(file.FileUid);
                file.Url = presignedUrl;
            }

            return new OkObjectResult(device);
            
        }
    }
}
