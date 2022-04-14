using BEIMA.Backend.AuthService;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

            // Verify JWT token
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null || !claims.Role.Equals(Constants.ADMIN_ROLE))
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

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
                var originalUserRecord = BsonSerializer.Deserialize<User>(originalUserDoc);

                // Check for username uniqueness
                if (originalUserRecord.Username != updatedUserRecord.Username)
                {
                    var filter = MongoFilterGenerator.GetEqualsFilter("username", updatedUserRecord.Username);
                    if (mongo.GetFilteredUsers(filter).Count > 0)
                    {
                        return new ConflictObjectResult(Resources.UsernameAlreadyExistsMessage);
                    }
                }

                // If the password sent in is null or empty string, then do not update the password.
                if (string.IsNullOrEmpty(updatedUserRecord.Password))
                {
                    updatedUserRecord.Password = originalUserRecord.Password;
                }
                else
                {
                    // Verify password meets all requirements
                    if (!Regex.Match(updatedUserRecord.Password, Constants.PASSWORD_REGEX).Success)
                    {
                        return new BadRequestObjectResult(Resources.InvalidPasswordMessage);
                    }
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

            user.SetLastModified(DateTime.UtcNow, claims.Username);

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsUserValid(user, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var updatedUserDocument = mongo.UpdateUser(user.GetBsonDocument());

            // UpdateUser returned null, meaning it failed, so send a 500 error.
            // We already checked to make sure it existed earlier, so this means something is wrong with Mongo.
            if (updatedUserDocument is null)
            {
                var response = new ObjectResult(Resources.InternalServerErrorMessage);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

            var updatedUser = BsonSerializer.Deserialize<User>(updatedUserDocument);

            //Do not expose password.
            updatedUser.Password = string.Empty;

            return new OkObjectResult(updatedUser);
        }
    }
}
