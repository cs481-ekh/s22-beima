using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Linq;

namespace BEIMA.Backend.UserFunctions
{
    /// <summary>
    /// Handles a delete request involving a single user.
    /// </summary>
    public static class DeleteUser
    {
        /// <summary>
        /// Handles user delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the user.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/{id}/delete")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a user delete request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var mongo = MongoDefinition.MongoInstance;
            var userId = new ObjectId(id);

            // Do not delete if at least one device exists with this user.
            // TODO: Use database filter for devices instead of getting the list of all users.
            if (mongo.GetAllDevices().Where(d => d["location"]["userId"].AsObjectId.Equals(userId)).Any())
            {
                return new ConflictObjectResult(Resources.CannotDeleteUserMessage);
            }

            if (!mongo.DeleteUser(userId))
            {
                return new NotFoundObjectResult(Resources.UserNotFoundMessage);
            }

            return new OkResult();
        }
    }
}
