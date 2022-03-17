using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
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

            Device device;
            var mongo = MongoDefinition.MongoInstance;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<AddDeviceRequest>(requestBody);
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

                var reqBuildingId = data.Location.BuildingId;
                ObjectId? buildingId = reqBuildingId != null ? ObjectId.Parse(reqBuildingId) : null;

                device.SetLocation(
                    buildingId,
                    data.Location.Notes,
                    data.Location.Latitude,
                    data.Location.Longitude
                );

                // Check that each field is a valid device type field.
                if (data.Fields != null)
                {
                    var deviceType = BsonSerializer.Deserialize<DeviceType>(mongo.GetDeviceType(device.DeviceTypeId));

                    foreach (var field in data.Fields)
                    {
                        if (!deviceType.Fields.Contains(field.Key))
                        {
                            return new BadRequestObjectResult(Resources.CouldNotParseBody);
                        }
                    }
                    device.SetFields(data.Fields);
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
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

            var id = mongo.InsertDevice(device.GetBsonDocument());

            return new OkObjectResult(id.ToString());
        }
    }
}
