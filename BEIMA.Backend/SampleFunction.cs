using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BEIMA.Backend
{
    public class SampleCommand
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
    }

    public static class SampleFunction
    {

        /*
         * The function name tag[] is the name of this endpoint
         * Http Trigger means its triggered by an http call
         * AuthorizationLevel.Anonymous means that there is no built in auth, anyone can hit it. To clamp it down we will check the httpRequest headers for the auth token
         * post meand that only a post can hit this endpoint
         * Route means the route this endpoint exists on
         * HttpRequest is the request body. You can swap this out with a class that maps to the post body and it will auto map for you, but we lose the headers.
         * Instead just follow the streamreader jsoncovert, 
         * 
         */

        [FunctionName("SampleFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sample")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SampleCommand>(requestBody);

            string responseMessage = string.IsNullOrEmpty(data.ItemName)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {data.ItemName}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
