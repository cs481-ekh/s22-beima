using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace BEIMA.Backend.UserFunctions
{
    /// <summary>
    /// Handles a get request involving multiple buildlings.
    /// </summary>
    public static class GetUserList
    {
        /// <summary>
        /// Handles a user GET list request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the user information.</returns>
        [FunctionName("GetUserList")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user-list")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a user list request.");

            var mongo = MongoDefinition.MongoInstance;
            var users = mongo.GetAllUsers();
            var userObjList = new List<User>();
            foreach (var user in users)
            {
                var userObj = BsonSerializer.Deserialize<User>(user);
                // Do not expose the password through this endpoint, return an empty string.
                userObj.Password = "";
                userObjList.Add(userObj);
            }

            return new OkObjectResult(userObjList);
        }
    }
}
