using BEIMA.Backend.UserFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BEIMA.Backend.Test.UserFunctions
{
    [TestFixture]
    public class UpdateUserTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateUser_ReturnsNotFound()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateUser(It.Is<BsonDocument>(u => u["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateUserWithPassword);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("User could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateUser_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateUser(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateUserWithPassword);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingUser_UpdateUserWithPassword_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var user = new User(new ObjectId(testId), "user.name", "updatedPassword123!", "Aaron", "Doe", "user");
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateUser(It.IsAny<BsonDocument>()))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testUpdateUserWithPassword;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var userResponse = (User)((OkObjectResult)response).Value;
            Assert.That(userResponse.Username, Is.EqualTo(user.Username));
            Assert.That(userResponse.Password, Is.EqualTo(user.Password));
            Assert.That(userResponse.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(userResponse.LastName, Is.EqualTo(user.LastName));
            Assert.That(userResponse.Role, Is.EqualTo(user.Role));
        }
    }
}
