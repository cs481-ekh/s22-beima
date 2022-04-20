using BEIMA.Backend.AuthService;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a request to add a single device.
    /// </summary>
    public static class AddDevice
    {
        /// <summary>
        /// Handles device create request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the id of the newly created device.</returns>
        [FunctionName("AddDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device post request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            Device device;
            DeviceType deviceType;
            IFormCollection reqForm;
            var mongo = MongoDefinition.MongoInstance;
            try
            {
                reqForm = await req.ReadFormAsync();
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

                // Check that building id is valid (allow null building ids)
                var reqBuildingId = data.Location.BuildingId;
                ObjectId? buildingId = reqBuildingId != null ? ObjectId.Parse(reqBuildingId) : null;
                if (buildingId != null && mongo.GetBuilding((ObjectId)buildingId) is null)
                {
                    return new NotFoundObjectResult(Resources.BuildingNotFoundMessage);
                }

                device.SetLocation(
                    buildingId,
                    data.Location.Notes,
                    data.Location.Latitude,
                    data.Location.Longitude
                );

                // Check that each custom field is a valid device type custom field.
                deviceType = BsonSerializer.Deserialize<DeviceType>(mongo.GetDeviceType(device.DeviceTypeId));
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

            device.SetLastModified(DateTime.UtcNow, claims.Username);

            // Store attached files and add file uid to device
            var _storage = StorageDefinition.StorageInstance;

            foreach (var file in reqForm.Files)
            {
                if (file.Name == "photo" && file.Length > 0)
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

            // Validate device properties
            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceValid(device, deviceType, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var id = mongo.InsertDevice(device.GetBsonDocument());

            return new OkObjectResult(id.ToString());
        }
    }
}
