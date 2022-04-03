using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using BEIMA.Backend.Models;
using System.Net;
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

            User user;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UserRequest>(requestBody);
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
            // TODO: Use actual user.
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsUserValid(user, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
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
