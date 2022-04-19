using BEIMA.Backend.UserFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;
using BEIMA.Backend.AuthService;
using Microsoft.AspNetCore.Http;
using BEIMA.Backend.Models;
using System.Collections.Generic;

namespace BEIMA.Backend.Test.UserFunctions
{
    [TestFixture]
    public class GetUserTest : UnitTestBase
    {
        #region FailureTests

        [Test]
        public void IdNotInDatabase_GetUser_ReturnsNotFound()
        {
            // ARRANGE

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "1234567890abcdef12345678";
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock auth service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Set up the http request.
            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = GetUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("User could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("10alskdjfhgytur74839skcm")]
        public void InvalidId_GetUser_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock auth service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = GetUser.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [TestCaseSource(nameof(ClaimsFactory))]
        public void InvalidCredentials_GetUser_ReturnsUser(Claims claim)
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock auth service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claim)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)GetUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Value, Is.EqualTo("Invalid credentials."));
        }

        #endregion FailureTests

        #region SuccessTests

        [Test]
        public void IdIsValid_GetUser_ReturnsUser()
        {
            // ARRANGE

            var testId = "1234567890abcdef12345678";

            var dbUser = new User(new ObjectId(testId), "user.name123", "ThisIsAPassword!123", "Alex", "Smith", "user");
            dbUser.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(dbUser.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock auth service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = GetUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var user = (User)((OkObjectResult)response).Value;
            Assert.That(user.Username, Is.EqualTo("user.name123"));
            // GetUser should return an empty string for password, as it should not be sending the password on the endpoint.
            Assert.That(user.Password, Is.EqualTo(""));
            Assert.That(user.FirstName, Is.EqualTo("Alex"));
            Assert.That(user.LastName, Is.EqualTo("Smith"));
            Assert.That(user.Role, Is.EqualTo("user"));
        }

        #endregion SuccessTests

        private static IEnumerable<Claims?> ClaimsFactory()
        {
            yield return null;
            yield return new Claims { Role = "nonadmin", Username = "Bob" };
        }
    }
}
