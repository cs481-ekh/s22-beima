using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceFT : FunctionalTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            // Delete all the devices in the database
            var deviceList = await TestClient.GetDeviceList();
            foreach (var device in deviceList)
            {
                if (device?.Id is not null)
                {
                    await TestClient.DeleteDevice(device.Id);
                }
            }
        }

        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_DeviceGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetDevice(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoDevicesInDatabase_DeviceGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetDevice(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeviceNotInDatabase_AddDevice_CreatesNewDevice()
        {
            // ARRANGE
            var device = new Device
            {
                DeviceTag = "A-2",
                DeviceTypeId = "abcd1234abcd1234abcd1234",
                Location = new Location
                {
                    BuildingId = "111111111111111111111111",
                    Notes = "Some building notes.",
                    Longitude = "123.001",
                    Latitude = "101.321",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "26",
                Notes = "Some device notes.",
                SerialNum = "xb76",
                YearManufactured = 2020,
            };

            // ACT
            var responseId = await TestClient.AddDevice(device);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            var getDevice = await TestClient.GetDevice(responseId);
            Assert.That(getDevice, Is.Not.Null);
            Assert.That(getDevice.DeviceTag, Is.EqualTo(device.DeviceTag));
            Assert.That(getDevice.DeviceTypeId, Is.EqualTo(device.DeviceTypeId));
            Assert.That(getDevice.Manufacturer, Is.EqualTo(device.Manufacturer));
            Assert.That(getDevice.ModelNum, Is.EqualTo(device.ModelNum));
            Assert.That(getDevice.Notes, Is.EqualTo(device.Notes));
            Assert.That(getDevice.SerialNum, Is.EqualTo(device.SerialNum));
            Assert.That(getDevice.YearManufactured, Is.EqualTo(device.YearManufactured));

            Assert.That(getDevice.LastModified?.Date, Is.Not.Null);
            Assert.That(getDevice.LastModified?.User, Is.EqualTo("Anonymous"));

            Assert.That(getDevice.Location?.BuildingId, Is.EqualTo(device.Location.BuildingId));
            Assert.That(getDevice.Location?.Notes, Is.EqualTo(device.Location.Notes));
            Assert.That(getDevice.Location?.Longitude, Is.EqualTo(device.Location.Longitude));
            Assert.That(getDevice.Location?.Latitude, Is.EqualTo(device.Location.Latitude));

            // Cleanup
            if(getDevice.Id != null)
            {
                await TestClient.DeleteDevice(getDevice.Id);
            }
        }

        [Test]
        public async Task DevicesInDatabase_GetDeviceList_ReturnsDeviceList()
        {
            // ARRANGE
            var deviceList = new List<Device>
            {
                new Device
                {
                    DeviceTag = "A-2",
                    DeviceTypeId = "192830495728394823103456",
                    Location = new Location
                    {
                        BuildingId = "192830495728394829103986",
                        Latitude = "101.001",
                        Longitude = "6.234",
                        Notes = "Outside",
                    },
                    Manufacturer = "Generic Inc.",
                    ModelNum = "44tsec",
                    Notes = "Giberish",
                    SerialNum = "tt4s",
                    YearManufactured = 2010,
                },
                new Device
                {
                    DeviceTag = "B-1",
                    DeviceTypeId = "902830495728394829103456",
                    Location = new Location
                    {
                        BuildingId = "192831695728394829103456",
                        Latitude = "74.003",
                        Longitude = "138.123",
                        Notes = "Inside",
                    },
                    Manufacturer = "Generic Labs",
                    ModelNum = "dbbr6f",
                    Notes = "Blah Blah",
                    SerialNum = "c4ta",
                    YearManufactured = 2011,
                },
                new Device
                {
                    DeviceTag = "C-3",
                    DeviceTypeId = "192831195728394829103456",
                    Location = new Location
                    {
                        BuildingId = "192830495728214829103456",
                        Latitude = "101.989",
                        Longitude = "25.004",
                        Notes = "Above",
                    },
                    Manufacturer = "Generic Company",
                    ModelNum = "y5eyf",
                    Notes = "qwerty",
                    SerialNum = "t4vw",
                    YearManufactured = 2012,
                },
            };

            foreach (var device in deviceList)
            {
                await TestClient.AddDevice(device);
            }

            // ACT
            var actualDevices = await TestClient.GetDeviceList();

            // ASSERT
            Assert.That(actualDevices.Count, Is.EqualTo(3));

            foreach (var device in actualDevices)
            {
                Assert.That(device, Is.Not.Null);
                var expectedDevice = deviceList.Single(d => d.DeviceTag?.Equals(device.DeviceTag) ?? false);

                Assert.That(device.DeviceTypeId, Is.EqualTo(expectedDevice.DeviceTypeId));
                Assert.That(device.Manufacturer, Is.EqualTo(expectedDevice.Manufacturer));
                Assert.That(device.ModelNum, Is.EqualTo(expectedDevice.ModelNum));
                Assert.That(device.Notes, Is.EqualTo(expectedDevice.Notes));
                Assert.That(device.SerialNum, Is.EqualTo(expectedDevice.SerialNum));
                Assert.That(device.YearManufactured, Is.EqualTo(expectedDevice.YearManufactured));

                Assert.That(device.Location?.BuildingId, Is.EqualTo(expectedDevice.Location?.BuildingId));
                Assert.That(device.Location?.Notes, Is.EqualTo(expectedDevice.Location?.Notes));
                Assert.That(device.Location?.Longitude, Is.EqualTo(expectedDevice.Location?.Longitude));
                Assert.That(device.Location?.Latitude, Is.EqualTo(expectedDevice.Location?.Latitude));

                Assert.That(device.LastModified, Is.Not.Null);
                Assert.That(device.LastModified?.Date, Is.Not.Null);
                Assert.That(device.LastModified?.User, Is.EqualTo("Anonymous"));
            }

            // Cleanup
            foreach (var device in actualDevices)
            {
                if (device.Id != null)
                {
                    await TestClient.DeleteDevice(device.Id);
                }
            }
        }
        
        [Test]
        public async Task DeviceInDatabase_DeleteDevice_DeviceDeletedSuccessfully()
        {
            // ARRANGE
            var device = new Device
            {
                DeviceTag = "D-4",
                DeviceTypeId = "473830495728394823103456",
                Location = new Location
                {
                    BuildingId = "a19830495728394829103986",
                    Latitude = "111.001",
                    Longitude = "8.242",
                    Notes = "Outside",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "44tsec",
                Notes = "Giberish",
                SerialNum = "dvsd",
                YearManufactured = 2000,
            };

            var deviceId = await TestClient.AddDevice(device);
            Assume.That(await TestClient.GetDevice(deviceId), Is.Not.Null);

            // ACT
            Assert.DoesNotThrowAsync(async () => await TestClient.DeleteDevice(deviceId));

            // ASSERT
            var ex = Assert.ThrowsAsync<BeimaException>(async () => await TestClient.GetDevice(deviceId));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        
        [Test]
        public async Task DeviceInDatabase_UpdateDevice_ReturnsUpdatedDevice()
        {
            // ARRANGE
            var origDevice = new Device
            {
                DeviceTag = "E-6",
                DeviceTypeId = "abc830495728312323103456",
                Location = new Location
                {
                    BuildingId = "cab830495728394829103986",
                    Latitude = "11.001",
                    Longitude = "61.234",
                    Notes = "Near",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "avv3ar",
                Notes = "Giberish",
                SerialNum = "3dvs",
                YearManufactured = 2012,
            };

            var deviceId = await TestClient.AddDevice(origDevice);
            var updateItem = await TestClient.GetDevice(deviceId);

            updateItem.Notes = "Updated Notes.";

            // ACT
            var updatedDevice = await TestClient.UpdateDevice(updateItem);

            // ASSERT
            Assert.That(updatedDevice, Is.Not.Null);
            Assert.That(updatedDevice.Notes, Is.Not.EqualTo(origDevice.Notes));

            Assert.That(updatedDevice.LastModified?.Date, Is.Not.EqualTo(updateItem.LastModified?.Date));
            Assert.That(updatedDevice.LastModified?.User, Is.EqualTo(updateItem.LastModified?.User));

            Assert.That(updatedDevice.DeviceTag, Is.EqualTo(updateItem.DeviceTag));
            Assert.That(updatedDevice.DeviceTypeId, Is.EqualTo(updateItem.DeviceTypeId));
            Assert.That(updatedDevice.Manufacturer, Is.EqualTo(updateItem.Manufacturer));
            Assert.That(updatedDevice.ModelNum, Is.EqualTo(updateItem.ModelNum));
            Assert.That(updatedDevice.SerialNum, Is.EqualTo(updateItem.SerialNum));
            Assert.That(updatedDevice.Notes, Is.EqualTo(updateItem.Notes));
            Assert.That(updatedDevice.YearManufactured, Is.EqualTo(updateItem.YearManufactured));

            Assert.That(updatedDevice.Location?.BuildingId, Is.EqualTo(updateItem.Location?.BuildingId));
            Assert.That(updatedDevice.Location?.Notes, Is.EqualTo(updateItem.Location?.Notes));
            Assert.That(updatedDevice.Location?.Latitude, Is.EqualTo(updateItem.Location?.Latitude));
            Assert.That(updatedDevice.Location?.Longitude, Is.EqualTo(updateItem.Location?.Longitude));

            // Cleanup
            await TestClient.DeleteDevice(deviceId);
            
        }
    }
}
