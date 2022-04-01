using MongoDB.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class BuildingFT : FunctionalTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            // Delete all the devices in the database.
            var deviceList = await TestClient.GetDeviceList();
            foreach (var device in deviceList)
            {
                if (device?.Id is not null)
                {
                    await TestClient.DeleteDevice(device.Id);
                }
            }
            // Delete all the device types in the database
            var deviceTypeList = await TestClient.GetDeviceTypeList();
            foreach (var deviceType in deviceTypeList)
            {
                if (deviceType?.Id is not null)
                {
                    await TestClient.DeleteDeviceType(deviceType.Id);
                }
            }
            // Delete all the buildings in the database
            var buildingList = await TestClient.GetBuildingList();
            foreach (var building in buildingList)
            {
                if (building?.Id is not null)
                {
                    await TestClient.DeleteBuilding(building.Id);
                }
            }
        }

        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_BuildingGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetBuilding(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoBuildingsInDatabase_BuildingGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetBuilding(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task BuildingNotInDatabase_AddBuilding_CreatesNewBuilding()
        {
            // ARRANGE
            var building = new Building
            {
                Name = "Student Union",
                Number = "1234",
                Notes = "Some notes.",
                Location = new Location
                {
                    Longitude = "12.001",
                    Latitude = "24.321",
                },
            };

            // ACT
            var responseId = await TestClient.AddBuilding(building);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            Assert.That(ObjectId.TryParse(responseId, out _), Is.True);

            var getBuilding = await TestClient.GetBuilding(responseId);
            Assert.That(getBuilding.Name, Is.EqualTo(building.Name));
            Assert.That(getBuilding.Number, Is.EqualTo(building.Number));
            Assert.That(getBuilding.Notes, Is.EqualTo(building.Notes));

            Assert.That(getBuilding.LastModified, Is.Not.Null);
            Assert.That(getBuilding.LastModified?.Date, Is.Not.Null);
            Assert.That(getBuilding.LastModified?.User, Is.EqualTo("Anonymous"));

            Assert.That(getBuilding.Location, Is.Not.Null);
            Assert.That(getBuilding.Location?.Latitude, Is.EqualTo(building.Location?.Latitude));
            Assert.That(getBuilding.Location?.Longitude, Is.EqualTo(building.Location?.Longitude));
        }

        [Test]
        public async Task BuildingsInDatabase_GetBuildingList_ReturnsBuildingList()
        {
            // ARRANGE
            var buildingList = new List<Building>
            {
                new Building
                {
                    Name = "Student Union",
                    Number = "1234",
                    Notes = "Some SUB notes.",
                    Location = new Location
                    {
                        Latitude = "0.123",
                        Longitude = "4.567",
                    }
                },
                new Building
                {
                    Name = "Interactive Learning Center",
                    Number = "4321",
                    Notes = "Some ILC notes.",
                    Location = new Location
                    {
                        Latitude = "-0.123",
                        Longitude = "-4.567",
                    }
                },
                new Building
                {
                    Name = "Albertons Library",
                    Number = "2468",
                    Notes = "Some Library notes.",
                    Location = new Location
                    {
                        Latitude = "0.123",
                        Longitude = "-4.567",
                    }
                },
            };

            foreach (var building in buildingList)
            {
                building.Id = await TestClient.AddBuilding(building);
            }

            // ACT
            var actualBuildings = await TestClient.GetBuildingList();

            // ASSERT
            Assert.That(actualBuildings.Count, Is.EqualTo(3));

            foreach (var building in actualBuildings)
            {
                Assert.That(building, Is.Not.Null);
                var expectedBuilding = buildingList.Single(b => b.Id?.Equals(building.Id) ?? false);

                Assert.That(building.Name, Is.EqualTo(expectedBuilding.Name));
                Assert.That(building.Number, Is.EqualTo(expectedBuilding.Number));
                Assert.That(building.Notes, Is.EqualTo(expectedBuilding.Notes));

                Assert.That(building.LastModified, Is.Not.Null);
                Assert.That(building.LastModified?.Date, Is.Not.Null);
                Assert.That(building.LastModified?.User, Is.EqualTo("Anonymous"));

                Assert.That(building.Location, Is.Not.Null);
                Assert.That(building.Location?.Latitude, Is.EqualTo(expectedBuilding.Location?.Latitude));
                Assert.That(building.Location?.Longitude, Is.EqualTo(expectedBuilding.Location?.Longitude));
            }
        }

        [Test]
        public async Task BuildingInDatabase_DeleteBuilding_BuildingDeletedSuccessfully()
        {
            // ARRANGE
            var building = new Building
            {
                Name = "Student Union",
                Number = "1234",
                Notes = "Some notes.",
                Location = new Location
                {
                    Latitude = "1.234",
                    Longitude = "2.345",
                },
            };

            var buildingId = await TestClient.AddBuilding(building);
            Assume.That(await TestClient.GetBuilding(buildingId), Is.Not.Null);

            // ACT
            Assert.DoesNotThrowAsync(async () => await TestClient.DeleteBuilding(buildingId));

            // ASSERT
            var ex = Assert.ThrowsAsync<BeimaException>(async () => await TestClient.GetBuilding(buildingId));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task BuildingInDatabase_UpdateBuilding_ReturnsUpdatedBuilding()
        {
            // ARRANGE
            var origBuilding = new Building
            {
                Name = "Student Union",
                Number = "1234",
                Notes = "Some building notes.",
                Location = new Location
                {
                    Latitude = "12.345",
                    Longitude = "10.101",
                },
            };

            var buildingId = await TestClient.AddBuilding(origBuilding);
            var origItem = await TestClient.GetBuilding(buildingId);
            Assume.That(origItem, Is.Not.Null);

            var updateItem = new Building
            {
                Id = origItem.Id,
                Name = origItem.Name + " Building",
                Number = origItem.Number,
                Notes = "Some new building notes.",
                Location = new Location
                {
                    Latitude = "-12.345",
                    Longitude = "-10.101",
                }
            };

            // ACT
            var updatedBuilding = await TestClient.UpdateBuilding(updateItem);

            // ASSERT
            Assert.That(updatedBuilding, Is.Not.Null);
            Assert.That(updatedBuilding.Notes, Is.Not.EqualTo(origBuilding.Notes));

            Assert.That(updatedBuilding.LastModified?.Date, Is.Not.EqualTo(origItem.LastModified?.Date));
            Assert.That(updatedBuilding.LastModified?.User, Is.EqualTo(origItem.LastModified?.User));

            Assert.That(updatedBuilding.Id, Is.EqualTo(updateItem.Id));
            Assert.That(updatedBuilding.Name, Is.EqualTo(updateItem.Name));
            Assert.That(updatedBuilding.Number, Is.EqualTo(updateItem.Number));
            Assert.That(updatedBuilding.Notes, Is.EqualTo(updateItem.Notes));

            Assert.That(updatedBuilding.Location?.Latitude, Is.EqualTo(updateItem.Location?.Latitude));
            Assert.That(updatedBuilding.Location?.Longitude, Is.EqualTo(updateItem.Location?.Longitude));
        }
    }
}
