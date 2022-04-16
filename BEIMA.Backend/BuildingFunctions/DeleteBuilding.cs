using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Linq;

namespace BEIMA.Backend.BuildingFunctions
{
    /// <summary>
    /// Handles a delete request involving a single building.
    /// </summary>
    public static class DeleteBuilding
    {
        /// <summary>
        /// Handles building delete request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the building.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response indicating whether or not the deletion was successful.</returns>
        [FunctionName("DeleteBuilding")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "building/{id}/delete")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a building delete request.");

            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);

            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            var mongo = MongoDefinition.MongoInstance;
            var buildingId = new ObjectId(id);

            // Do not delete if at least one device exists with this building.
            var filter = MongoFilterGenerator.GetEqualsFilter("location.buildingId", buildingId);
            if (mongo.GetFilteredDevices(filter).Count > 0)
            {
                return new ConflictObjectResult(Resources.CannotDeleteBuildingMessage);
            }

            if (!mongo.DeleteBuilding(buildingId))
            {
                return new NotFoundObjectResult(Resources.BuildingNotFoundMessage);
            }

            return new OkResult();
        }
    }
}
