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
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class UpdateDeviceTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateDevice_ReturnsNotFound()
        {
            // ARRANGE
            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            var claims = new Claims()
            {
                Username = "Test"
            };

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(id => id.ToString().Equals(testId))));
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDevice(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.DeleteFile(It.IsAny<string>()))
                .Returns<bool>(null)
                .Verifiable();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Set up the http request.
            var data = TestData._testUpdateDeviceDeleteFiles;
            var request = CreateMultiPartHttpRequest(data);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Device could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(id => id.ToString().Equals(id))))
                 .Returns<BsonDocument>(null)
                 .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.DeleteFile(It.IsAny<string>()))
                .Returns<bool>(null)
                .Verifiable();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var data = TestData._testUpdateDeviceDeleteFiles;
            var request = CreateMultiPartHttpRequest(data);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDevice.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingDevice_UpdatesDevice_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var device = new Device(new ObjectId(testId), new ObjectId("12341234abcdabcd43214321"), "A-3", "Generic Inc.", "1234", "abcd1234", 2004, "Some notes.");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetLocation(new ObjectId("622cf00109137c26f913b281"), "Some notes.", "12.345", "10.101");

            var building = new Building(new ObjectId("622cf00109137c26f913b281"), null, null, null);
            building.SetLastModified(DateTime.UtcNow, "Anonymous");
            building.SetLocation(null, null);

            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.IsAny<ObjectId>()))
                  .Returns(device.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetBuilding(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("622cf00109137c26f913b281")))))
                  .Returns(device.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()))
                  .Returns( () => { 
                        device.LastModified.User = claims.Username;
                        return device.GetBsonDocument();
                  })
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.GetPresignedURL(It.IsAny<string>()))
                .Returns(Task.FromResult("url"))
                .Verifiable();
            mockStorage.Setup(mock => mock.DeleteFile(It.IsAny<string>()))
               .Returns(Task.FromResult(true))
               .Verifiable();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            // Create request
            var data = TestData._testUpdateDeviceDeleteFiles;
            var request = CreateMultiPartHttpRequest(data);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetBuilding(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Exactly(2)));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.GetPresignedURL(It.IsAny<string>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Once));
            var resDevice = (Device)((OkObjectResult)response).Value;

            Assert.That(resDevice.ModelNum, Is.EqualTo(device.ModelNum));
            Assert.That(resDevice.Notes, Is.EqualTo(device.Notes));
            Assert.That(resDevice.LastModified.User, Is.EqualTo(claims.Username));
        }

        [Test]
        public async Task ExistingDevice_NotAuthorized_UpdatesDevice_ReturnsUnauthorized()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var device = new Device(new ObjectId(testId), new ObjectId("12341234abcdabcd43214321"), "A-3", "Generic Inc.", "1234", "abcd1234", 2004, "Some notes.");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetLocation(new ObjectId("111111111111111111111111"), "Some notes.", "12.345", "10.101");

            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.IsAny<ObjectId>()))
                  .Returns(device.GetBsonDocument());
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()))
                  .Returns(device.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.GetPresignedURL(It.IsAny<string>()))
                .Returns(Task.FromResult("url"))
                .Verifiable();
            StorageDefinition.StorageInstance = mockStorage.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var data = TestData._testUpdateDeviceNoLocation;
            var request = CreateMultiPartHttpRequest(data);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public async Task ExistingDevice_IsAuthorized_UpdatesDeviceWithNoLocation_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var device = new Device(new ObjectId(testId), new ObjectId("12341234abcdabcd43214321"), "A-3", "Generic Inc.", "1234", "abcd1234", 2004, "Some notes.");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetLocation(new ObjectId("111111111111111111111111"), "Some notes.", "12.345", "10.101");

            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.IsAny<ObjectId>()))
                  .Returns(device.GetBsonDocument());
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()))
                  .Returns(() => {
                      device.LastModified.User = claims.Username;
                      return device.GetBsonDocument();
                  })
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
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var data = TestData._testUpdateDeviceNoLocation;
            var request = CreateMultiPartHttpRequest(data);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDevice.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.DeleteFile(It.IsAny<string>()), Times.Never));
            var resDevice = (Device)((OkObjectResult)response).Value;

            Assert.That(resDevice.ModelNum, Is.EqualTo(device.ModelNum));
            Assert.That(resDevice.Notes, Is.EqualTo(device.Notes));
            Assert.That(resDevice.LastModified.User, Is.EqualTo(claims.Username));
        }
    }
}
