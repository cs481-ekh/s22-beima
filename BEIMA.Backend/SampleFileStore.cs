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
using BEIMA.Backend.StorageService;
using System.Collections.Generic;

namespace BEIMA.Backend
{
    public class SampleFileStore
    {
        private readonly IStorageProvider _storage;

        public SampleFileStore(IStorageProvider storage)
        {
            _storage = storage;
        }


        [FunctionName("SampleFileStore")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");
            var file = req.Form.Files[0];
            var uid = await _storage.PutFile(file);
            var url = await _storage.GetPresignedURL(uid);
            var stream = await _storage.GetFileStream(uid);
            if(stream != null)
            {
                return new FileStreamResult(stream, "application/octet-stream");
            } else
            {
                return new BadRequestObjectResult("Expected a GET request.");
            }
            
            
            

            
            //var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            //req.Form.Files.
            //await _storage.GetAllFiles();
            //string path = Directory.GetCurrentDirectory();
            //string blobName = "EAStest_600x300.jpg";
            //BlobClient blobClient = new BlobClient(connectionString, "documents", blobName);

            //// Will write to BEIMA.Backend\bin\Debug
            //var stream = File.OpenWrite(path + blobClient.Name);
            //blobClient.DownloadTo(stream);
            //stream.Close();

            //return new OkObjectResult("");
        }
    }
}
