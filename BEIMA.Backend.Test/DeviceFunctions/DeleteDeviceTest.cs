using BEIMA.Backend.AuthService;
using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
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

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class DeleteDeviceTest : UnitTestBase
    {
        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("jddkkslo9402mdjgkflsnxjf")]
        public async Task IdIsInvalid_DeleteDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await DeleteDevice.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));    

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task IdNotInDatabase_DeleteDevice_ReturnsNotFound()
        {
            // ARRANGE
            var claims = new Claims()
            {
                Username = "Test"
            };

            var testId = ObjectId.GenerateNewId().ToString();
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
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

            var request = CreateHttpRequest(RequestMethod.POST);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await DeleteDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task IdIsValid_NotAuthenticated_DeleteDevice_ReturnsUnauthorized()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";
            var device = new Device(ObjectId.Parse(testId), ObjectId.GenerateNewId(), "A-1", "Generic Inc.", "43", "x46b", 2005, "Comment");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetPhoto(Guid.NewGuid() + ".png", "photo");
            device.AddFile(Guid.NewGuid() + ".txt", "file");
            device.SetLocation(null, null, null, null);

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                .Returns(device.GetBsonDocument())
                .Verifiable();
            mockDb.Setup(mock => mock.DeleteDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                .Returns(true)
                .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.DeleteFile(It.IsAny<string>()))
                .Returns(Task.FromResult(true))
                .Verifiable();

            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await DeleteDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public async Task IdIsValid_IsAuthenticated_DeleteDevice_DeletionSuccessful()
        {
            // ARRANGE
            var testId = "1234567890abcdef12345678";
            var device = new Device(ObjectId.Parse(testId), ObjectId.GenerateNewId(), "A-1", "Generic Inc.", "43", "x46b", 2005, "Comment");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetPhoto(Guid.NewGuid() + ".png", "photo");
            device.AddFile(Guid.NewGuid() + ".txt", "file");
            device.SetLocation(null,null,null,null);

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                .Returns(device.GetBsonDocument())
                .Verifiable();
            mockDb.Setup(mock => mock.DeleteDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                .Returns(true)
                .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.DeleteFile(It.IsAny<string>()))
                .Returns(Task.FromResult(true))
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
            var response = await DeleteDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDevice(It.IsAny<ObjectId>()), Times.Once));

            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Exactly(2)));

            Assert.That(response, Is.TypeOf(typeof(OkResult)));
            Assert.That(((OkResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
