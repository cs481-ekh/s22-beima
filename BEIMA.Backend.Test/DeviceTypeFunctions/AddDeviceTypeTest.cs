using BEIMA.Backend.DeviceTypeFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceTypeFunctions
{
    [TestFixture]
    public class AddDeviceTypeTest : UnitTestBase
    {
        [Test]
        public async Task NoDeviceType_AddDeviceType_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertDeviceType(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Create request
            var body = TestData._testDeviceType;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var deviceTypeId = ((ObjectResult)await AddDeviceType.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(deviceTypeId);
            Assert.That(ObjectId.TryParse(deviceTypeId, out _), Is.True);
        }
    }
}
