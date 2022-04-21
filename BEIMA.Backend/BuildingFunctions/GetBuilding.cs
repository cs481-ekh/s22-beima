using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace BEIMA.Backend.BuildingFunctions
{
    /// <summary>
    /// Handles GET requests involving a single building.
    /// </summary>
    public static class GetBuilding
    {
        /// <summary>
        /// Handles a building GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the building.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the building information.</returns>
        [FunctionName("GetBuilding")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "building/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a building GET request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            // Check if the id is valid.
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Retrieve the building from the database.
            ObjectId oid = new ObjectId(id);
            BsonDocument doc = MongoDefinition.MongoInstance.GetBuilding(oid);

            // Check that the building returned is not null.
            if (doc is null)
            {
                return new NotFoundObjectResult(Resources.BuildingNotFoundMessage);
            }

            // Return the building.
            var building = BsonSerializer.Deserialize<Building>(doc);
            return new OkObjectResult(building);
        }
    }
}
