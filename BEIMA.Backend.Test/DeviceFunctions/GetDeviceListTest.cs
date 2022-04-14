using BEIMA.Backend.AuthService;
using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class GetDeviceListTest : UnitTestBase
    {
        [Test]
        public void PopulatedDatabase_NotAuthenticated_GetDeviceList_ReturnsUnauthorized()
        {
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");
            var response = (ObjectResult)GetDeviceList.Run(request, logger);

            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            var retVal = ((ObjectResult)response).Value;

            Assert.That(retVal, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public void PopulatedDatabase_IsAuthenticated_GetDeviceList_ReturnsListOfDevices()
        {
            // ARRANGE
            var device1 = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "A-1", "Generic Inc.", "43", "x46b", 2005, "Comment");
            var device2 = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "B-2", "Generic Co.", "425", "w43s", 2007, "Giberish");
            var device3 = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "C-1", "Device Inc.", "26", "jd5r", 2013, "Device stuff.");

            device1.SetLocation(ObjectId.GenerateNewId(), "some notes", "98.345", "106.211");
            device2.SetLocation(ObjectId.GenerateNewId(), "more notes", "97.345", "107.211");
            device3.SetLocation(ObjectId.GenerateNewId(), "final notes", "96.345", "108.211");

            device1.SetLastModified(DateTime.UtcNow, "Anonymous");
            device2.SetLastModified(DateTime.UtcNow, "Anonymous");
            device3.SetLastModified(DateTime.UtcNow, "Anonymous");

            var deviceList = new List<BsonDocument>
            {
                device1.GetBsonDocument(),
                device2.GetBsonDocument(),
                device3.GetBsonDocument(),
            };

            var claims = new Claims()
            {
                Username = "Test"
            };

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(deviceList)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claims)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (OkObjectResult)GetDeviceList.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));

            var getList = (List<Device>)response.Value;
            Assert.IsNotNull(getList);
            for (int i = 0; i < deviceList.Count; i++)
            {
                var device = getList[i];
                var expectedDevice = deviceList[i];
                Assert.That(device.Id.ToString(), Is.EqualTo(expectedDevice["_id"].AsObjectId.ToString()));
                Assert.That(device.DeviceTypeId.ToString(), Is.EqualTo(expectedDevice["deviceTypeId"].AsObjectId.ToString()));
                Assert.That(device.DeviceTag, Is.EqualTo(expectedDevice["deviceTag"].AsString));
                Assert.That(device.Notes, Is.EqualTo(expectedDevice["notes"].AsString));
                Assert.That(device.Manufacturer, Is.EqualTo(expectedDevice["manufacturer"].AsString));
                Assert.That(device.ModelNum, Is.EqualTo(expectedDevice["modelNum"].AsString));
                Assert.That(device.SerialNum, Is.EqualTo(expectedDevice["serialNum"].AsString));
                Assert.That(device.YearManufactured, Is.EqualTo(expectedDevice["yearManufactured"].AsInt32));

                var location = device.Location;
                var expectedLocation = expectedDevice["location"];
                Assert.That(location.BuildingId.ToString(), Is.EqualTo(expectedLocation["buildingId"].AsObjectId.ToString()));
                Assert.That(location.Notes, Is.EqualTo(expectedLocation["notes"].AsString));
                Assert.That(location.Longitude, Is.EqualTo(expectedLocation["longitude"].AsString));
                Assert.That(location.Latitude, Is.EqualTo(expectedLocation["latitude"].AsString));

                var lastMod = device.LastModified;
                var expectedLastMod = expectedDevice["lastModified"];
                Assert.That(lastMod.Date.ToString(), Is.EqualTo(expectedLastMod["date"].ToUniversalTime().ToString()));
                Assert.That(lastMod.User.ToString(), Is.EqualTo(expectedLastMod["user"].AsString));
            }
        }

        [TestCase("deviceType", "deviceTypeId")]
        [TestCase("building", "location.buildingId")]
        public void FilteredQueryString_GetDeviceList_ReturnsListOfDevices(string queryProp, string filterProp)
        {
            // ARRANGE
            var device1 = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "A-1", "Generic Inc.", "43", "x46b", 2005, "Comment");
            device1.SetLocation(ObjectId.GenerateNewId(), "some notes", "98.345", "106.211");
            device1.SetLastModified(DateTime.UtcNow, "Anonymous");

            var deviceList = new List<BsonDocument>
            {
                device1.GetBsonDocument()
            };

            var filterPropValue = filterProp.Equals("deviceTypeId") ? device1.DeviceTypeId : device1.Location.BuildingId;
            var expectedFilter = MongoFilterGenerator.GetEqualsFilter(filterProp, filterPropValue).ToString();

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.Is<FilterDefinition<BsonDocument>>(fd => string.Equals(fd.ToString(), expectedFilter))))
                  .Returns(deviceList)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET, query: new Dictionary<string, StringValues> { { queryProp, new StringValues(filterPropValue.ToString()) } });
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (OkObjectResult)GetDeviceList.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));

            var getList = (List<Device>)response.Value;
            Assert.IsNotNull(getList);
            Assert.That(getList.Count, Is.EqualTo(1));
            var device = getList.Single();
            var expectedDevice = deviceList.Single();
            Assert.That(device.Id.ToString(), Is.EqualTo(expectedDevice["_id"].AsObjectId.ToString()));
            Assert.That(device.DeviceTypeId.ToString(), Is.EqualTo(expectedDevice["deviceTypeId"].AsObjectId.ToString()));
            Assert.That(device.DeviceTag, Is.EqualTo(expectedDevice["deviceTag"].AsString));
            Assert.That(device.Notes, Is.EqualTo(expectedDevice["notes"].AsString));
            Assert.That(device.Manufacturer, Is.EqualTo(expectedDevice["manufacturer"].AsString));
            Assert.That(device.ModelNum, Is.EqualTo(expectedDevice["modelNum"].AsString));
            Assert.That(device.SerialNum, Is.EqualTo(expectedDevice["serialNum"].AsString));
            Assert.That(device.YearManufactured, Is.EqualTo(expectedDevice["yearManufactured"].AsInt32));

            var location = device.Location;
            var expectedLocation = expectedDevice["location"];
            Assert.That(location.BuildingId.ToString(), Is.EqualTo(expectedLocation["buildingId"].AsObjectId.ToString()));
            Assert.That(location.Notes, Is.EqualTo(expectedLocation["notes"].AsString));
            Assert.That(location.Longitude, Is.EqualTo(expectedLocation["longitude"].AsString));
            Assert.That(location.Latitude, Is.EqualTo(expectedLocation["latitude"].AsString));

            var lastMod = device.LastModified;
            var expectedLastMod = expectedDevice["lastModified"];
            Assert.That(lastMod.Date.ToString(), Is.EqualTo(expectedLastMod["date"].ToUniversalTime().ToString()));
            Assert.That(lastMod.User.ToString(), Is.EqualTo(expectedLastMod["user"].AsString));
        }
    }
}
