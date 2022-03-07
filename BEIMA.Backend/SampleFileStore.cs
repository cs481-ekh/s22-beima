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
            var files = req.Form.Files;
            var file = req.Form.Files[0];
            var uid = await _storage.PutFile(file);
            var url = await _storage.GetPresignedURL(uid);
            var deleted = await _storage.DeleteFile(uid);


            uid = await _storage.PutFile(file);
            var stream = await _storage.GetFileStream(uid);
            if (stream != null)
            {
                return new FileStreamResult(stream, "application/octet-stream");
            }
            else
            {
                return new BadRequestObjectResult("Invalid file");
            }
        }
    }
}
