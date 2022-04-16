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
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.BuildingFunctions
{
    [TestFixture]
    public class AddBuildingTest : UnitTestBase
    {
        [Test]
        public async Task NoBuilding_AddBuilding_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertBuilding(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Create request
            var body = TestData._testBuilding;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var buildingId = ((ObjectResult)await AddBuilding.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertBuilding(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(buildingId);
            Assert.That(ObjectId.TryParse(buildingId, out _), Is.True);
        }

        [Test]
        public async Task NoBuilding_Unauthorized_AddBuilding_ReturnsUnauthorized()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertBuilding(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Create request
            var body = TestData._testBuilding;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = ((ObjectResult) await AddBuilding.Run(request, logger));

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertBuilding(It.IsAny<BsonDocument>()), Times.Never));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }
    }
}
