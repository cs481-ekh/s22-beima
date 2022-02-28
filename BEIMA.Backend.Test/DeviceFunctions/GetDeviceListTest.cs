using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class GetDeviceListTest : UnitTestBase
    {
        [Test]
        public void PopulatedDatabase_GetDeviceList_ReturnsListOfDevices()
        {
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
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(deviceList)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");
            var response = (OkObjectResult)GetDeviceList.Run(request, logger);
            var getList = (List<object>)response.Value;
            Assert.IsNotNull(getList);
            for (int i = 0; i < deviceList.Count; i++)
            {
                var device = (Dictionary<string, object>)getList[i];
                var expectedDevice = deviceList[i];
                Assert.That(device["_id"].ToString(), Is.EqualTo(expectedDevice["_id"].AsObjectId.ToString()));
                Assert.That(device["deviceTypeId"].ToString(), Is.EqualTo(expectedDevice["deviceTypeId"].AsObjectId.ToString()));
                Assert.That(device["deviceTag"], Is.EqualTo(expectedDevice["deviceTag"].AsString));
                Assert.That(device["notes"], Is.EqualTo(expectedDevice["notes"].AsString));
                Assert.That(device["manufacturer"], Is.EqualTo(expectedDevice["manufacturer"].AsString));
                Assert.That(device["modelNum"], Is.EqualTo(expectedDevice["modelNum"].AsString));
                Assert.That(device["serialNum"], Is.EqualTo(expectedDevice["serialNum"].AsString));
                Assert.That(device["yearManufactured"], Is.EqualTo(expectedDevice["yearManufactured"].AsInt32));

                var location = (Dictionary<string, object>)device["location"];
                var expectedLocation = expectedDevice["location"];
                Assert.That(location["buildingId"].ToString(), Is.EqualTo(expectedLocation["buildingId"].AsObjectId.ToString()));
                Assert.That(location["notes"], Is.EqualTo(expectedLocation["notes"].AsString));
                Assert.That(location["longitude"], Is.EqualTo(expectedLocation["longitude"].AsString));
                Assert.That(location["latitude"], Is.EqualTo(expectedLocation["latitude"].AsString));

                var lastMod = (Dictionary<string, object>)device["lastModified"];
                var expectedLastMod = expectedDevice["lastModified"];
                Assert.That(lastMod["date"].ToString(), Is.EqualTo(expectedLastMod["date"].ToUniversalTime().ToString()));
                Assert.That(lastMod["user"].ToString(), Is.EqualTo(expectedLastMod["user"].AsString));
            }
        }
    }
}
