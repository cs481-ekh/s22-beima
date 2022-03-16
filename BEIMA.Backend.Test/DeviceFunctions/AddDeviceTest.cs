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
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
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
            using (var fileStream = new ByteArrayContent(Encoding.ASCII.GetBytes("TestOne")).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));
                
                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                deviceId = ((ObjectResult) await AddDevice.Run(request, logger)).Value?.ToString();
            }

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }

        [Test]
        public async Task NoDevice_AddDeviceWithNoLocation_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
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
            using (var fileStream = new ByteArrayContent(Encoding.ASCII.GetBytes("TestOne")).ReadAsStream())
            {
                fileCollection.Add(new FormFile(fileStream, 0, fileStream.Length, "files", "file.txt"));

                var request = CreateMultiPartHttpRequest(data, fileCollection);
                var logger = (new LoggerFactory()).CreateLogger("Testing");


                // ACT
                deviceId = ((ObjectResult)await AddDevice.Run(request, logger)).Value?.ToString();
            }

            // ASSERT
            Assert.IsNotNull(deviceId);
            Assert.That(ObjectId.TryParse(deviceId, out _), Is.True);
        }
    }
}
