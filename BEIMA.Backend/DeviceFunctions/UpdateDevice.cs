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
using BEIMA.Backend.Models;
using MongoDB.Bson.Serialization;
using System.Linq;
using BEIMA.Backend.StorageService;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles update requests involving a single device.
    /// </summary>
    public static class UpdateDevice
    {
        /// <summary>
        /// Handles device update request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An object representing the updated device.</returns>
        [FunctionName("UpdateDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device/{id}/update")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Parse out necessary data            
            var mongo = MongoDefinition.MongoInstance;
            IFormCollection reqForm;            
            Device device;
            UpdateDeviceRequest data;
            try
            {
                reqForm = await req.ReadFormAsync();
                data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(reqForm["data"]);                              
                
                var deviceDocument = mongo.GetDevice(new ObjectId(id));
                if (deviceDocument == null)
                {
                    return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
                }

                device = BsonSerializer.Deserialize<Device>(deviceDocument);                
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            if (!ObjectId.TryParse(data.DeviceTypeId, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }            

            // Set Data to new values
            device.DeviceTypeId = ObjectId.Parse(data.DeviceTypeId);
            device.DeviceTag = data.DeviceTag;
            device.Manufacturer = data.Manufacturer;
            device.ModelNum = data.ModelNum;
            device.SerialNum = data.SerialNum;
            device.YearManufactured = data.YearManufactured;
            device.Notes = data.Notes;

            var reqBuildingId = data.Location.BuildingId;
            ObjectId? buildingId = reqBuildingId != null ? ObjectId.Parse(reqBuildingId) : null;
            device.SetLocation(
                buildingId,
                data.Location.Notes,
                data.Location.Latitude,
                data.Location.Longitude
            );

            device.SetFields(data.Fields);

            var _storage = StorageDefinition.StorageInstance;

            // Check if there is a new photo
            var updatePhoto = reqForm.Files.Any(file => file.Name == "photos");
            if (updatePhoto && device.Photo != null)
            {
                var result = await _storage.DeleteFile(device.Photo.FileUid);
                if (!result)
                {
                    log.LogInformation($"Failed to delete a file: {device.Photo.FileUid.ToString()}.");
                }
                device.Photo = null;
            }

            // Remove files from device's file list
            device.Files = device.Files.FindAll(file => !data.DeletedFiles.Contains(file.FileUid));

            // Removed deleted files from storage
            foreach (var fileUid in data.DeletedFiles)
            {
                var result = await _storage.DeleteFile(fileUid);
                if (!result)
                {
                    log.LogInformation($"Failed to delete a file: {fileUid.ToString()}.");
                }
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
                    device.SetPhoto(fileUid, file.FileName);
                }
            }

            device.SetLastModified(DateTime.UtcNow, "Anonymous");


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

            var updatedDevice = BsonSerializer.Deserialize<Device>(updatedDeviceDocument);
            
            if(updatedDevice.Photo.FileUid != null)
            {
                var presignedUrl = await _storage.GetPresignedURL(updatedDevice.Photo.FileUid);
                updatedDevice.Photo.FileUrl = presignedUrl;
            }
            foreach (var file in updatedDevice.Files)
            {
                var url = await _storage.GetPresignedURL(file.FileUid);
                file.FileUrl = url;
            }

            return new OkObjectResult(updatedDevice);
        }
    }
}
