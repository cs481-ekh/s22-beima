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

            Device device;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UpdateDeviceRequest>(requestBody);
                device = new Device(
                     ObjectId.Parse(id),
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

                device.SetFields(data.Fields);

                device.SetLastModified(DateTime.UtcNow, "Anonymous");
                // TODO: include device type attributes
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceValid(device, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
            var updatedDevice = mongo.UpdateDevice(device.GetBsonDocument());
            if (updatedDevice is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }
            var dotNetObj = BsonSerializer.Deserialize<Device>(updatedDevice);
            return new OkObjectResult(dotNetObj);
        }
    }
}
