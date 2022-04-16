using Microsoft.AspNetCore.Http.Internal;
using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceFT : FunctionalTestBase
    {
        private static string? _deviceTypeId;
        private static string? _deviceTypeId2;

        private static string? _deviceTypeFieldUuid;
        private static string? _deviceTypeFieldUuid2;

        private static string? _buildingId;
        private static string? _buildingId2;

        #region SetUp and TearDown

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
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

            // Add back in a couple test device types to test with
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
            _deviceTypeId2 = await TestClient.AddDeviceType(
                new DeviceTypeAdd
                {
                    Description = "Boiler device type.",
                    Name = "Boiler",
                    Notes = "Device type for boiler devices.",
                    Fields = new List<string>
                    {
                        "BoilerField",
                    },
                });
            _deviceTypeFieldUuid2 = (await TestClient.GetDeviceType(_deviceTypeId2)).Fields?.Keys.Single();

            // Add back in a couple test buildings to test with
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
            _buildingId2 = await TestClient.AddBuilding(
                new Building
                {
                    Name = "Albertsons Library",
                    Number = "4321",
                    Notes = "Some building notes.",
                    Location = new Location
                    {
                        Latitude = "1.001",
                        Longitude = "2.002",
                    },
                });
        }

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

        #endregion SetUp and TearDown

        #region Device GET Tests

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

        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_NotAuthorized_DeviceGet_ReturnsUnauthorized(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await UnauthorizedTestClient.GetDevice(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
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

        #endregion Device GET Tests

        #region Device Add Tests

        [Test]
        public void NoDevicesInDatabase_NotAuthorized_DeviceGet_ValidId_ReturnsUnauthorized()
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await UnauthorizedTestClient.GetDevice("1234567890abcdef12345678")
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void NoDevicesInDatabase_NotAuthorized_DeviceGet_EmptyId_ReturnsNotFound()
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await UnauthorizedTestClient.GetDevice("")
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
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
                {
                    BuildingId = _buildingId,
                    Notes = "Some building notes.",
                    Longitude = "12.301",
                    Latitude = "10.321",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "26",
                Notes = "Some device notes.",
                SerialNum = "xb76",
                YearManufactured = 2020,
            };
            FormFileCollection files = new FormFileCollection();
            var fileName = "file.txt";
            var photoName = "photo.txt";

            string responseId;
            using (var fileStream = new MemoryStream(TestObjects._fileBytes))
            using (var photoStream = new MemoryStream(TestObjects._fileBytes))
            {
                files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                // ACT
                responseId = await TestClient.AddDevice(device, files);
            }

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
            Assert.That(getDevice.Files?.Count, Is.EqualTo(1));
            Assert.That(getDevice.Files?[0].FileUid, Is.Not.Null);
            Assert.That(getDevice.Files?[0].FileName, Is.EqualTo(fileName));

            Assert.That(getDevice.Photo?.FileName, Is.EqualTo(photoName));
            Assert.That(getDevice.Photo?.FileUid, Is.Not.Null);

            Assert.That(getDevice.LastModified?.Date, Is.Not.Null);
            Assert.That(getDevice.LastModified?.User, Is.EqualTo(TestUsername));

            Assert.That(getDevice.Location?.BuildingId, Is.EqualTo(device.Location.BuildingId));
            Assert.That(getDevice.Location?.Notes, Is.EqualTo(device.Location.Notes));
            Assert.That(getDevice.Location?.Longitude, Is.EqualTo(device.Location.Longitude));
            Assert.That(getDevice.Location?.Latitude, Is.EqualTo(device.Location.Latitude));

            Assert.That(getDevice.Fields?.Single().Key, Is.EqualTo(device.Fields.Single().Key));
            Assert.That(getDevice.Fields?.Single().Value, Is.EqualTo(device.Fields.Single().Value));
        }

        #endregion Device Add Tests

        #region Device List Tests

        [Test]
        public void DeviceNotInDatabase_NotAuthorized_AddDevice_ReturnsUnauthorized()
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
            {
                var device = new Device
                {
                    DeviceTag = "A-2",
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Notes = "Some building notes.",
                        Longitude = "12.301",
                        Latitude = "10.321",
                    },
                    Manufacturer = "Generic Inc.",
                    ModelNum = "26",
                    Notes = "Some device notes.",
                    SerialNum = "xb76",
                    YearManufactured = 2020,
                };
                FormFileCollection files = new FormFileCollection();
                var fileName = "file.txt";
                var photoName = "photo.txt";

                using (var fileStream = new MemoryStream(TestObjects._fileBytes))
                using (var photoStream = new MemoryStream(TestObjects._fileBytes))
                {
                    files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                    files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                    // ACT
                    await UnauthorizedTestClient.AddDevice(device, files);
                }
            });
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "10.101",
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "11.989",
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
                Assert.That(device.LastModified?.User, Is.EqualTo(TestUsername));

                Assert.That(device.Fields?.Single().Key, Is.EqualTo(expectedDevice.Fields?.Single().Key));
                Assert.That(device.Fields?.Single().Value, Is.EqualTo(expectedDevice.Fields?.Single().Value));
            }
        }

        [TestCase("deviceType=DT_1", new int[] { 0, 2 })]
        [TestCase("building=B_1", new int[] { 0, 1 })]
        [TestCase("deviceType=DT_1&building=B_1", new int[] { 0 })]
        [TestCase("deviceType=DT_1&deviceType=DT_2&building=B_1", new int[] { 0, 1 })]
        [TestCase("deviceType=DT_1&building=B_1&building=B_2", new int[] { 0, 2 })]
        [TestCase("deviceType=DT_1&deviceType=DT_2&building=B_1&building=B_2", new int[] { 0, 1, 2, 3 })]
        [TestCase("unusedQueryKey=000000000000000000000000", new int[] { 0, 1, 2, 3 })]
        [TestCase("deviceType=badId", new int[] { 0, 1, 2, 3 })]
        [TestCase("deviceType=000000000000000000000000", new int[] { })]
        public async Task DevicesInDatabase_GetDeviceList_WithFilters_ReturnsDeviceList(string queryString, params int[] expectedDeviceIndicies)
        {
            // ARRANGE
            var deviceList = new List<Device>
            {
                new Device
                {
                    DeviceTag = "A-2",
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "10.101",
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
                    DeviceTypeId = _deviceTypeId2,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid2 ?? "UuidRetrievalFailed", "BoilerValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId2,
                        Latitude = "11.989",
                        Longitude = "25.004",
                        Notes = "Above",
                    },
                    Manufacturer = "Generic Company",
                    ModelNum = "y5eyf",
                    Notes = "qwerty",
                    SerialNum = "t4vw",
                    YearManufactured = 2012,
                },
                new Device
                {
                    DeviceTag = "D-4",
                    DeviceTypeId = _deviceTypeId2,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid2 ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId2,
                        Latitude = "45.285",
                        Longitude = "39.102",
                        Notes = "Above",
                    },
                    Manufacturer = "Generic Company",
                    ModelNum = "ev4ve",
                    Notes = "fdsat",
                    SerialNum = "vtwv",
                    YearManufactured = 2013,
                },
            };

            foreach (var device in deviceList)
            {
                await TestClient.AddDevice(device);
            }

            // Fill in query string ids with actual device type and building ids
            queryString = queryString.Replace("DT_1", _deviceTypeId)
                                     .Replace("DT_2", _deviceTypeId2)
                                     .Replace("B_1", _buildingId)
                                     .Replace("B_2", _buildingId2);

            // ACT
            var actualDevices = await TestClient.GetDeviceList(queryString);

            // ASSERT
            Assert.That(actualDevices.Count, Is.EqualTo(expectedDeviceIndicies.Count()));

            // Create the expected list of devices
            var expectedDevices = new List<Device>();
            foreach (var index in expectedDeviceIndicies)
            {
                expectedDevices.Add(deviceList[index]);
            }

            // Assert on the devices
            foreach (var device in actualDevices)
            {
                Assert.That(device, Is.Not.Null);
                var expectedDevice = expectedDevices.Single(d => d.DeviceTag?.Equals(device.DeviceTag) ?? false);

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
                Assert.That(device.LastModified?.User, Is.EqualTo(TestUsername));

                Assert.That(device.Fields?.Single().Key, Is.EqualTo(expectedDevice.Fields?.Single().Key));
                Assert.That(device.Fields?.Single().Value, Is.EqualTo(expectedDevice.Fields?.Single().Value));
            }
        }

        #endregion Device List Tests

        #region Device Delete Tests

        [Test]
        public async Task DevicesInDatabase_NotAuthorized_GetDeviceList_ReturnsUnauthorized()
        {
            // ARRANGE
            var deviceList = new List<Device>
            {
                new Device
                {
                    DeviceTag = "A-2",
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "10.101",
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
                    {
                        BuildingId = _buildingId,
                        Latitude = "11.989",
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
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await UnauthorizedTestClient.GetDeviceList()
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeviceInDatabase_DeleteDevice_DeviceDeletedSuccessfully()
        {
            // ARRANGE
            var device = new Device
            {
                DeviceTag = "D-4",
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
                {
                    BuildingId = _buildingId,
                    Latitude = "11.001",
                    Longitude = "8.242",
                    Notes = "Outside",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "44tsec",
                Notes = "Giberish",
                SerialNum = "dvsd",
                YearManufactured = 2000,
            };

            FormFileCollection files = new FormFileCollection();
            var fileName = "file.txt";
            var photoName = "photo.txt";

            string deviceId;
            using (var fileStream = new MemoryStream(TestObjects._fileBytes))
            using (var photoStream = new MemoryStream(TestObjects._fileBytes))
            {
                files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                // ACT
                deviceId = await TestClient.AddDevice(device, files);
            }

            var response = await TestClient.GetDevice(deviceId);
            Assert.That(response, Is.Not.Null);

            var photoUid = response.Photo?.FileUid;
            var fileUid = response.Files?[0].FileUid;

            Assert.That(photoUid, Is.Not.Null);
            Assert.That(fileUid, Is.Not.Null);

            // ACT
            Assert.DoesNotThrowAsync(async () => await TestClient.DeleteDevice(deviceId));

            // ASSERT
            var ex = Assert.ThrowsAsync<BeimaException>(async () => await TestClient.GetDevice(deviceId));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        #endregion Device Delete Tests

        #region Device Update Tests

        [Test]
        public async Task DeviceInDatabase_NotAuthorized_DeleteDevice_ReturnsUnauthorized()
        {
            // ARRANGE
            var device = new Device
            {
                DeviceTag = "D-4",
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
                {
                    BuildingId = _buildingId,
                    Latitude = "11.001",
                    Longitude = "8.242",
                    Notes = "Outside",
                },
                Manufacturer = "Generic Inc.",
                ModelNum = "44tsec",
                Notes = "Giberish",
                SerialNum = "dvsd",
                YearManufactured = 2000,
            };

            FormFileCollection files = new FormFileCollection();
            var fileName = "file.txt";
            var photoName = "photo.txt";

            string deviceId;
            using (var fileStream = new MemoryStream(TestObjects._fileBytes))
            using (var photoStream = new MemoryStream(TestObjects._fileBytes))
            {
                files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                // ACT
                deviceId = await TestClient.AddDevice(device, files);
            }

            var response = await TestClient.GetDevice(deviceId);
            Assert.That(response, Is.Not.Null);

            var photoUid = response.Photo?.FileUid;
            var fileUid = response.Files?[0].FileUid;

            Assert.That(photoUid, Is.Not.Null);
            Assert.That(fileUid, Is.Not.Null);

            // ACT
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await UnauthorizedTestClient.DeleteDevice(deviceId)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeviceInDatabase_UpdateDevice_ReturnsUpdatedDevice()
        {
            // ARRANGE
            var origDevice = new Device
            {
                DeviceTag = "E-6",
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
                {
                    BuildingId = _buildingId,
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
            FormFileCollection files = new FormFileCollection();
            var fileName = "file.txt";
            var photoName = "photo.txt";

            Device updatedDevice;
            DeviceUpdate updateItem;
            using (var fileStream = new MemoryStream(TestObjects._fileBytes))
            using (var photoStream = new MemoryStream(TestObjects._fileBytes))
            {
                files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                var deviceId = await TestClient.AddDevice(origDevice, files);
                var deviceItem = await TestClient.GetDevice(deviceId);
                updateItem = new DeviceUpdate()
                {
                    Id = deviceItem.Id,
                    Manufacturer = deviceItem.Manufacturer,
                    ModelNum = deviceItem.ModelNum,
                    DeviceTag = deviceItem.DeviceTag,
                    DeviceTypeId = deviceItem.DeviceTypeId,
                    Fields = deviceItem.Fields,
                    LastModified = deviceItem.LastModified,
                    Location = deviceItem.Location,
                    SerialNum = deviceItem.SerialNum,
                    Photo = deviceItem.Photo,
                    YearManufactured = deviceItem.YearManufactured,
                    Notes = "Updated Notes.",
                    Files = deviceItem.Files
                };

                updateItem.DeletedFiles = new List<string>();

                var fileUid = updateItem.Files?[0].FileUid;
                if (fileUid != null)
                {
                    updateItem.DeletedFiles.Add(fileUid);
                }


                // Act
                updatedDevice = await TestClient.UpdateDevice(updateItem);
            }

            // ASSERT
            Assert.That(updatedDevice, Is.Not.Null);
            Assert.That(updatedDevice.Notes, Is.Not.EqualTo(origDevice.Notes));

            Assert.That(updatedDevice.LastModified?.Date, Is.Not.EqualTo(updateItem.LastModified?.Date));
            Assert.That(updatedDevice.LastModified?.User, Is.EqualTo(TestUsername));

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

            Assert.That(updatedDevice.Fields?.Single().Key, Is.EqualTo(updateItem.Fields?.Single().Key));
            Assert.That(updatedDevice.Fields?.Single().Value, Is.EqualTo(updateItem.Fields?.Single().Value));

            Assert.That(updatedDevice.Photo?.FileName, Is.EqualTo(photoName));
            Assert.That(updatedDevice.Photo?.FileUid, Is.Not.Null);
            Assert.That(updatedDevice.Photo?.FileUrl, Is.Not.Null);

            Assert.That(updatedDevice.Files?.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeviceInDatabase_NotAuthorized_UpdateDevice_ReturnsUnauthorized()
        {
            // ARRANGE
            var origDevice = new Device
            {
                DeviceTag = "E-6",
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
                {
                    BuildingId = _buildingId,
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
            FormFileCollection files = new FormFileCollection();
            var fileName = "file.txt";
            var photoName = "photo.txt";

            DeviceUpdate updateItem;
            using (var fileStream = new MemoryStream(TestObjects._fileBytes))
            using (var photoStream = new MemoryStream(TestObjects._fileBytes))
            {
                files.Add(new FormFile(fileStream, 0, fileStream.Length, "files", fileName));
                files.Add(new FormFile(photoStream, 0, photoStream.Length, "photo", photoName));

                var deviceId = await TestClient.AddDevice(origDevice, files);
                var deviceItem = await TestClient.GetDevice(deviceId);
                updateItem = new DeviceUpdate()
                {
                    Id = deviceItem.Id,
                    Manufacturer = deviceItem.Manufacturer,
                    ModelNum = deviceItem.ModelNum,
                    DeviceTag = deviceItem.DeviceTag,
                    DeviceTypeId = deviceItem.DeviceTypeId,
                    Fields = deviceItem.Fields,
                    LastModified = deviceItem.LastModified,
                    Location = deviceItem.Location,
                    SerialNum = deviceItem.SerialNum,
                    Photo = deviceItem.Photo,
                    YearManufactured = deviceItem.YearManufactured,
                    Notes = "Updated Notes.",
                    Files = deviceItem.Files
                };

                updateItem.DeletedFiles = new List<string>();

                var fileUid = updateItem.Files?[0].FileUid;
                if (fileUid != null)
                {
                    updateItem.DeletedFiles.Add(fileUid);
                }


                // Act
                var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                    await UnauthorizedTestClient.UpdateDevice(updateItem)
                );

                // Assert
                Assert.IsNotNull(ex);
                Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
                Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }

        #endregion Device Update Tests
    }
}
