using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace BEIMA.Backend.BuildingFunctions
{
    /// <summary>
    /// Handles a get request involving multiple buildings.
    /// </summary>
    public static class GetBuildingList
    {
        /// <summary>
        /// Handles a building GET list request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the building information.</returns>
        [FunctionName("GetBuildingList")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "building-list")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a building list request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = 401 };
            }

            var mongo = MongoDefinition.MongoInstance;
            var buildings = mongo.GetAllBuildings();

            // Add each building to the list.
            var buildingObjList = new List<Building>();
            foreach (var building in buildings)
            {
                var buildingObj = BsonSerializer.Deserialize<Building>(building);
                buildingObjList.Add(buildingObj);
            }

            return new OkObjectResult(buildingObjList);
        }
    }
}
