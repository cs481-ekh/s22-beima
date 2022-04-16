using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace BEIMA.Backend.DeviceTypeFunctions
{
    /// <summary>
    /// Handles GET requests involving a single device type.
    /// </summary>
    public static class GetDeviceType
    {
        /// <summary>
        /// Handles a device type GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device type.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response contataining the device type information.</returns>
        [FunctionName("GetDeviceType")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "device-type/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device type get request.");

            // Check if the id is valid.
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Retrieve the device type from the database.
            ObjectId oid = new ObjectId(id);
            var mongo = MongoDefinition.MongoInstance;
            BsonDocument doc = mongo.GetDeviceType(oid);

            // Check that the device type returned is not null.
            if (doc is null)
            {
                return new NotFoundObjectResult(Resources.DeviceTypeNotFoundMessage);
            }

            // Find the total number of devices that are match this deviceType, and return the count of devices in the return object.
            var filter = MongoFilterGenerator.GetEqualsFilter("deviceTypeId", doc.GetValue("_id"));
            var deviceCount = mongo.GetFilteredDevices(filter).Count;
            doc.Add("count", deviceCount);

            // Return the device type.
            var dotNetObj = BsonTypeMapper.MapToDotNetValue(doc);
            return new OkObjectResult(dotNetObj);
        }
    }
}
