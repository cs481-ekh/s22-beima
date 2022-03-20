using BEIMA.Backend.StorageService;
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
        private string? _deviceTypeId;
        private string? _deviceTypeFieldUuid;

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
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
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
            Assert.That(getDevice.LastModified?.User, Is.EqualTo("Anonymous"));

            Assert.That(getDevice.Location?.BuildingId, Is.EqualTo(device.Location.BuildingId));
            Assert.That(getDevice.Location?.Notes, Is.EqualTo(device.Location.Notes));
            Assert.That(getDevice.Location?.Longitude, Is.EqualTo(device.Location.Longitude));
            Assert.That(getDevice.Location?.Latitude, Is.EqualTo(device.Location.Latitude));

            Assert.That(getDevice.Fields?.Single().Key, Is.EqualTo(device.Fields.Single().Key));
            Assert.That(getDevice.Fields?.Single().Value, Is.EqualTo(device.Fields.Single().Value));
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
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
                    DeviceTypeId = _deviceTypeId,
                    Fields = new Dictionary<string, string>
                    {
                        { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                    },
                    Location = new DeviceLocation
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

                Assert.That(device.Fields?.Single().Key, Is.EqualTo(expectedDevice.Fields?.Single().Key));
                Assert.That(device.Fields?.Single().Value, Is.EqualTo(expectedDevice.Fields?.Single().Value));
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
                Location = new DeviceLocation
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
            var _storage = StorageDefinition.StorageInstance;
            var photoExists = _storage.GetFileExists(photoUid);
            var fileExists = _storage.GetFileExists(fileUid);

            Assert.That(photoExists, Is.False);
            Assert.That(fileExists, Is.False);

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
                DeviceTypeId = _deviceTypeId,
                Fields = new Dictionary<string, string>
                {
                    { _deviceTypeFieldUuid ?? "UuidRetrievalFailed", "GenericValue" },
                },
                Location = new DeviceLocation
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

            Assert.That(updatedDevice.Fields?.Single().Key, Is.EqualTo(updateItem.Fields?.Single().Key));
            Assert.That(updatedDevice.Fields?.Single().Value, Is.EqualTo(updateItem.Fields?.Single().Value));

            Assert.That(updatedDevice.Photo?.FileName, Is.EqualTo(photoName));
            Assert.That(updatedDevice.Photo?.FileUid, Is.Not.Null);
            Assert.That(updatedDevice.Photo?.FileUrl, Is.Not.Null);

            Assert.That(updatedDevice.Files?.Count, Is.EqualTo(0));
        }
    }
}
