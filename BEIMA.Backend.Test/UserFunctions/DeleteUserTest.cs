using BEIMA.Backend.UserFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;
using MongoDB.Driver;

namespace BEIMA.Backend.Test.UserFunctions
{
    [TestFixture]
    public class DeleteUserTest : UnitTestBase
    {
        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("jddkkslo9402mdjgkflsnxjf")]
        public void IdIsInvalid_DeleteUser_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteUser.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteUser(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public void IdNotInDatabase_DeleteUser_ReturnsNotFound()
        {
            // ARRANGE
            var testId = ObjectId.GenerateNewId().ToString();

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(false)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteUser(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("User could not be found."));
        }

        [Test]
        public void DeviceWithUserExists_DeleteUser_DeletionStopped()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            var user = new User();
            user.Role = "admin";


            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.ToBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.Is<FilterDefinition<BsonDocument>>(filter => filter != null)))
                  .Returns(new List<BsonDocument> { user.ToBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.Is<FilterDefinition<BsonDocument>>(filter => filter != null)), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteUser(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ConflictObjectResult)));
            Assert.That(((ConflictObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
            Assert.That(((ConflictObjectResult)response).Value,
                        Is.EqualTo("The admin could not be deleted because this is the only remaining admin account remaining. There must be at least one admin account existing at all times."));
        }

        [Test]
        public void IdIsValid_DeleteUser_DeletionSuccessful()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            var user = new User();

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.ToBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.DeleteUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(true)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteUser(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkResult)));
            Assert.That(((OkResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
