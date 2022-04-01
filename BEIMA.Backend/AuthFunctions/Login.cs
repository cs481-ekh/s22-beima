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

namespace BEIMA.Backend.AuthFunctions
{
    public static class Login
    {
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<LoginRequest>(requestBody);


            var dbServce = MongoDefinition.MongoInstance;
            string hashedPassword = data.Password; // Todo wait for bcrypt to be added by other pr
            var usernameFilter = MongoFilterGenerator.GetEqualsFilter("username", data.Username);
            var passwordFilter = MongoFilterGenerator.GetEqualsFilter("password", hashedPassword);
            var userFilter = MongoFilterGenerator.CombineFilters(usernameFilter, passwordFilter);

            var userDocs = dbServce.GetFilteredUsers(userFilter);            
            if(userDocs == null || userDocs.Count == 0)
            {
                return new UnauthorizedResult();
            } else if(userDocs.Count > 1)
            {
                // This should never happen
                return new UnauthorizedResult();
            }

            var user = BsonSerializer.Deserialize<User>(userDocs[0]);
            if(user == null)
            {
                return new UnauthorizedResult();
            }

            var authService = AuthenticationDefinition.AuthticationInstance;
            var token = authService.CreateToken(user);
            return new OkObjectResult(token);
        }
    }
}
