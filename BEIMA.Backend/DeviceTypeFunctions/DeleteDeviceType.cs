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
    /// Handles a delete request involving a single device type.
    /// </summary>
    public static class DeleteDeviceType
    {
        /// <summary>
        /// Handles device type delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device type.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteDeviceType")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device-type/{id}/delete")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a device type delete request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var mongo = MongoDefinition.MongoInstance;
            var deviceTypeId = new ObjectId(id);

            if (!mongo.DeleteDeviceType(deviceTypeId))
            {
                return new NotFoundObjectResult(Resources.DeviceTypeNotFoundMessage);
            }

            return new OkResult();
        }
    }
}
