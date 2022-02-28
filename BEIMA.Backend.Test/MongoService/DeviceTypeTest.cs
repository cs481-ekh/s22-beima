using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class DeviceTypeTest
    {
        //Instance variables
        readonly ObjectId validObjId = ObjectId.GenerateNewId();
        readonly string validName = "Device Type Name 123";
        readonly string validDescription = "This is a device type.";
        readonly string validNotes = "Example of notes.";

        readonly DateTime validDate = DateTime.Now.ToUniversalTime();
        readonly string validUser = "User1";

        readonly string validKey1 = "key1";
        readonly string validKey2 = "key2";
        readonly string validKey3 = "key3";
        readonly string validValue1 = "value1";
        readonly bool validValue2 = true;
        readonly int validValue3 = 123;

        [Test]
        public void DeviceTypeNotInstantiated_InstantiateUsingFullConstructor_DeviceTypeInstantiatedWithCorrectValues()
        {
            var deviceType = new DeviceType(validObjId, validName, validDescription, validNotes);
            deviceType.SetLastModified(validDate, validUser);
            deviceType.AddField(validKey1, validValue1);
            deviceType.AddField(validKey2, validValue2);
            deviceType.AddField(validKey3, validValue3);

            Assert.That(deviceType.Id, Is.EqualTo(validObjId));
            Assert.That(deviceType.Name, Is.EqualTo(validName));
            Assert.That(deviceType.Description, Is.EqualTo(validDescription));
            Assert.That(deviceType.Notes, Is.EqualTo(validNotes));

            var lastModified = deviceType.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(validUser));

            var fields = deviceType.Fields;
            Assert.That((string)fields.GetElement(validKey1).Value, Is.EqualTo(validValue1));
            Assert.That((bool)fields.GetElement(validKey2).Value, Is.EqualTo(validValue2));
            Assert.That((int)fields.GetElement(validKey3).Value, Is.EqualTo(validValue3));
        }

        [Test]
        public void DeviceTypeNotInstantiated_InstantiateUsingObjectInitializer_DeviceInstantiatedWithCorrectValues()
        {
            var deviceType = new DeviceType
            {
                Id = validObjId,
                Name = validName,
                Description = validDescription,
                Notes = validNotes
            };
            deviceType.SetLastModified(validDate, validUser);
            deviceType.AddField(validKey1, validValue1);
            deviceType.AddField(validKey2, validValue2);
            deviceType.AddField(validKey3, validValue3);

            Assert.That(deviceType.Id, Is.EqualTo(validObjId));
            Assert.That(deviceType.Name, Is.EqualTo(validName));
            Assert.That(deviceType.Description, Is.EqualTo(validDescription));
            Assert.That(deviceType.Notes, Is.EqualTo(validNotes));

            var lastModified = deviceType.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(validUser));

            var fields = deviceType.Fields;
            Assert.That((string)fields.GetElement(validKey1).Value, Is.EqualTo(validValue1));
            Assert.That((bool)fields.GetElement(validKey2).Value, Is.EqualTo(validValue2));
            Assert.That((int)fields.GetElement(validKey3).Value, Is.EqualTo(validValue3));
        }

        [Test]
        public void DeviceTypeNotInstantiated_InstantiateUsingObjectInitializerWithAllNullStringValues_GetBsonDocumentThrowsException()
        {
            var deviceType = new DeviceType
            {
                Id = validObjId,
                Name = null,
                Description = null,
                Notes = null
            };
            Assert.That(deviceType.Id, Is.EqualTo(validObjId));
            Assert.That(deviceType.Name, Is.Null);
            Assert.That(deviceType.Description, Is.Null);
            Assert.That(deviceType.Notes, Is.Null);

            Assert.Throws<ArgumentNullException>(() => { deviceType.GetBsonDocument(); });
        }

        [Test]
        public void DeviceTypeNotInstantiated_InstantiateUsingFullConstructorWithAllNullStringValues_ValuesAreReplacedWithEmptyStrings()
        {
            var deviceType = new DeviceType(validObjId, null, null, null);
            Assert.That(deviceType.Id, Is.EqualTo(validObjId));
            Assert.That(deviceType.Name, Is.EqualTo(string.Empty));
            Assert.That(deviceType.Description, Is.EqualTo(string.Empty));
            Assert.That(deviceType.Notes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void DeviceTypeInstantiatedWithSomeNullValuesWithFullConstructor_CallGetBsonDocument_ExceptionIsThrown()
        {
            var deviceType = new DeviceType(validObjId, null, validDescription, validNotes);
            Assert.Throws<ArgumentNullException>(() => { deviceType.GetBsonDocument(); });
        }

        [Test]
        public void DeviceTypeInstantiatedWithSomeNullValuesWithObjectInitializer_CallGetBsonDocument_ExceptionIsThrown()
        {
            var deviceType = new DeviceType
            {
                Id = validObjId,
                Name = validName,
                Description = null,
                Notes = null
            };
            deviceType.SetLastModified(validDate, validUser);
            Assert.Throws<ArgumentNullException>(() => { deviceType.GetBsonDocument(); });
        }

        [Test]
        public void DeviceTypeInstantiated_PassInNullsToSetterMethods_NullValuesReplacedWithDefaultValues()
        {
            var deviceType = new DeviceType
            {
                Id = validObjId,
                Name = validName,
                Description = validDescription,
                Notes = validNotes
            };
            deviceType.SetLastModified(null, null);

            var lastModified = deviceType.LastModified;
            Assert.That((DateTime)lastModified.GetElement("date").Value, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
            Assert.That((string)lastModified.GetElement("user").Value, Is.EqualTo(string.Empty));
        }
    }
}
