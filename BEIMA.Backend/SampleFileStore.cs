using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

namespace BEIMA.Backend
{
    public static class SampleFileStore
    {
        [FunctionName("SampleFileStore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            
            string path = Directory.GetCurrentDirectory();
            string blobName = "EAStest_600x300.jpg";
            BlobClient blobClient = new BlobClient(connectionString, "documents", blobName);

            // Will write to BEIMA.Backend\bin\Debug
            var stream = File.OpenWrite(path + blobClient.Name);
            blobClient.DownloadTo(stream);
            stream.Close();

            return new OkObjectResult("");
        }
    }
}
