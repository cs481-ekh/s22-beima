using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class RulesTest : UnitTestBase
    {
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

            var expectedResult = expectedStatusCode.Equals(HttpStatusCode.OK);

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceValid(device, out message, out statusCode);

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

            var expectedResult = expectedStatusCode.Equals(HttpStatusCode.OK);

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceValid(device, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(message, Is.EqualTo(expectedMessage));
            Assert.That(statusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test]
        public void DeviceYearAndLocationInvalid_IsDeviceValid_ReturnsFalseAndInvalidYearMessage()
        {
            // ARRANGE
            var device = new Device(ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), "Tag", "Manufacturer", "Model", "SerialNumber", -1, "Notes");
            device.SetLocation(null, "Notes", "abc", "9999.0");

            string message;
            HttpStatusCode statusCode;

            // ACT
            var result = Rules.IsDeviceValid(device, out message, out statusCode);

            // ASSERT
            Assert.That(result, Is.False);
            Assert.That(message, Is.EqualTo("Device year manufactured is invalid."));
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        #endregion Device Rules

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
