using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test, Explicit("Need to implement delete endpoint to clear database before testing.")]
        public async Task DevicesInDatabase_GetDeviceTypeList_ReturnsDeviceTypeList()
        {
            // ARRANGE
            var deviceTypeList = new List<DeviceTypeAdd>
            {
                new DeviceTypeAdd
                {
                    Description = "Boiler description.",
                    Name = "Boiler",
                    Notes = "Some boiler notes.",
                    Fields = new List<string>
                    {
                        "Type",
                        "Fuel Input Rate",
                        "Output",
                    },
                },
                new DeviceTypeAdd
                {
                    Description = "Meters description",
                    Name = "Electric Meters",
                    Notes = "Some meter notes.",
                    Fields = new List<string>
                    {
                        "Account Number",
                        "Service ID",
                        "Voltage",
                    },
                },
                new DeviceTypeAdd
                {
                    Description = "Cooling tower description",
                    Name = "Cooling Tower",
                    Notes = "Some cooling tower notes.",
                    Fields = new List<string>
                    {
                        "Type",
                        "Chiller(s) Served",
                        "Capacity",
                    },
                },
            };

            foreach (var deviceType in deviceTypeList)
            {
                await TestClient.AddDeviceType(deviceType);
            }

            // ACT
            var actualDeviceTypes = await TestClient.GetDeviceTypeList();

            // ASSERT
            Assert.That(actualDeviceTypes.Count, Is.EqualTo(3));

            foreach (var deviceType in actualDeviceTypes)
            {
                Assert.That(deviceType, Is.Not.Null);
                var expectedDeviceType = deviceTypeList.Single(dt => dt.Name?.Equals(deviceType.Name) ?? false);

                Assert.That(deviceType.Description, Is.EqualTo(expectedDeviceType.Description));
                Assert.That(deviceType.Name, Is.EqualTo(expectedDeviceType.Name));
                Assert.That(deviceType.Notes, Is.EqualTo(expectedDeviceType.Notes));

                Assert.That(deviceType.Fields, Is.Not.Null.And.Not.Empty);
                if (deviceType.Fields != null)
                {
                    foreach (var field in deviceType.Fields)
                    {
                        Assert.That(Guid.TryParse(field.Key, out _), Is.True);
                    }
                }

                Assume.That(expectedDeviceType.Fields, Is.Not.Null.And.Not.Empty);
                if (expectedDeviceType.Fields != null)
                {
                    foreach (var field in expectedDeviceType.Fields)
                    {
                        Assert.That(deviceType.Fields, Contains.Value(field));
                    }
                }

                Assert.That(deviceType.LastModified, Is.Not.Null);
                Assert.That(deviceType.LastModified?.Date, Is.Not.Null);
                Assert.That(deviceType.LastModified?.User, Is.EqualTo("Anonymous"));
            }
        }
    }
}
