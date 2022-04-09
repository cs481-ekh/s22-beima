using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a get request involving multiple devices.
    /// </summary>
    public static class GetDeviceList
    {
        /// <summary>
        /// Handles a device GET list request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the device information.</returns>
        [FunctionName("GetDeviceList")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device-list")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device list request.");

            var mongo = MongoDefinition.MongoInstance;
            var filter = Builders<BsonDocument>.Filter.Empty;

            // If query filter parameters are defined then apply filters
            if (req.Query is not null && req.Query.Count > 0)
            {
                // Parse all device type filters
                var deviceTypeFilters = new List<FilterDefinition<BsonDocument>>();
                if (req.Query.ContainsKey("deviceType"))
                {
                    foreach (var queryId in req.Query["deviceType"].ToArray())
                    {
                        ObjectId id;
                        if (ObjectId.TryParse(queryId, out id))
                        {
                            deviceTypeFilters.Add(MongoFilterGenerator.GetEqualsFilter("deviceTypeId", id));
                        }
                    }
                }

                // Parse all building filters
                var buildingFilters = new List<FilterDefinition<BsonDocument>>();
                if (req.Query.ContainsKey("building"))
                {
                    foreach (var queryId in req.Query["building"].ToArray())
                    {
                        ObjectId id;
                        if (ObjectId.TryParse(queryId, out id))
                        {
                            buildingFilters.Add(MongoFilterGenerator.GetEqualsFilter("location.buildingId", id));
                        }
                    }
                }

                FilterDefinition<BsonDocument> deviceTypeFilter = null;
                FilterDefinition<BsonDocument> buildingFilter = null;

                // Create device type filter
                if (deviceTypeFilters.Count == 1)
                {
                    deviceTypeFilter = deviceTypeFilters.Single();
                }
                else if (deviceTypeFilters.Count > 1)
                {
                    deviceTypeFilter = MongoFilterGenerator.OrFilters(deviceTypeFilters.ToArray());
                }

                // Create building filter
                if (buildingFilters.Count == 1)
                {
                    buildingFilter = buildingFilters.Single();
                }
                else if (buildingFilters.Count > 1)
                {
                    buildingFilter = MongoFilterGenerator.OrFilters(buildingFilters.ToArray());
                }

                // Create the final filter, or leave as empty filter if there are no device type/building filters
                if (deviceTypeFilter != null && buildingFilter != null)
                {
                    filter = MongoFilterGenerator.AndFilters(deviceTypeFilter, buildingFilter);
                }
                else if (deviceTypeFilter != null)
                {
                    filter = deviceTypeFilter;
                }
                else if (buildingFilter != null)
                {
                    filter = buildingFilter;
                }

            }

            var devices = mongo.GetFilteredDevices(filter);

            var dotNetObjList = new List<Device>();
            foreach (var device in devices)
            {
                var dotNetObj = BsonSerializer.Deserialize<Device>(device);
                dotNetObjList.Add(dotNetObj);
            }

            return new OkObjectResult(dotNetObjList);
        }
    }
}
