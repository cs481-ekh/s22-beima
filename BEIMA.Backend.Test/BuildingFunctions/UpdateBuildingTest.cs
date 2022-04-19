using BEIMA.Backend.AuthService;
using BEIMA.Backend.BuildingFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.BuildingFunctions
{
    [TestFixture]
    public class UpdateBuildingTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateBuilding_ReturnsNotFound()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.UpdateBuilding(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateBuilding);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Building could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateBuilding_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateBuilding(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateBuilding);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateBuilding.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingBuilding_UpdateBuilding_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var building = new Building(new ObjectId(testId), "Student Union Building", "1234", "Some new building notes.");
            building.SetLocation("-12.345", "-10.101");
            building.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()))
                  .Returns(building.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUpdateBuilding;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var buildingResponse = (Building)((OkObjectResult)response).Value;
            Assert.That(buildingResponse.Name, Is.EqualTo(building.Name));
            Assert.That(buildingResponse.Number, Is.EqualTo(building.Number));
            Assert.That(buildingResponse.Notes, Is.EqualTo(building.Notes));
            Assert.That(buildingResponse.Location.Latitude, Is.EqualTo(building.Location.Latitude));
            Assert.That(buildingResponse.Location.Longitude, Is.EqualTo(building.Location.Longitude));
        }

        [Test]
        public async Task ExistingBuilding_Unauthorized_UpdateBuilding_ReturnsUnauthorized()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var building = new Building(new ObjectId(testId), "Student Union Building", "1234", "Some new building notes.");
            building.SetLocation("-12.345", "-10.101");
            building.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()))
                  .Returns(building.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUpdateBuilding;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateBuilding.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateBuilding(It.IsAny<BsonDocument>()), Times.Never));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }
    }
}
