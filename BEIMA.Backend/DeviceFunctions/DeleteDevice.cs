using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace BEIMA.Backend.DeviceFunctions
{
    /// <summary>
    /// Handles a delete request involving a single device.
    /// </summary>
    public static class DeleteDevice
    {
        /// <summary>
        /// Handles device delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the device.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteDevice")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device/{id}/delete")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var mongo = MongoDefinition.MongoInstance;
            var deviceId = new ObjectId(id);

            if (!mongo.DeleteDevice(deviceId))
            {
                return new NotFoundObjectResult(Resources.DeviceNotFoundMessage);
            }

            return new OkResult();
        }
    }
}
