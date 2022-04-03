using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class RulesTest : UnitTestBase
    {
        private static string _longString = new string('x', Constants.MAX_CHARACTER_LENGTH + 1);

        #region Device Rules

        [TestCase(-1, "Device year manufactured is invalid.", HttpStatusCode.BadRequest)]
        [TestCase(0, "Device year manufactured is invalid.", HttpStatusCode.BadRequest)]
        [TestCase(999, "Device year manufactured is invalid.", HttpStatusCode.BadRequest)]
        [TestCase(10000, "Device year manufactured is invalid.", HttpStatusCode.BadRequest)]
        [TestCase(1000, "", HttpStatusCode.OK)]
        [TestCase(2022, "", HttpStatusCode.OK)]
        [TestCase(9999, "", HttpStatusCode.OK)]
        [TestCase(null, "", HttpStatusCode.OK)]
        public void DeviceWithYearManufactured_IsDeviceValid_ReturnsCorrectValue(int? yearManufactured, string expectedMessage, HttpStatusCode expectedStatusCode)
        {
            // ARRANGE
            var device = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "Tag", "Manufacturer", "Model", "SerialNumber", yearManufactured, "Notes");
            device.SetLocation(null, "Notes", "0.0", "0.0");
            device.SetFields(new Dictionary<string, string> { { Guid.NewGuid().ToString().ToString(), "TestValue" } });
            var deviceType = new DeviceType(device.DeviceTypeId, "Name", "Description", "Notes.");
            deviceType.AddField(device.Fields.Keys.Single(), "TestField");
            var expectedResult = expectedStatusCode.Equals(HttpStatusCode.OK);

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceValid(device, deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(message, Is.EqualTo(expectedMessage));
            Assert.That(statusCode, Is.EqualTo(expectedStatusCode));
        }

        [TestCaseSource(nameof(DeviceLocationTestCaseFactory))]
        public void DeviceWithLocation_IsDeviceValid_ReturnsCorrectValue(string lat, string lon, string expectedMessage, HttpStatusCode expectedStatusCode)
        {
            // ARRANGE
            var device = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "Tag", "Manufacturer", "Model", "SerialNumber", 2022, "Notes");
            device.SetLocation(null, "Notes", lat, lon);
            device.SetFields(new Dictionary<string, string> { { Guid.NewGuid().ToString().ToString(), "TestValue" } });
            var deviceType = new DeviceType(device.DeviceTypeId, "Name", "Description", "Notes.");
            deviceType.AddField(device.Fields.Keys.Single(), "TestField");

            var expectedResult = expectedStatusCode.Equals(HttpStatusCode.OK);

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceValid(device, deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(message, Is.EqualTo(expectedMessage));
            Assert.That(statusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test]
        public void DeviceAllFieldsInvalid_IsDeviceValid_ReturnsFalseAndInvalidMessages()
        {
            // ARRANGE
            var device = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), _longString, _longString, _longString, _longString, -1, _longString);
            device.SetLocation(null, _longString, "abc", "9999.0");
            device.SetFields(new Dictionary<string, string> { { Guid.NewGuid().ToString(), _longString } });
            var deviceType = new DeviceType(device.DeviceTypeId, "Name", "Description", "Notes.");
            deviceType.AddField(device.Fields.Keys.Single(), "TestField");

            string message;
            HttpStatusCode statusCode;

            var expectedMessage = "Device year manufactured is invalid.\n" +
                                  "Location is invalid.\n" +
                                  "The max character length on field \"DeviceTag\" has been exceeded.\n" +
                                  "The max character length on field \"Manufacturer\" has been exceeded.\n" +
                                  "The max character length on field \"ModelNum\" has been exceeded.\n" +
                                  "The max character length on field \"SerialNum\" has been exceeded.\n" +
                                  "The max character length on field \"Notes\" has been exceeded.\n" +
                                  "The max character length on field \"TestField\" has been exceeded.\n" +
                                  "The max character length on field \"Location Notes\" has been exceeded.";

            // ACT
            var result = Rules.IsDeviceValid(device, deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        #endregion Device Rules

        #region Device Type Rules

        [Test]
        public void DeviceTypeValid_IsDeviceTypeValid_ReturnsTrue()
        {
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), "Name", "Description", "Notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceTypeValid(deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.True);
            Assert.That(message, Is.EqualTo(string.Empty));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void DeviceTypeMatchingFields_IsDeviceTypeValid_ReturnsFalse()
        {
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), "Name", "Description", "Notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceTypeValid(deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo("Cannot have matching field names."));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void DeviceTypeTooManyCharacterField_IsDeviceTypeValid_ReturnsFalse()
        {
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), _longString, "Description", "Notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceTypeValid(deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo("The max character length on field \"Name\" has been exceeded."));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void DeviceTypeTooManyCharacterCustomField_IsDeviceTypeValid_ReturnsFalse()
        {
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), "Name", "Description", "Notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), _longString);

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceTypeValid(deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo($"The max character length on field \"{_longString}\" has been exceeded."));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void DeviceTypeAllFieldsInvalid_IsDeviceTypeValid_ReturnsFalse()
        {
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), _longString, _longString, _longString);
            deviceType.AddField(Guid.NewGuid().ToString(), _longString);
            deviceType.AddField(Guid.NewGuid().ToString(), _longString);

            string message;
            HttpStatusCode statusCode;

            var expectedMessage = "Cannot have matching field names.\n" +
                                  $"The max character length on field \"{_longString}\" has been exceeded.\n" +
                                  $"The max character length on field \"{_longString}\" has been exceeded.\n" +
                                  $"The max character length on field \"Name\" has been exceeded.\n" +
                                  $"The max character length on field \"Description\" has been exceeded.\n" +
                                  $"The max character length on field \"Notes\" has been exceeded.";

            // ACT
            var result = Rules.IsDeviceTypeValid(deviceType, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo(expectedMessage));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        #endregion Device Type Rules

        /// <summary>
        /// Generates a set of test case parameters relating to the device location.
        /// </summary>
        /// <returns>A set of test case parameters relating to the device location.</returns>
        private static IEnumerable<object[]?> DeviceLocationTestCaseFactory()
        {
            var lats = new List<string> { "-90.1", "90.1", "abc", "-90.0", "12.345", "90.0", "" };
            var longs = new List<string> { "-180.1", "180.1", "abc", "-180.0", "12.345", "180.0", "" };
            var validStartIndex = 3;

            for (var i = 0; i < lats.Count; i++)
            {

                for (var j = 0; j < longs.Count; j++)
                {
                    if (!(i >= validStartIndex && j >= validStartIndex))
                    {
                        yield return new object[] { lats[i], longs[j], "Location is invalid.", HttpStatusCode.BadRequest };
                    }
                    else
                    {
                        yield return new object[] { lats[i], longs[j], "", HttpStatusCode.OK };
                    }
                }
            }
        }
    }
}
