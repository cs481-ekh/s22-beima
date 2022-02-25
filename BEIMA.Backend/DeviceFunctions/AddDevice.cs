using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BEIMA.Backend.DeviceFunctions
{
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
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                device = new Device(ObjectId.GenerateNewId(),
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
                // TODO: include device type attributes
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

            var mongo = MongoDefinition.MongoInstance;
            var id = mongo.InsertDevice(device.GetBsonDocument());

            return new OkObjectResult(id.ToString());
        }
    }
}
