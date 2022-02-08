using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using MongoDB.Driver;

namespace BEIMA.Backend
{
    public static class SampleDB
    {
        [FunctionName("SampleDB")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            var connectionString = Environment.GetEnvironmentVariable("AzureCosmosConnection");
            var dbClient = new MongoClient(connectionString);
            var dbList = dbClient.ListDatabaseNames().ToList();

            var json = JsonSerializer.Serialize(dbList);


            return new OkObjectResult(json);
        }
    }
}
