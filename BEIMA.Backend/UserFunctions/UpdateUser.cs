using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Bson;
using BEIMA.Backend.MongoService;
using MongoDB.Bson.Serialization;
using BEIMA.Backend.Models;
using System.Net;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BEIMA.Backend.UserFunctions
{
    /// <summary>
    /// Handles update requests involving a single user.
    /// </summary>
    public static class UpdateUser
    {
        /// <summary>
        /// Handles user update request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the user.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An object representing the updated user.</returns>
        [FunctionName("UpdateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/{id}/update")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a user update request.");

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            // Instantiate mongo connector
            var mongo = MongoDefinition.MongoInstance;

            User user;
            try
            {
                // Read body and parse out request data
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedUserRecord = JsonConvert.DeserializeObject<UserRequest>(requestBody);
                var userId = new ObjectId(id);

                // Verify that the user exists
                var originalUserDoc = mongo.GetUser(userId);
                if (originalUserDoc == null)
                {
                    return new NotFoundObjectResult(Resources.UserNotFoundMessage);
                }

                // If the password sent in is null or empty string, then do not update the password.
                if (updatedUserRecord.Password == null || updatedUserRecord.Password == string.Empty)
                {
                    var originalUserRecord = BsonSerializer.Deserialize<User>(originalUserDoc);
                    updatedUserRecord.Password = originalUserRecord.Password;
                }
                else
                {
                    // TODO: Add validation for password length and special characters
                    updatedUserRecord.Password = BCryptNet.HashPassword(updatedUserRecord.Password);
                }

                // Set user properties to new values
                user = new User(new ObjectId(id), updatedUserRecord.Username, updatedUserRecord.Password, 
                    updatedUserRecord.FirstName, updatedUserRecord.LastName, updatedUserRecord.Role);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            // TODO: Use actual username when authentication is implemented.
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsUserValid(user, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var updatedUserDocument = mongo.UpdateUser(user.GetBsonDocument());
            if (updatedUserDocument is null)
            {
                return new NotFoundObjectResult(Resources.UserNotFoundMessage);
            }

            var updatedUser = BsonSerializer.Deserialize<User>(updatedUserDocument);

            return new OkObjectResult(updatedUser);
        }
    }
}
