using BEIMA.Backend.DeviceTypeFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceTypeFunctions
{
    public class DeleteDeviceTypeTest : UnitTestBase
    {
        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("jddkkslo9402mdjgkflsnxjf")]
        public void IdIsInvalid_DeleteDeviceType_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteDeviceType.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDeviceType(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public void IdNotInDatabase_DeleteDeviceType_ReturnsNotFound()
        {
            // ARRANGE
            var testId = ObjectId.GenerateNewId().ToString();

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(false)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDeviceType(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void DeviceWithDeviceTypeExists_DeleteDeviceType_DeletionStopped()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            var device = new Device();
            device.DeviceTypeId = new ObjectId(testId);

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument> { device.ToBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDeviceType(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ConflictObjectResult)));
            Assert.That(((ConflictObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public void IdIsValid_DeleteDeviceType_DeletionSuccessful()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDeviceType(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkResult)));
            Assert.That(((OkResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
