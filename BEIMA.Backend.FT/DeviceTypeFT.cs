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
        public async Task SetUp()
        {
            // Delete all the device types in the database
            var deviceTypeList = await TestClient.GetDeviceTypeList();
            foreach (var deviceType in deviceTypeList)
            {
                if (deviceType?.Id is not null)
                {
                    await TestClient.DeleteDeviceType(deviceType.Id);
                }
            }
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

        [Test]
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

        [Test]
        public async Task DeviceTypeInDatabase_DeleteDeviceType_DeviceTypeDeletedSuccessfully()
        {
            // ARRANGE
            var deviceType = new DeviceTypeAdd
            {
                Description = "Boiler description.",
                Name = "Boiler",
                Notes = "Some other boiler notes.",
                Fields = new List<string>
                    {
                        "Type",
                        "Fuel Input Rate",
                        "Output",
                    },
            };

            var deviceTypeId = await TestClient.AddDeviceType(deviceType);
            Assume.That(await TestClient.GetDeviceType(deviceTypeId), Is.Not.Null);

            // ACT
            Assert.DoesNotThrowAsync(async () => await TestClient.DeleteDeviceType(deviceTypeId));

            // ASSERT
            var ex = Assert.ThrowsAsync<BeimaException>(async () => await TestClient.GetDeviceType(deviceTypeId));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeviceTypeInDatabase_UpdateDeviceType_ReturnsUpdatedDeviceType()
        {
            // ARRANGE
            var origDeviceType = new DeviceTypeAdd
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
            };

            var deviceTypeId = await TestClient.AddDeviceType(origDeviceType);
            var origItem = await TestClient.GetDeviceType(deviceTypeId);

            var updateDictionary = new Dictionary<string, string>();
            if (origItem.Fields != null)
            {
                foreach (var item in origItem.Fields)
                {
                    updateDictionary.Add(item.Key, item.Value);
                    if (item.Value.Equals("Type"))
                    {
                        updateDictionary[item.Key] = "Boiler Type";
                    }
                }
            }

            var updateItem = new DeviceTypeUpdate
            {
                Id = origItem.Id,
                Description = origItem.Description,
                Name = origItem.Name,
                Notes = "Some other boiler notes.",
                Fields = updateDictionary,
                NewFields = new List<string>
                {
                    "Capacity",
                },
            };

            // ACT
            var updatedDeviceType = await TestClient.UpdateDeviceType(updateItem);

            // ASSERT
            Assert.That(updatedDeviceType, Is.Not.Null);
            Assert.That(updatedDeviceType.Notes, Is.Not.EqualTo(origDeviceType.Notes));

            Assert.That(updatedDeviceType.LastModified?.Date, Is.Not.EqualTo(origItem.LastModified?.Date));
            Assert.That(updatedDeviceType.LastModified?.User, Is.EqualTo(origItem.LastModified?.User));

            Assert.That(updatedDeviceType.Id, Is.EqualTo(updateItem.Id));
            Assert.That(updatedDeviceType.Description, Is.EqualTo(updateItem.Description));
            Assert.That(updatedDeviceType.Name, Is.EqualTo(updateItem.Name));
            Assert.That(updatedDeviceType.Notes, Is.EqualTo(updateItem.Notes));

            Assume.That(origItem, Is.Not.Null);
            foreach (var item in updateItem.Fields)
            {
                Assert.That(updatedDeviceType.Fields, Contains.Key(item.Key));
                Assert.That(updatedDeviceType.Fields?[item.Key], Is.EqualTo(item.Value));
            }
            Assert.That(updatedDeviceType.Fields?.Where(kv => kv.Value.Equals("Type")), Is.Empty);
            Assert.That(Guid.TryParse(updatedDeviceType.Fields?.Single(kv => kv.Value.Equals("Capacity")).Key, out _), Is.True);
        }
    }
}
