using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BEIMA.Backend.Models;
using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using MongoDB.Bson.Serialization;
using BCryptNet = BCrypt.Net.BCrypt;
using MongoDB.Bson;
using System.Collections.Generic;

namespace BEIMA.Backend.AuthFunctions
{
    /// <summary>
    /// Handles a request to login
    /// </summary>
    public static class Login
    {
        /// <summary>
        /// Handles a login request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing a jwt token of the user</returns>
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] HttpRequest req,
            ILogger log)
        {
            List<BsonDocument> userDocs;
            string password;
            try
            {
                // Deserialize request
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<LoginRequest>(requestBody);
                
                if(data == null || data.Username == null || data.Password == null)
                {
                    return new UnauthorizedResult();
                }
                password = data.Password;

                // Get all users with request's username
                var dbServce = MongoDefinition.MongoInstance;
                var usernameFilter = MongoFilterGenerator.GetEqualsFilter("username", data.Username);
                userDocs = dbServce.GetFilteredUsers(usernameFilter);
            } catch (Exception)
            {
                return new UnauthorizedResult();
            }
            
            if(userDocs == null || userDocs.Count == 0)
            {
                return new UnauthorizedResult();
            }

            // Go through the users and find the one whose password hash matches the request's password
            // Doesn't use filter for password as bcrypt hashing creates a unique salt on every hash.
            User user = null;
            for(var i = 0; i < userDocs.Count && user == null; i++)
            {
                var person = BsonSerializer.Deserialize<User>(userDocs[i]);
                if(BCryptNet.Verify(password, person.Password))
                {
                    user = person;
                }
            }

            if(user == null)
            {
                return new UnauthorizedResult();
            }

            // Create a jwt token
            var authService = AuthenticationDefinition.AuthticationInstance;
            var token = authService.CreateToken(user);
            return new OkObjectResult(token);
        }
    }
}
