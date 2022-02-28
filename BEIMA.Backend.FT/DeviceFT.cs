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
        }

        [Test, Explicit("Must have a cleared database.")]
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
        }
    }
}
