using BEIMA.Backend.AuthService;
using BEIMA.Backend.DeviceTypeFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceTypeFunctions
{
    public class GetDeviceTypeTest : UnitTestBase
    {
        #region FailureTests

        [Test]
        public void IdNotInDatabase_GetDeviceType_ReturnsNotFound()
        {
            // ARRANGE

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "1234567890abcdef12345678";
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
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
            var response = GetDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Device type could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("10alskdjfhgytur74839skcm")]
        public void InvalidId_GetDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = GetDeviceType.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public void InvalidCredentials_GetDeviceType_ReturnsUnauthenticated()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock auth service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Set up the http request.
            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)GetDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Value, Is.EqualTo("Invalid credentials."));
        }

        #endregion FailureTests

        #region SuccessTests

        [Test]
        public void IdIsValid_GetDevice_ReturnsDevice()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";

            var dbDeviceType = new DeviceType(new ObjectId(testId), "Boiler", "Describes a boiler.", "Some notes.");
            dbDeviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            dbDeviceType.AddField(Guid.NewGuid().ToString(), "MaxTemperature");
            dbDeviceType.AddField(Guid.NewGuid().ToString(), "MinTemperature");
            dbDeviceType.AddField(Guid.NewGuid().ToString(), "Capacity");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(dbDeviceType.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = GetDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var device = (Dictionary<string, object>)((OkObjectResult)response).Value;
            Assert.That(device["name"], Is.EqualTo("Boiler"));
            Assert.That(device["description"], Is.EqualTo("Describes a boiler."));
            Assert.That(device["notes"], Is.EqualTo("Some notes."));
        }

        #endregion SuccessTests
    }
}
