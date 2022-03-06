using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceTypeFT : FunctionalTestBase
    {
        [SetUp]
        public void SetUp()
        {
            // TODO: Remove all device types from database during setup
        }

        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_DeviceTypeGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetDeviceType(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoDeviceTypesInDatabase_DeviceTypeGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetDeviceType(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task NoDeviceTypesInDb_AddDeviceType_DeviceTypeAddedSuccessfullyAsync()
        {
            // ARRANGE
            var device = new DeviceTypeAdd
            {
                Description = "This is a boiler type.",
                Name = "Boiler",
                Notes = "Some type notes.",
                Fields = new List<string>()
                {
                    "MaxTemperature",
                    "MinTemperature",
                    "Capacity"
                }
            };

            // ACT
            var responseId = await TestClient.AddDeviceType(device);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            Assert.That(ObjectId.TryParse(responseId, out _), Is.True);
            var getDeviceType = await TestClient.GetDeviceType(responseId);
            Assert.That(getDeviceType.Name, Is.EqualTo(device.Name));
            Assert.That(getDeviceType.Notes, Is.EqualTo(device.Notes));
            Assert.That(getDeviceType.Description, Is.EqualTo(device.Description));
            Assert.That(getDeviceType.Fields, Is.Not.Null.And.Not.Empty);
            Assert.That(getDeviceType.Fields?.Count, Is.EqualTo(device.Fields.Count));
            if (getDeviceType.Fields != null)
            {
                foreach (var key in getDeviceType.Fields.Keys)
                {
                    Assert.That(Guid.TryParse(key, out _), Is.True);
                }
            }
            Assert.That(getDeviceType.Fields, Contains.Value(device.Fields[0]));
            Assert.That(getDeviceType.Fields, Contains.Value(device.Fields[1]));
            Assert.That(getDeviceType.Fields, Contains.Value(device.Fields[2]));
        }
    }
}
