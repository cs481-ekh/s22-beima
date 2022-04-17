using BEIMA.Backend.DeviceTypeFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceTypeFunctions
{
    [TestFixture]
    public class GetDeviceTypeListTest : UnitTestBase
    {
        [Test]
        public void PopulatedDatabase_GetDeviceTypeList_ReturnsListOfDeviceTypes()
        {
            // ARRANGE
            var deviceType1 = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "Boiler description", "Some notes.");
            var deviceType2 = new DeviceType(ObjectId.GenerateNewId(), "Electric Meters", "Meter description", "Some notes.");
            var deviceType3 = new DeviceType(ObjectId.GenerateNewId(), "Cooling Towers", "Generator description", "Some notes.");

            deviceType1.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType2.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceType3.SetLastModified(DateTime.UtcNow, "Anonymous");

            deviceType1.AddField(Guid.NewGuid().ToString(), "Type");
            deviceType1.AddField(Guid.NewGuid().ToString(), "Fuel Input Rate");
            deviceType1.AddField(Guid.NewGuid().ToString(), "Output");

            deviceType2.AddField(Guid.NewGuid().ToString(), "Account Number");
            deviceType2.AddField(Guid.NewGuid().ToString(), "Service ID");
            deviceType2.AddField(Guid.NewGuid().ToString(), "Voltage");

            deviceType3.AddField(Guid.NewGuid().ToString(), "Type");
            deviceType3.AddField(Guid.NewGuid().ToString(), "Chiller(s) Served");
            deviceType3.AddField(Guid.NewGuid().ToString(), "Capacity");

            var deviceTypeList = new List<BsonDocument>
            {
                deviceType1.GetBsonDocument(),
                deviceType2.GetBsonDocument(),
                deviceType3.GetBsonDocument(),
            };
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns(deviceTypeList)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (OkObjectResult)GetDeviceTypeList.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDeviceTypes(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Exactly(deviceTypeList.Count)));

            var getList = (List<object>)response.Value;
            Assert.IsNotNull(getList);
            for (int i = 0; i < deviceTypeList.Count; i++)
            {
                var deviceType = (Dictionary<string, object>)getList[i];
                var expectedDevice = deviceTypeList[i];
                Assert.That(deviceType["_id"].ToString(), Is.EqualTo(expectedDevice["_id"].AsObjectId.ToString()));
                Assert.That(deviceType["name"].ToString(), Is.EqualTo(expectedDevice["name"].AsString));
                Assert.That(deviceType["description"].ToString(), Is.EqualTo(expectedDevice["description"].AsString));
                Assert.That(deviceType["notes"].ToString(), Is.EqualTo(expectedDevice["notes"].AsString));
                Assert.That(deviceType["count"], Is.EqualTo(0));

                var fields = (Dictionary<string, object>)deviceType["fields"];
                var expectedFields = expectedDevice["fields"].AsBsonDocument;
                foreach (var field in expectedFields)
                {
                    Assert.That(fields, Contains.Key(field.Name));
                    Assert.That(fields, Contains.Value(field.Value.AsString));
                }

                var lastMod = (Dictionary<string, object>)deviceType["lastModified"];
                var expectedLastMod = expectedDevice["lastModified"];
                Assert.That(lastMod["date"].ToString(), Is.EqualTo(expectedLastMod["date"].ToUniversalTime().ToString()));
                Assert.That(lastMod["user"].ToString(), Is.EqualTo(expectedLastMod["user"].AsString));
            }
        }
    }
}
