using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
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
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();

            // Create request
            var body = TestData._testDevice;
            var temp = CreateHttpRequest
            var request = CreateHttpRequest(RequestMethod.POST){
                
            };
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var deviceId = ((ObjectResult) await new AddDevice(storageProvider).Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }
    }
}
