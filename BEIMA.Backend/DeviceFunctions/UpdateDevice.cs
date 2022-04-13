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
using System.Collections.Generic;
using BEIMA.Backend.AuthService;

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

            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);

            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            Device device;
            DeviceType deviceType;
            IFormCollection reqForm;
            UpdateDeviceRequest data;
            var mongo = MongoDefinition.MongoInstance;
            try
            {
                // Read Form and parse out request data
                reqForm = await req.ReadFormAsync();
                data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(reqForm["data"]);

                // Get original device
                var deviceDocument = mongo.GetDevice(new ObjectId(id));
                if (deviceDocument == null)
                {
                    return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
                }
                device = BsonSerializer.Deserialize<Device>(deviceDocument);

                // Get related device type document
                if (!ObjectId.TryParse(data.DeviceTypeId, out _))
                {
                    return new BadRequestObjectResult(Resources.InvalidIdMessage);
                }
                var deviceTypeId = ObjectId.Parse(data.DeviceTypeId);
                var deviceTypeDocument = mongo.GetDeviceType(deviceTypeId);
                deviceType = BsonSerializer.Deserialize<DeviceType>(deviceTypeDocument);

                // Parse building id if it exists
                var reqBuildingId = data.Location.BuildingId;
                ObjectId? buildingId = reqBuildingId != null ? ObjectId.Parse(reqBuildingId) : null;
                if (buildingId != null && mongo.GetBuilding((ObjectId)buildingId) is null)
                {
                    return new NotFoundObjectResult(Resources.BuildingNotFoundMessage);
                }

                // Set device properties to new values
                device.DeviceTypeId = deviceTypeId;
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

                // Check that each request field matches the device type fields
                if (data.Fields != null)
                {
                    if (data.Fields.Count != deviceType.Fields.ToDictionary().Count)
                    {
                        return new BadRequestObjectResult(Resources.CouldNotParseBody);
                    }

                    foreach (var field in data.Fields)
                    {
                        if (!deviceType.Fields.Contains(field.Key))
                        {
                            return new BadRequestObjectResult(Resources.CouldNotParseBody);
                        }
                    }
                    device.SetFields(data.Fields);
                }
                else if (deviceType.Fields.ToDictionary().Count > 0)
                {
                    return new BadRequestObjectResult(Resources.CouldNotParseBody);
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            List<string> filesToDelete = new List<string>(data.DeletedFiles);

            // Check if there is a new photo to replace the old one
            var updatePhoto = reqForm.Files.Any(file => file.Name == "photo");
            if (updatePhoto && device.Photo != null && device.Photo.FileUid != null)
            {
                filesToDelete.Add(device.Photo.FileUid);
            }

            // Remove files from device's file list
            device.Files = device.Files.FindAll(file => !data.DeletedFiles.Contains(file.FileUid));

            // Add new files and photos
            var _storage = StorageDefinition.StorageInstance;
            foreach (var file in reqForm.Files)
            {
                var fileUid = await _storage.PutFile(file);
                if (file.Name == "files")
                {
                    device.AddFile(fileUid, file.FileName);
                }
                else if (file.Name == "photo")
                {
                    device.SetPhoto(fileUid, file.FileName);
                }
            }

            device.SetLastModified(DateTime.UtcNow, claims.Username);

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceValid(device, deviceType, out message, out statusCode))
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

            // Removed deleted files from storage
            foreach (var fileUid in filesToDelete)
            {
                var result = await _storage.DeleteFile(fileUid);
                if (!result)
                {
                    log.LogInformation($"Failed to delete a file: {fileUid.ToString()}.");
                }
            }

            var updatedDevice = BsonSerializer.Deserialize<Device>(updatedDeviceDocument);

            if (updatedDevice.Photo.FileUid != null)
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
