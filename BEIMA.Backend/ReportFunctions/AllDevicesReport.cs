using BEIMA.Backend.AuthService;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.ReportService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Net;

namespace BEIMA.Backend.ReportFunctions
{
    /// <summary>
    /// Handles a get request for an all device report.
    /// </summary>
    public static class AllDevicesReport
    {
        /// <summary>
        /// Handles an all devices report GET request.
        /// </summary>
        /// <param name="req">The http request.</param>
        /// <param name="log">The logger to log to.</param>
        /// <returns>An http response containing the all devices report in a zip file.</returns>
        [FunctionName("AllDevicesReport")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "report/devices")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed an all devices report request.");

            // Authenticate
            var authService = AuthenticationDefinition.AuthenticationInstance;
            var claims = authService.ParseToken(req);
            if (claims == null)
            {
                return new ObjectResult(Resources.UnauthorizedMessage) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            try
            {
                var mongo = MongoDefinition.MongoInstance;
                List<DeviceType> deviceTypes = new List<DeviceType>();
                List<Device> devices = new List<Device>();
                List<Building> buildings = new List<Building>();

                // Retrieve all device types.
                foreach (var deviceType in mongo.GetAllDeviceTypes())
                {
                    deviceTypes.Add(BsonSerializer.Deserialize<DeviceType>(deviceType));
                }

                // Retrieve all devices.
                foreach (var device in mongo.GetAllDevices())
                {
                    devices.Add(BsonSerializer.Deserialize<Device>(device));
                }

                // Retrieve all buildings.
                foreach (var building in mongo.GetAllBuildings())
                {
                    buildings.Add(BsonSerializer.Deserialize<Building>(building));
                }

                // Generate and return zip file.
                var filebytes = ReportWriter.GenerateAllDeviceReports(deviceTypes, devices, buildings);
                return new FileContentResult(filebytes, "application/octet-stream")
                {
                    FileDownloadName = "devices_report.zip"
                };
            }
            catch (Exception)
            {
                var result = new ObjectResult(Resources.InternalServerErrorMessage);
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                return result;
            }
        }
    }
}
