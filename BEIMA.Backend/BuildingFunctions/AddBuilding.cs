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
using System.Threading.Tasks;

namespace BEIMA.Backend.BuildingFunctions
{
    /// <summary>
    /// Handles a request to add a single building.
    /// </summary>
    public static class AddBuilding
    {
        /// <summary>
        /// Handles building create request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the id of the newly created building.</returns>
        [FunctionName("AddBuilding")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "building")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a building post request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            Building building;
            try
            {
                // Parse building object from request.
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<BuildingRequest>(requestBody);
                building = new Building(ObjectId.GenerateNewId(),
                                        data.Name,
                                        data.Number,
                                        data.Notes);
                building.SetLocation(data.Location.Latitude, data.Location.Longitude);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }
            building.SetLastModified(DateTime.UtcNow, claims.Username);

            // Validate building properties.
            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsBuildingValid(building, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
            var id = mongo.InsertBuilding(building.GetBsonDocument());

            return new OkObjectResult(id.ToString());
        }
    }
}
