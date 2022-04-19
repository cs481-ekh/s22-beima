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
using BEIMA.Backend.AuthService;

namespace BEIMA.Backend.BuildingFunctions
{
    /// <summary>
    /// Handles update requests involving a single building.
    /// </summary>
    public static class UpdateBuilding
    {
        /// <summary>
        /// Handles building update request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="id">The id of the building.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An object representing the updated building.</returns>
        [FunctionName("UpdateBuilding")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "building/{id}/update")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a building update request.");

            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);

            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            if (!ObjectId.TryParse(id, out _))
            {
                return new BadRequestObjectResult(Resources.InvalidIdMessage);
            }

            Building building;
            try
            {
                // Read body and parse out request data
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<BuildingRequest>(requestBody);

                // Set building properties to new values
                building = new Building(new ObjectId(id), data.Name, data.Number, data.Notes);
                building.SetLocation(
                    data.Location.Latitude,
                    data.Location.Longitude
                );
            }
            catch (Exception)
            {
                return new BadRequestObjectResult(Resources.CouldNotParseBody);
            }

            building.SetLastModified(DateTime.UtcNow, claims.Username);

            string message;
            HttpStatusCode statusCode;
            if (!Rules.IsBuildingValid(building, out message, out statusCode))
            {
                var response = new ObjectResult(message);
                response.StatusCode = (int)statusCode;
                return response;
            }

            var mongo = MongoDefinition.MongoInstance;
            var updatedBuildingDocument = mongo.UpdateBuilding(building.GetBsonDocument());
            if (updatedBuildingDocument is null)
            {
                return new NotFoundObjectResult(Resources.BuildingNotFoundMessage);
            }

            var updatedBuilding = BsonSerializer.Deserialize<Building>(updatedBuildingDocument);

            return new OkObjectResult(updatedBuilding);
        }
    }
}
