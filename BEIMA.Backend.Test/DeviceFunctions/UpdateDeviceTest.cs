using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class UpdateDeviceTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateDevice_ReturnsNotFound()
        {
            // ARRANGE

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.UpdateDevice(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup storage provider.
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();

            // Set up the http request.
            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateDevice);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await new UpdateDevice(storageProvider).Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Device could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateDevice(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup storage provider.
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateDevice);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await new UpdateDevice(storageProvider).Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingDevice_UpdatesDevice_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var device = new Device(new ObjectId(testId), new ObjectId("12341234abcdabcd43214321"), "A-3", "Generic Inc.", "1234", "abcd1234", 2004, "Some notes.");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetLocation(new ObjectId("111111111111111111111111"), "Some notes.", "123.456", "101.101");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()))
                  .Returns(device.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup storage provider.
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            var storageProvider = serviceProivder.GetRequiredService<IStorageProvider>();

            var body = TestData._testUpdateDevice;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await new UpdateDevice(storageProvider).Run(request, testId, logger);

            // ASSERT
            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var deviceBson = (Dictionary<string, object>)((OkObjectResult)response).Value;
            Assert.That(deviceBson["serialNum"], Is.EqualTo(device.SerialNum));
        }
    }
}
