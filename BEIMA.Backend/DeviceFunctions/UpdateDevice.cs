using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Bson;
using BEIMA.Backend.MongoService;
using System.Net;
using BEIMA.Backend.StorageService;
using BEIMA.Backend.Models;
using MongoDB.Bson.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles update requests involving a single device.
    /// </summary>
    public class UpdateDevice
    {

        private readonly IStorageProvider _storage;

        /// <summary>
        /// Constructor for the DeleteDevice Function
        /// </summary>
        /// <param name="storage">Depedency injected storage provider</param>
        public UpdateDevice(IStorageProvider storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Handles device update request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An object representing the updated device.</returns>
        [FunctionName("UpdateDevice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device/{id}/update")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var reqForm = await req.ReadFormAsync();
            var mongo = MongoDefinition.MongoInstance;

            // Parse out necessary data
            UpdateDeviceRequest data;
            Device device;
            ObjectId devieTypeId;
            ObjectId buildingId;
            try
            {
                data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(reqForm["data"]);
                devieTypeId = ObjectId.Parse(data.DeviceTypeId);
                buildingId = ObjectId.Parse(data.Location.BuildingId);

                var deviceDocument = mongo.GetDevice(new ObjectId(id));
                device = BsonSerializer.Deserialize<Device>(deviceDocument);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            // Set Data to new values
            device.DeviceTypeId = devieTypeId;
            device.DeviceTag = data.DeviceTag;
            device.Manufacturer = data.Manufacturer;
            device.ModelNum = data.ModelNum;
            device.SerialNum = data.SerialNum;
            device.YearManufactured = data.YearManufactured;
            device.Notes = data.Notes;
            device.SetLocation(
                buildingId,
                data.Location.Notes,
                data.Location.Latitude,
                data.Location.Longitude
            );

            // Check if there is a new photo, if so remove all others on the device
            // Frontend only supports 1 photo, but a device can contain more
            var updatePhoto = reqForm.Files.Any(file => file.Name == "photos");
            if (updatePhoto && device.Photos.Count > 0)
            {
                foreach(var file in device.Photos)
                {
                    await _storage.DeleteFile(file.FileUid);
                    // Log if file delete failed.
                }                
                device.Photos = new List<DeviceFile>();
            }
            
            // Remove files from device's file list
            device.Files = device.Files.FindAll(file => !data.DeletedFiles.Contains(file.FileUid));

            // Removed deleted files from storage
            foreach (var fileUid in data.DeletedFiles)
            {
                await _storage.DeleteFile(fileUid);
                // Log to file if fails
            }

            // Add new files and photos
            foreach (var file in reqForm.Files)
            {
                var fileUid = await _storage.PutFile(file);
                if (file.Name == "files")
                {
                    device.AddFile(fileUid, file.FileName);
                }
                else if (file.Name == "photos")
                {
                    device.AddPhoto(fileUid, file.FileName);
                }
            }

            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            // TODO: include device type attributes


            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceValid(device, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var updatedDeviceDocument = mongo.UpdateDevice(device.GetBsonDocument());
            if (updatedDeviceDocument is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }
            return new OkObjectResult(device);
        }
    }
}
