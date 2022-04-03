using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

            // Do not delete admin if they are the only admin in the system. Prevents admins from deleting all admin accounts.
            var userDoc = mongo.GetUser(userId);
            if(userDoc == null)
            {
                return new NotFoundObjectResult(Resources.UserNotFoundMessage);
            }

            var userToDelete = BsonSerializer.Deserialize<User>(userDoc);
            if(userToDelete.Role == "admin")
            {
                var adminFilter = MongoFilterGenerator.GetEqualsFilter("role", "admin");
                var adminResults = mongo.GetFilteredUsers(adminFilter);

                if (adminResults.Count <= 1)
                {
                    return new ConflictObjectResult(Resources.CannotDeleteAdminMessage);
                }
            }

            // DeleteUser returned false, meaning it failed, so send a 500 error.
            // We already checked to make sure it existed earlier, so this means something is wrong with Mongo.
            if (!mongo.DeleteUser(userId))
            {
                var response = new ObjectResult(Resources.InternalServerErrorMessage);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

            return new OkResult();
        }
    }
}
