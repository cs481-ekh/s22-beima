using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class BaseDeviceTest
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
        readonly bool validValue2 = true;
        readonly int validValue3 = 123;

        [Test]
        public void BaseDeviceNotInstantiated_InstantiateUsingFullConstructor_BaseDeviceInstantiatedWithCorrectValues()
        {
            var device = new BaseDevice(validObjId, validDeviceTypeId, validDeviceTag, validManufacturer, validModelNum, validSerialNum, validYearManufactured, validNotes);
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(validDeviceTag));
            Assert.That(device.Manufacturer, Is.EqualTo(validManufacturer));
            Assert.That(device.ModelNum, Is.EqualTo(validModelNum));
            Assert.That(device.SerialNum, Is.EqualTo(validSerialNum));
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.EqualTo(validNotes));

            device.SetLastModified(validDate, validUser);
            var lastModified = device.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(validUser));

            device.SetLocation(validBuildingId, validLocationNotes, validLatitude, validLongitude);
            var location = device.Location;
            Assert.That((ObjectId)location.GetElement("buildingId").Value, Is.EqualTo(validBuildingId));
            Assert.That((string)location.GetElement("notes").Value, Is.EqualTo(validLocationNotes));
            Assert.That((string)location.GetElement("latitude").Value, Is.EqualTo(validLatitude));
            Assert.That((string)location.GetElement("longitude").Value, Is.EqualTo(validLongitude));

            device.AddField(validKey1, validValue1);
            device.AddField(validKey2, validValue2);
            device.AddField(validKey3, validValue3);
            var fields = device.Fields;
            Assert.That((string)fields.GetElement(validKey1).Value, Is.EqualTo(validValue1));
            Assert.That((bool)fields.GetElement(validKey2).Value, Is.EqualTo(validValue2));
            Assert.That((int)fields.GetElement(validKey3).Value, Is.EqualTo(validValue3));
        }

        [Test]
        public void BaseDeviceNotInstantiated_InstantiateUsingObjectInitializer_BaseDeviceInstantiatedWithCorrectValues()
        {
            var device = new BaseDevice
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
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(validDeviceTag));
            Assert.That(device.Manufacturer, Is.EqualTo(validManufacturer));
            Assert.That(device.ModelNum, Is.EqualTo(validModelNum));
            Assert.That(device.SerialNum, Is.EqualTo(validSerialNum));
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.EqualTo(validNotes));

            device.SetLastModified(validDate, validUser);
            var lastModified = device.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(validUser));

            device.SetLocation(validBuildingId, validLocationNotes, validLatitude, validLongitude);
            var location = device.Location;
            Assert.That((ObjectId)location.GetElement("buildingId").Value, Is.EqualTo(validBuildingId));
            Assert.That((string)location.GetElement("notes").Value, Is.EqualTo(validLocationNotes));
            Assert.That((string)location.GetElement("latitude").Value, Is.EqualTo(validLatitude));
            Assert.That((string)location.GetElement("longitude").Value, Is.EqualTo(validLongitude));

            device.AddField(validKey1, validValue1);
            device.AddField(validKey2, validValue2);
            device.AddField(validKey3, validValue3);
            var fields = device.Fields;
            Assert.That((string)fields.GetElement(validKey1).Value, Is.EqualTo(validValue1));
            Assert.That((bool)fields.GetElement(validKey2).Value, Is.EqualTo(validValue2));
            Assert.That((int)fields.GetElement(validKey3).Value, Is.EqualTo(validValue3));
        }

        [Test]
        public void BaseDeviceNotInstantiated_InstantiateUsingObjectInitializerWithAllNullStringValues_GetBsonDocumentThrowsException()
        {
            var device = new BaseDevice
            {
                Id = validObjId,
                DeviceTypeId = validDeviceTypeId,
                DeviceTag = null,
                Manufacturer = null,
                ModelNum = null,
                SerialNum = null,
                YearManufactured = validYearManufactured,
                Notes = null
            };
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.Null);
            Assert.That(device.Manufacturer, Is.Null);
            Assert.That(device.ModelNum, Is.Null);
            Assert.That(device.SerialNum, Is.Null);
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.Null);

            Assert.Throws<ArgumentNullException>(() => { device.GetBsonDocument(); });
        }

        [Test]
        public void BaseDeviceNotInstantiated_InstantiateUsingFullConstructorWithAllNullStringValues_ValuesAreReplacedWithEmptyStrings()
        {
            var device = new BaseDevice(validObjId, validDeviceTypeId, null, null, null, null, validYearManufactured, null);
            Assert.That(device.Id, Is.EqualTo(validObjId));
            Assert.That(device.DeviceTypeId, Is.EqualTo(validDeviceTypeId));
            Assert.That(device.DeviceTag, Is.EqualTo(string.Empty));
            Assert.That(device.Manufacturer, Is.EqualTo(string.Empty));
            Assert.That(device.ModelNum, Is.EqualTo(string.Empty));
            Assert.That(device.SerialNum, Is.EqualTo(string.Empty));
            Assert.That(device.YearManufactured, Is.EqualTo(validYearManufactured));
            Assert.That(device.Notes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void BaseDeviceInstantiatedWithSomeNullValuesWithFullConstructor_CallGetBsonDocument_ExceptionIsThrown()
        {
            var device = new BaseDevice(validObjId, validDeviceTypeId, validDeviceTag, validManufacturer, null, null, validYearManufactured, null);
            device.SetLastModified(validDate, validUser);
            Assert.Throws<ArgumentNullException>(() => { device.GetBsonDocument(); });
        }

        [Test]
        public void BaseDeviceInstantiatedWithSomeNullValuesWithObjectInitializer_CallGetBsonDocument_ExceptionIsThrown()
        {
            var device = new BaseDevice
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
        public void BaseDeviceInstantiated_PassInNullsToSetterMethods_NullValuesReplacedWithDefaultValues()
        {
            var device = new BaseDevice
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
            var lastModified = device.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(string.Empty));

            device.SetLocation(validBuildingId, null, null, null);
            var location = device.Location;
            Assert.That((ObjectId)location.GetElement("buildingId").Value, Is.EqualTo(validBuildingId));
            Assert.That((string)location.GetElement("notes").Value, Is.EqualTo(string.Empty));
            Assert.That((string)location.GetElement("latitude").Value, Is.EqualTo(string.Empty));
            Assert.That((string)location.GetElement("longitude").Value, Is.EqualTo(string.Empty));
        }
    }
}
