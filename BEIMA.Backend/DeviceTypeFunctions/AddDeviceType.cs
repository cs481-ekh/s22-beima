using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BEIMA.Backend.DeviceTypeFunctions
{
    /// <summary>
    /// Handles a request to add a single device type.
    /// </summary>
    public static class AddDeviceType
    {
        /// <summary>
        /// Handles a device type POST request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>The id of the newly created device type.</returns>
        [FunctionName("AddDeviceType")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device_type")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device type POST request.");

            DeviceType deviceType;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                deviceType = new DeviceType(ObjectId.GenerateNewId(),
                                            (string)data.name,
                                            (string)data.description,
                                            (string)data.notes);

                // Populate field attributes
                var fieldList = ((JArray)data.fields).ToObject<List<string>>();
                foreach (var field in fieldList)
                {
                    deviceType.AddField(Guid.NewGuid().ToString(), field);
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }
            // TODO: Use actual user.
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceTypeValid(deviceType, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
            var id = mongo.InsertDeviceType(deviceType.GetBsonDocument());

            return new OkObjectResult(id.ToString());
        }
    }
}
