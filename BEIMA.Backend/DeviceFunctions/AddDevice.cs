using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a request to add a single device.
    /// </summary>
    public class AddDevice
    {
        private readonly IStorageProvider _storage;

        /// <summary>
        /// Constructor for the AddDevice Function
        /// </summary>
        /// <param name="storage">Depedency injected storage provider</param>
        public AddDevice(IStorageProvider storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Handles device create request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the id of the newly created device.</returns>
        [FunctionName("AddDevice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device post request.");

            var reqForm = await req.ReadFormAsync();

            Device device;
            try
            {
                var data = JsonConvert.DeserializeObject<AddDeviceRequest>(reqForm["data"]);
                device = new Device(
                    ObjectId.GenerateNewId(),
                    ObjectId.Parse(data.DeviceTypeId),
                    data.DeviceTag,
                    data.Manufacturer,
                    data.ModelNum,
                    data.SerialNum,
                    data.YearManufactured,
                    data.Notes
                );

                device.SetLocation(
                    ObjectId.Parse(data.Location.BuildingId),
                    data.Location.Notes,
                    data.Location.Latitude,
                    data.Location.Longitude
                );

                device.Fields = data.Fields;                
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            // Store attached files and add file uid to device
            foreach (var file in reqForm.Files)
            {
                if (file.Name == "photos" && file.Length > 0)
                {
                    var fileUid = await _storage.PutFile(file);
                    device.SetPhoto(fileUid, file.FileName);
                }
                else if (file.Name == "files" && file.Length > 0)
                {
                    var fileUid = await _storage.PutFile(file);
                    device.AddFile(fileUid, file.FileName);
                }                
            }

            // TODO: Use actual user.
            device.SetLastModified(DateTime.UtcNow, "Anonymous");

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceValid(device, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
            var deviceDocument = device.GetBsonDocument();
            var id = mongo.InsertDevice(deviceDocument);

            return new OkObjectResult(id.ToString());
        }
    }
}
