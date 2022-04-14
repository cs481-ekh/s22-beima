using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class ReportFT : FunctionalTestBase
    {
        public static string? _deviceTypeId;
        public static string? _deviceTypeFieldUuid;
        public static string? _buildingId;
        public static string? _deviceId;

        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync()
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

            // Add back in a test device type
            _deviceTypeId = await TestClient.AddDeviceType(
                new DeviceTypeAdd
                {
                    Description = "Generic device type.",
                    Name = "Generic",
                    Notes = "Device type for generic devices.",
                    Fields = new List<string>
                    {
                        "GenericField",
                    },
                });
            _deviceTypeFieldUuid = (await TestClient.GetDeviceType(_deviceTypeId)).Fields?.Keys.Single();

            // Add back in a test building
            _buildingId = await TestClient.AddBuilding(
                new Building
                {
                    Name = "Student Union",
                    Number = "1234",
                    Notes = "Some building notes.",
                    Location = new Location
                    {
                        Latitude = "0.123",
                        Longitude = "4.567",
                    },
                });

            // Add back in a test device
            _deviceId = await TestClient.AddDevice(
                new Device
                {
                    DeviceTag = "A-1",
                    DeviceTypeId = _deviceTypeId,
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "1.123",
                        Longitude = "4.567",
                        Notes = "Location notes.",
                    },
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "Device type invalid", "GenericValue"}
                    },
                    Manufacturer = "Generic Inc.",
                    ModelNum = "1234",
                    Notes = "Some notes.",
                    SerialNum = "asdf",
                    YearManufactured = 2020,
                });
        }

        [Test]
        public async Task PopulatedDatabase_AllDevicesReport_ReturnsValidZipFileByteArray()
        {
            // ARRANGE
            // ACT
            var response = await TestClient.AllDevicesReport();

            // ASSERT
            Assert.That(response, Is.Not.Null);
            using (MemoryStream mem = new MemoryStream(response))
            using (var zipArchive = new ZipArchive(mem, ZipArchiveMode.Read))
            {
                var entry = zipArchive.Entries.Single(e => e.Name == "Generic.csv");
                using (var entryStream = entry.Open())
                using (var streamReader = new StreamReader(entryStream))
                {
                    // Read in and assert first line of the csv
                    var line1 = streamReader.ReadLine();
                    Assert.That(line1, Is.EqualTo("Id,DeviceTypeId,DeviceTag,Manufacturer,ModelNum,SerialNum,YearManufactured,Notes,GenericField,BuildingId,Notes,Latitude,Longitude,Date,User"));
                }
            }
        }
    }
}
