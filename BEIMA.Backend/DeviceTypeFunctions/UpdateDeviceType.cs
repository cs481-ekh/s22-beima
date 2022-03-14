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
using System.Net;
using BEIMA.Backend.MongoService;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BEIMA.Backend.DeviceTypeFunctions
{
    public static class UpdateDeviceType
    {
        [FunctionName("UpdateDeviceType")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device-type/{id}/update")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            DeviceType deviceType;
            var mongo = MongoDefinition.MongoInstance;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                deviceType = new DeviceType(new ObjectId(id),
                                            (string)data.name,
                                            (string)data.description,
                                            (string)data.notes);

                var fieldDictionary = ((JObject)data.fields).ToObject<Dictionary<string, string>>();
                var currentDeviceType = mongo.GetDeviceType(new ObjectId(id));
                if (currentDeviceType == null)
                {
                    return new NotFoundObjectResult(Resources.DeviceTypeNotFoundMessage);
                }
                foreach (var field in fieldDictionary)
                {
                    // Check that field ids exist in the current device type.
                    if (!((BsonDocument)currentDeviceType["fields"]).Contains(field.Key))
                    {
                        return new BadRequestObjectResult(Resources.CouldNotParseBody);
                    }
                    deviceType.AddField(new Guid(field.Key).ToString(), field.Value);
                }

                var newFieldList = ((JArray)data.newFields).ToObject<List<string>>();
                foreach (var newField in newFieldList)
                {
                    deviceType.AddField(Guid.NewGuid().ToString(), newField);
                }
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceTypeValid(deviceType, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var updatedDeviceType = mongo.UpdateDeviceType(deviceType.GetBsonDocument());
            if (updatedDeviceType is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }
            var dotNetObj = BsonTypeMapper.MapToDotNetValue(updatedDeviceType);
            return new OkObjectResult(dotNetObj);
        }
    }
}
