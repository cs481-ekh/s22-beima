using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class AddDeviceTest : UnitTestBase
    {
        [Test]
        public async Task NoDevice_AddDevice_ReturnsValidId()
        {
            // ARRANGE
            var building = new Building(new ObjectId("111111111111111111111111"), null, null, null);
            building.SetLastModified(DateTime.UtcNow, "Anonymous");
            building.SetLocation(null, null);
            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetBuilding(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("111111111111111111111111")))))
                  .Returns(building.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.PutFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString() + ".txt"))
                .Verifiable();

            StorageDefinition.StorageInstance = mockStorage.Object;

            // Create request
            var data = TestData._testDevice;
            var fileCollection = new FormFileCollection();

            string? deviceId;
            using (var fileStream = new ByteArrayContent(TestData._fileBytes).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));

                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();
            }

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetBuilding(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.PutFile(It.IsAny<IFormFile>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertDevice(It.IsAny<BsonDocument>()), Times.Once));
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }

        [Test]
        public async Task NoDevice_AddDevice_NullYearManufactured_ReturnsValidId()
        {
            // ARRANGE
            var building = new Building(new ObjectId("111111111111111111111111"), null, null, null);
            building.SetLastModified(DateTime.UtcNow, "Anonymous");
            building.SetLocation(null, null);
            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetBuilding(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("111111111111111111111111")))))
                  .Returns(building.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.PutFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString() + ".txt"))
                .Verifiable();

            StorageDefinition.StorageInstance = mockStorage.Object;

            // Create request
            var data = TestData._testAddDeviceNullYearManufactured;
            var fileCollection = new FormFileCollection();

            string? deviceId;
            using (var fileStream = new ByteArrayContent(TestData._fileBytes).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));

                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();
            }

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.PutFile(It.IsAny<IFormFile>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertDevice(It.IsAny<BsonDocument>()), Times.Once));
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }

        [Test]
        public async Task NoDevice_AddDeviceWithNoLocation_ReturnsValidId()
        {
            // ARRANGE
            var deviceType = new DeviceType(new ObjectId("12341234abcdabcd43214321"), null, null, null);
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestName1");
            deviceType.AddField("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestName2");

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("12341234abcdabcd43214321")))))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.InsertDevice(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            mockStorage.Setup(mock => mock.PutFile(It.IsAny<IFormFile>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString() + ".txt"))
                .Verifiable();

            StorageDefinition.StorageInstance = mockStorage.Object;

            // Create request
            var data = TestData._testAddDeviceNoLocation;
            var fileCollection = new FormFileCollection();

            string? deviceId;
            using (var fileStream = new ByteArrayContent(TestData._fileBytes).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));

                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();
            }

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.PutFile(It.IsAny<IFormFile>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertDevice(It.IsAny<BsonDocument>()), Times.Once));
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }

        [Test]
        public async Task NoBuilding_AddDevice_ReturnsBuildingNotFound()
        {
            // ARRANGE

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetBuilding(It.Is<ObjectId>(oid => oid.Equals(new ObjectId("111111111111111111111111")))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock storage client.
            Mock<IStorageProvider> mockStorage = new Mock<IStorageProvider>();
            StorageDefinition.StorageInstance = mockStorage.Object;

            // Create request
            var data = TestData._testDevice;
            var fileCollection = new FormFileCollection();

            ObjectResult response;
            using (var fileStream = new ByteArrayContent(TestData._fileBytes).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));

                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                response = ((ObjectResult)await AddDevice.Run(request, logger));
            }

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetBuilding(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockStorage.Verify(mock => mock.PutFile(It.IsAny<IFormFile>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.InsertDevice(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value.ToString(), Is.EqualTo("Building could not be found."));
        }
    }
}
