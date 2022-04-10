using BEIMA.Backend.AuthService;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using BEIMA.Backend.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class GetDeviceTest : UnitTestBase
    {

        #region FailureTests

        [Test]
        public async Task IdNotInDatabase_GetDevice_ReturnsNotFound()
        {
            // ARRANGE
            var claims = new Claims()
            {
                Username = "Test"
            };

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "1234567890abcdef12345678";
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            // Set up the http request.
            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await GetDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.GetPresignedURL(It.IsAny<string>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Device could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("10alskdjfhgytur74839skcm")]
        public async Task InvalidId_GetDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await GetDevice.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.GetPresignedURL(It.IsAny<string>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task IdIsValid_NotAuthenticated_GetDevice_ReturnsUnauthorized()
        {
            // ARRANGE

            var testId = "1234567890abcdef12345678";

            var dbDevice = new Device(new ObjectId(testId), ObjectId.GenerateNewId(), "a", "b", "c", "1234", 2020, "d");
            dbDevice.SetLocation(ObjectId.GenerateNewId(), "notes", "0", "1");
            dbDevice.SetLastModified(DateTime.UtcNow, "Anonymous");
            dbDevice.SetPhoto(Guid.NewGuid() + ".png", "photo");
            dbDevice.AddFile(Guid.NewGuid() + ".txt", "file");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(dbDevice.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await GetDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }

        #endregion FailureTests

        #region SuccessTests

        [Test]
        public async Task IdIsValid_IsAuthenticated_GetDevice_ReturnsDevice()
        {
            // ARRANGE

            var testId = "1234567890abcdef12345678";

            var dbDevice = new Device(new ObjectId(testId), ObjectId.GenerateNewId(), "a", "b", "c", "1234", 2020, "d");
            dbDevice.SetLocation(ObjectId.GenerateNewId(), "notes", "0", "1");
            dbDevice.SetLastModified(DateTime.UtcNow, "Anonymous");
            dbDevice.SetPhoto(Guid.NewGuid() + ".png", "photo");
            dbDevice.AddFile(Guid.NewGuid() + ".txt", "file");

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(dbDevice.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.GetPresignedURL(It.IsAny<string>()))
                .Returns(Task.FromResult("url"))
                .Verifiable();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await GetDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.GetPresignedURL(It.IsAny<string>()), Times.Exactly(2)));

            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var device = (Device)((OkObjectResult)response).Value;
            Assert.That(device.SerialNum, Is.EqualTo("1234"));
        }

        #endregion SuccessTests
    }
}
