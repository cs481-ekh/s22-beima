using BEIMA.Backend.BuildingFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.BuildingFunctions
{
    [TestFixture]
    public class DeleteBuildingTest : UnitTestBase
    {
        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("jddkkslo9402mdjgkflsnxjf")]
        public void IdIsInvalid_DeleteBuilding_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteBuilding.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteBuilding(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public void IdNotInDatabase_DeleteBuilding_ReturnsNotFound()
        {
            // ARRANGE
            var testId = ObjectId.GenerateNewId().ToString();

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteBuilding(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(false)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(new List<BsonDocument> {})
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteBuilding(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Building could not be found."));
        }

        [Test]
        public void DeviceWithBuildingExists_DeleteBuilding_DeletionStopped()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            var device = new Device();
            device.Location.BuildingId = new ObjectId(testId);

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument> { device.ToBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteBuilding(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(new List<BsonDocument> { device.ToBsonDocument() })
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteBuilding(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ConflictObjectResult)));
            Assert.That(((ConflictObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
            Assert.That(((ConflictObjectResult)response).Value,
                        Is.EqualTo("The building could not be deleted because at least one device exists in the database with this building."));
        }

        [Test]
        public void IdIsValid_DeleteBuilding_DeletionSuccessful()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteBuilding(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(new List<BsonDocument> {})
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteBuilding(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkResult)));
            Assert.That(((OkResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
