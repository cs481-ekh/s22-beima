using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace BEIMA.Backend.UserFunctions
{
    /// <summary>
    /// Handles GET requests involving a single user.
    /// </summary>
    public static class GetUser
    {
        /// <summary>
        /// Handles a user GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the user.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the user information.</returns>
        [FunctionName("GetUser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a user GET request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null || !claims.Role.Equals(Constants.ADMIN_ROLE))
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            // Check if the id is valid.
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Retrieve the user from the database.
            ObjectId oid = new ObjectId(id);
            BsonDocument doc = MongoDefinition.MongoInstance.GetUser(oid);

            // Check that the user returned is not null.
            if (doc is null)
            {
                return new NotFoundObjectResult(Resources.UserNotFoundMessage);
            }

            // Do not return the password on this endpoint.
            var passwordBson = new BsonElement("password", "");
            doc.SetElement(passwordBson);

            // Return the user.
            var user = BsonSerializer.Deserialize<User>(doc);
            return new OkObjectResult(user);
        }
    }
}
