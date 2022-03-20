using BEIMA.Backend.DeviceTypeFunctions;
using BEIMA.Backend.MongoService;
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

namespace BEIMA.Backend.Test.DeviceTypeFunctions
{
    [TestFixture]
    public class UpdateDeviceTypeTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateDeviceType_ReturnsNotFound()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDeviceType(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateDeviceType);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("Device type could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateDeviceType_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDeviceType(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateDeviceType);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDeviceType.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingDeviceType_UpdatesDeviceType_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var origDeviceType = new DeviceType(new ObjectId(testId), "Boiler", "Device type for boilers", "Some notes");
            origDeviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            origDeviceType.AddField("533ba994-352d-497e-a827-ab78314405a8", "MaxTemperature");
            origDeviceType.AddField("7331073d-95bb-4940-9643-cdbbcdc3fdc1", "MinTemperature");
            origDeviceType.AddField("8f3f9a48-53d0-492a-98f7-c770a7736aec", "Capacity");

            var deviceType = new DeviceType(new ObjectId(testId), "Boiler", "Device type for boilers", "Some more notes.");
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("533ba994-352d-497e-a827-ab78314405a8", "MaxTemperature");
            deviceType.AddField("7331073d-95bb-4940-9643-cdbbcdc3fdc1", "MinTemperature");
            deviceType.AddField("8f3f9a48-53d0-492a-98f7-c770a7736aec", "BoilerCapacity");
            deviceType.AddField("f76e47dc-4206-42b4-ad3f-a4c5b9e2205f", "AddedField");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.ToString().Equals(testId))))
                  .Returns(origDeviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testUpdateDeviceType;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Never));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var deviceTypeBson = (Dictionary<string, object>)((OkObjectResult)response).Value;
            Assert.That(deviceTypeBson["notes"], Is.EqualTo(deviceType.Notes));
            Assert.That(((Dictionary<string, object>)deviceTypeBson["fields"])["f76e47dc-4206-42b4-ad3f-a4c5b9e2205f"],
                Is.EqualTo(deviceType.Fields["f76e47dc-4206-42b4-ad3f-a4c5b9e2205f"].AsString));
        }

        [Test]
        public async Task ExistingDeviceTypeWithAssociatedDevice_UpdatesDeviceType_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var origDeviceType = new DeviceType(new ObjectId(testId), "Boiler", "Device type for boilers", "Some notes");
            origDeviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            origDeviceType.AddField("533ba994-352d-497e-a827-ab78314405a8", "MaxTemperature");
            origDeviceType.AddField("7331073d-95bb-4940-9643-cdbbcdc3fdc1", "MinTemperature");
            origDeviceType.AddField("8f3f9a48-53d0-492a-98f7-c770a7736aec", "Capacity");

            var deviceType = new DeviceType(new ObjectId(testId), "Boiler", "Device type for boilers", "Some more notes.");
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType.AddField("533ba994-352d-497e-a827-ab78314405a8", "MaxTemperature");
            deviceType.AddField("7331073d-95bb-4940-9643-cdbbcdc3fdc1", "MinTemperature");
            deviceType.AddField("8f3f9a48-53d0-492a-98f7-c770a7736aec", "BoilerCapacity");
            deviceType.AddField("f76e47dc-4206-42b4-ad3f-a4c5b9e2205f", "AddedField");

            var origDevice = new Device(new ObjectId("000000000000000000000000"), new ObjectId(testId), "A-1", "Generic Manufacturer", "1234", "4321", 2001, "Some notes.");
            origDevice.SetLastModified(DateTime.UtcNow, "Anonymous");
            origDevice.SetLocation(null, null, null, null);
            origDevice.SetPhoto(string.Empty, string.Empty);
            origDevice.SetFields(new Dictionary<string, string>
            {
                { "533ba994-352d-497e-a827-ab78314405a8", "100" },
                { "7331073d-95bb-4940-9643-cdbbcdc3fdc1", "0" },
                { "8f3f9a48-53d0-492a-98f7-c770a7736aec", "200" },
            });

            var device = new Device(new ObjectId("000000000000000000000000"), new ObjectId(testId), "A-1", "Generic Manufacturer", "1234", "4321", 2001, "Some notes.");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");
            device.SetLocation(null, null, null, null);
            device.SetPhoto(string.Empty, string.Empty);
            device.SetFields(new Dictionary<string, string?>
            {
                { "533ba994-352d-497e-a827-ab78314405a8", "100" },
                { "7331073d-95bb-4940-9643-cdbbcdc3fdc1", "0" },
                { "8f3f9a48-53d0-492a-98f7-c770a7736aec", "200" },
                { "f76e47dc-4206-42b4-ad3f-a4c5b9e2205f", null }
            });

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid.ToString().Equals(testId))))
                  .Returns(origDeviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument> { origDevice.GetBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()))
                  .Returns(deviceType.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()))
                  .Returns(device.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testUpdateDeviceType;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateDeviceType.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDeviceType(It.IsAny<BsonDocument>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateDevice(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var deviceTypeBson = (Dictionary<string, object>)((OkObjectResult)response).Value;
            Assert.That(deviceTypeBson["notes"], Is.EqualTo(deviceType.Notes));
            Assert.That(((Dictionary<string, object>)deviceTypeBson["fields"])["f76e47dc-4206-42b4-ad3f-a4c5b9e2205f"],
                Is.EqualTo(deviceType.Fields["f76e47dc-4206-42b4-ad3f-a4c5b9e2205f"].AsString));
        }
    }
}
