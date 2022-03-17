using System;
using System.Linq;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class DeviceTest
    {
        //Instance variables
        readonly ObjectId validObjId = ObjectId.GenerateNewId();
        readonly ObjectId validDeviceTypeId = ObjectId.GenerateNewId();
        readonly string validDeviceTag = "DeviceTag123";
        readonly string validManufacturer = "BSU";
        readonly string validModelNum = "ABC-123";
        readonly string validSerialNum = "ABCDEFG123456";
        readonly int validYearManufactured = 2022;
        readonly string validNotes = "Example of notes.";

        readonly DateTime validDate = DateTime.Now.ToUniversalTime();
        readonly string validUser = "User1";

        readonly ObjectId validBuildingId = ObjectId.GenerateNewId();
        readonly string validLocationNotes = "Example of location notes.";
        readonly string validLatitude = "-000.21.21";
        readonly string validLongitude = "-323.32.32";

        readonly string validKey1 = "key1";
        readonly string validKey2 = "key2";
        readonly string validKey3 = "key3";
        readonly string validValue1 = "value1";
        readonly string validValue2 = "true";
        readonly string validValue3 = "123";

        readonly string validFileName1 = "manual.pdf";
        readonly string validFileName2 = "information.txt";
        readonly string validFileName3 = "document.docx";
        readonly string validFileUid1 = Guid.NewGuid().ToString();
        readonly string validFileUid2 = Guid.NewGuid().ToString();
        readonly string validFileUid3 = Guid.NewGuid().ToString();

        readonly string validPhotoName1 = "manual.pdf";
        readonly string validPhotoUid1 = Guid.NewGuid().ToString();

        [Test]
        public void DeviceNotInstantiated_InstantiateUsingFullConstructor_DeviceInstantiatedWithCorrectValues()
        {
            var device = new Device(validObjId, validDeviceTypeId, validDeviceTag, validManufacturer, validModelNum, validSerialNum, validYearManufactured, validNotes);
            device.SetLastModified(validDate, validUser);
            device.SetLocation(validBuildingId, validLocationNotes, validLatitude, validLongitude);
            device.AddField(validKey1, validValue1);
            device.AddField(validKey2, validValue2);
            device.AddField(validKey3, validValue3);
            device.AddFile(validFileUid1, validFileName1);
            device.AddFile(validFileUid2, validFileName2);
            device.AddFile(validFileUid3, validFileName3);
            device.SetPhoto(validPhotoUid1, validPhotoName1);


            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(validDeviceTag));
            Assert.That(device.Manufacturer, Is.EqualTo(validManufacturer));
            Assert.That(device.ModelNum, Is.EqualTo(validModelNum));
            Assert.That(device.SerialNum, Is.EqualTo(validSerialNum));
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.EqualTo(validNotes));

            var lastModified = device.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));

            var location = device.Location;
            Assert.That(location.BuildingId, Is.EqualTo(validBuildingId));
            Assert.That(location.Notes, Is.EqualTo(validLocationNotes));
            Assert.That(location.Latitude, Is.EqualTo(validLatitude));
            Assert.That(location.Longitude, Is.EqualTo(validLongitude));

            var fields = device.Fields;
            Assert.That(fields[validKey1], Is.EqualTo(validValue1));
            Assert.That(fields[validKey2], Is.EqualTo(validValue2));
            Assert.That(fields[validKey3], Is.EqualTo(validValue3));

            var files = device.Files;
            Assert.That(files.Single(file => file.FileUid == validFileUid1 && file.FileName == validFileName1), Is.Not.Null);
            Assert.That(files.Single(file => file.FileUid == validFileUid2 && file.FileName == validFileName2), Is.Not.Null);
            Assert.That(files.Single(file => file.FileUid == validFileUid3 && file.FileName == validFileName3), Is.Not.Null);

            var photo = device.Photo;
            Assert.That(photo.FileUid == validPhotoUid1 && photo.FileName == validPhotoName1, Is.True);
        }

        [Test]
        public void DeviceNotInstantiated_InstantiateUsingObjectInitializer_DeviceInstantiatedWithCorrectValues()
        {
            var device = new Device
            {
                Id = validObjId,
                DeviceTypeId = validDeviceTypeId,
                DeviceTag = validDeviceTag,
                Manufacturer = validManufacturer,
                ModelNum = validModelNum,
                SerialNum = validSerialNum,
                YearManufactured = validYearManufactured,
                Notes = validNotes
            };
            device.SetLastModified(validDate, validUser);
            device.SetLocation(validBuildingId, validLocationNotes, validLatitude, validLongitude);
            device.AddField(validKey1, validValue1);
            device.AddField(validKey2, validValue2);
            device.AddField(validKey3, validValue3);
            device.AddFile(validFileUid1, validFileName1);
            device.AddFile(validFileUid2, validFileName2);
            device.AddFile(validFileUid3, validFileName3);
            device.SetPhoto(validPhotoUid1, validPhotoName1);

            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(validDeviceTag));
            Assert.That(device.Manufacturer, Is.EqualTo(validManufacturer));
            Assert.That(device.ModelNum, Is.EqualTo(validModelNum));
            Assert.That(device.SerialNum, Is.EqualTo(validSerialNum));
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.EqualTo(validNotes));

            var lastModified = device.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));

            var location = device.Location;
            Assert.That(location.BuildingId, Is.EqualTo(validBuildingId));
            Assert.That(location.Notes, Is.EqualTo(validLocationNotes));
            Assert.That(location.Latitude, Is.EqualTo(validLatitude));
            Assert.That(location.Longitude, Is.EqualTo(validLongitude));

            var fields = device.Fields;
            Assert.That(fields[validKey1], Is.EqualTo(validValue1));
            Assert.That(fields[validKey2], Is.EqualTo(validValue2));
            Assert.That(fields[validKey3], Is.EqualTo(validValue3));

            var files = device.Files;
            Assert.That(files.Single(file => file.FileUid == validFileUid1 && file.FileName == validFileName1), Is.Not.Null);
            Assert.That(files.Single(file => file.FileUid == validFileUid2 && file.FileName == validFileName2), Is.Not.Null);
            Assert.That(files.Single(file => file.FileUid == validFileUid3 && file.FileName == validFileName3), Is.Not.Null);

            var photo = device.Photo;
            Assert.That(photo.FileUid == validPhotoUid1 && photo.FileName == validPhotoName1, Is.True);
        }

        [Test]
        public void DeviceNotInstantiated_InstantiateUsingObjectInitializerWithAllNullStringValues_GetBsonDocumentThrowsException()
        {
            var device = new Device
            {
                Id = validObjId,
                DeviceTypeId = validDeviceTypeId,
                DeviceTag = null,
                Manufacturer = null,
                ModelNum = null,
                SerialNum = null,
                YearManufactured = null,
                Notes = null
            };
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.Null);
            Assert.That(device.Manufacturer, Is.Null);
            Assert.That(device.ModelNum, Is.Null);
            Assert.That(device.SerialNum, Is.Null);
            Assert.That(device.YearManufactured, Is.Null);
            Assert.That(device.Notes, Is.Null);

            Assert.Throws<ArgumentNullException>(() => { device.GetBsonDocument(); });
        }

        [Test]
        public void DeviceNotInstantiated_InstantiateUsingFullConstructorWithAllNullStringValues_ValuesAreReplacedWithEmptyStrings()
        {
            var device = new Device(validObjId, validDeviceTypeId, null, null, null, null, null, null);
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(string.Empty));
            Assert.That(device.Manufacturer, Is.EqualTo(string.Empty));
            Assert.That(device.ModelNum, Is.EqualTo(string.Empty));
            Assert.That(device.SerialNum, Is.EqualTo(string.Empty));
            Assert.That(device.YearManufactured, Is.EqualTo(-1));
            Assert.That(device.Notes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void DeviceInstantiatedWithSomeNullValuesWithFullConstructor_CallGetBsonDocument_ExceptionIsThrown()
        {
            var device = new Device(validObjId, validDeviceTypeId, validDeviceTag, validManufacturer, null, null, validYearManufactured, null);
            device.SetLastModified(validDate, validUser);
            Assert.Throws<ArgumentNullException>(() => { device.GetBsonDocument(); });
        }

        [Test]
        public void DeviceInstantiatedWithSomeNullValuesWithObjectInitializer_CallGetBsonDocument_ExceptionIsThrown()
        {
            var device = new Device
            {
                Id = validObjId,
                DeviceTypeId = validDeviceTypeId,
                DeviceTag = validDeviceTag,
                Manufacturer = null,
                ModelNum = null,
                SerialNum = validSerialNum,
                YearManufactured = validYearManufactured,
                Notes = null
            };
            device.SetLocation(validBuildingId, validLocationNotes, validLatitude, validLongitude);
            Assert.Throws<ArgumentNullException>(() => { device.GetBsonDocument(); });
        }

        [Test]
        public void DeviceInstantiated_PassInNullsToSetterMethods_NullValuesReplacedWithDefaultValues()
        {
            var device = new Device
            {
                Id = validObjId,
                DeviceTypeId = validDeviceTypeId,
                DeviceTag = validDeviceTag,
                Manufacturer = validManufacturer,
                ModelNum = validModelNum,
                SerialNum = validSerialNum,
                YearManufactured = validYearManufactured,
                Notes = validNotes
            };
            device.SetLastModified(null, null);
            device.SetLocation(validBuildingId, null, null, null);

            var lastModified = device.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(string.Empty));

            var location = device.Location;
            Assert.That(location.BuildingId, Is.EqualTo(validBuildingId));
            Assert.That(location.Notes, Is.EqualTo(string.Empty));
            Assert.That(location.Latitude, Is.EqualTo(string.Empty));
            Assert.That(location.Longitude, Is.EqualTo(string.Empty));
        }
    }
}
