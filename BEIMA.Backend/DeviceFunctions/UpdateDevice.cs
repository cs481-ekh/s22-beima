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
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                device = new Device(new ObjectId(id),
                                    ObjectId.Parse((string)data.deviceTypeId),
                                    (string)data.deviceTag,
                                    (string)data.manufacturer,
                                    (string)data.modelNum,
                                    (string)data.serialNum,
                                    (int)data.yearManufactured,
                                    (string)data.notes);

                device.SetLocation(ObjectId.Parse((string)data.location.buildingId),
                                   (string)data.location.notes,
                                   (string)data.location.latitude,
                                   (string)data.location.longitude);

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
            var dotNetObj = BsonTypeMapper.MapToDotNetValue(updatedDevice);
            return new OkObjectResult(dotNetObj);
        }
    }
}
