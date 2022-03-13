using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using BEIMA.Backend.Test.StorageService;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class AddDeviceTest : UnitTestBase
    {
        [Test]
        public async Task NoDevice_AddDevice_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup storage provider.
            var storageProvider = StorageProviderExtensions.CreateAzureStorageProvider();

            // Create request
            var data = new AddDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "622cf00109137c26f913b282",
                Manufacturer = "man",
                ModelNum = "mod",
                SerialNum = "ser",
                Notes = "notes",
                Location = new Location()
                {
                    BuildingId = "622cf00109137c26f913b281",
                    Notes = "notes",
                    Latitude = "1231232",
                    Longitude = "123213213"
                },
                Fields = new Dictionary<string, string>()
            };
            data.Fields.Add("customIdOne", "valueOne");
            data.Fields.Add("customIdTwo", "valueTwo");

            var json = JsonConvert.SerializeObject(data);

            var request =  CreateMultiPartHttpRequest(json);
            
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var deviceId = ((ObjectResult) await new AddDevice(storageProvider).Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }
    }
}
