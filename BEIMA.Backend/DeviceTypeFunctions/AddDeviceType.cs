using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device-type")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device type POST request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            DeviceType deviceType;
            try
            {
                // Parse request body into device type object
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

            deviceType.SetLastModified(DateTime.UtcNow, claims.Username);

            // Validate device type properties
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
