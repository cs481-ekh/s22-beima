using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Net;

namespace BEIMA.Backend.DeviceFunctions
{
    public static class DeleteDevice
    {
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
