using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            DeviceType deviceType;
            List<Device> updatedDevices = new List<Device>();
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

                var removedFields = currentDeviceType["fields"].AsBsonDocument.ToDictionary().Keys.Where(k => !fieldDictionary.ContainsKey(k));

                var newFieldList = ((JArray)data.newFields).ToObject<List<string>>();
                var addedFields = new List<string>();
                foreach (var newField in newFieldList)
                {
                    var newGuid = Guid.NewGuid().ToString();
                    deviceType.AddField(newGuid, newField);
                    addedFields.Add(newGuid);
                }

                // Updated the devices affected by this device type change.
                if (removedFields.Count() > 0 || addedFields.Count() > 0)
                {
                    var affectedDevices = mongo.GetFilteredDevices(MongoFilterGenerator.GetEqualsFilter("deviceTypeId", deviceType.Id));
                    foreach (var device in affectedDevices)
                    {
                        var deviceObj = BsonSerializer.Deserialize<Device>(device);

                        // Update removed fields.
                        if (removedFields.Count() > 0)
                        {
                            foreach (var field in removedFields)
                            {
                                deviceObj.Fields.Remove(field);
                            }
                        }

                        // Update added fields.
                        if (addedFields.Count() > 0)
                        {
                            foreach (var field in addedFields)
                            {
                                deviceObj.Fields.Add(field, null);
                            }
                        }

                        // Add the devices to a list and wait to update them until
                        // the actual device type is updated in the database.
                        updatedDevices.Add(deviceObj);
                    }
                }

            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            deviceType.SetLastModified(DateTime.UtcNow, claims.Username);

            // Validate device type properties.
            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsDeviceTypeValid(deviceType, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            // Update the device type in the db
            var updatedDeviceType = mongo.UpdateDeviceType(deviceType.GetBsonDocument());
            if (updatedDeviceType is null)
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }
            // Update the affected devices in the db.
            updatedDevices.ForEach(d => mongo.UpdateDevice(d.GetBsonDocument()));

            var dotNetObj = BsonTypeMapper.MapToDotNetValue(updatedDeviceType);
            return new OkObjectResult(dotNetObj);
        }
    }
}
