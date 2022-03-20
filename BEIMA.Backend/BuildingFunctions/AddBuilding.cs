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

            Building building;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<AddBuildingRequest>(requestBody);
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
            // TODO: Use actual user.
            building.SetLastModified(DateTime.UtcNow, "Anonymous");

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
