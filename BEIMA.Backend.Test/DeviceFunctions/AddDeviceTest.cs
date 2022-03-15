using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
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
            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Create request
            var body = TestData._testDevice;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertDevice(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }

        [Test]
        public async Task NoDevice_AddDeviceWithNoLocation_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Create request
            var body = TestData._testAddDeviceNoLocation;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }
    }
}
