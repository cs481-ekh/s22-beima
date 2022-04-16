using BEIMA.Backend.AuthService;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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
    /// Handles a request to add a single user.
    /// </summary>
    public static class AddUser
    {
        /// <summary>
        /// Handles user create request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the id of the newly created user.</returns>
        [FunctionName("AddUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a user post request.");

            // Verify JWT token
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null || !claims.Role.Equals(Constants.ADMIN_ROLE))
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var mongo = MongoDefinition.MongoInstance;

            User user;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UserRequest>(requestBody);

                // Verify password meets all requirements
                if (string.IsNullOrEmpty(data.Password) || !Regex.Match(data.Password, Constants.PASSWORD_REGEX).Success)
                {
                    return new BadRequestObjectResult(Resources.InvalidPasswordMessage);
                }

                // Enforce username case insensitivity
                data.Username = data.Username.ToLower();

                // Check for username uniqueness
                var filter = MongoFilterGenerator.GetEqualsFilter("username", data.Username);
                if (mongo.GetFilteredUsers(filter).Count > 0)
                {
                    return new ConflictObjectResult(Resources.UsernameAlreadyExistsMessage);
                }

                var passwordHash = BCryptNet.HashPassword(data.Password);
                user = new User(ObjectId.GenerateNewId(),
                                        data.Username,
                                        passwordHash,
                                        data.FirstName,
                                        data.LastName,
                                        data.Role);
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

            var id = mongo.InsertUser(user.GetBsonDocument());

            //InsertUser returned a null result, meaning it failed, so send a 500 error
            if (id == null)
            {
                var response = new ObjectResult(Resources.InternalServerErrorMessage);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                return response;
            }

            return new OkObjectResult(id.ToString());
        }
    }
}
