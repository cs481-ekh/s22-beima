using MongoDB.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceTypeFT : FunctionalTestBase
    {
        [SetUp]
        public void SetUp()
        {
            // TODO: Remove all device types from database during setup
        }

        [Test]
        public async Task NoDeviceTypesInDb_AddDeviceType_DeviceTypeAddedSuccessfullyAsync()
        {
            // ARRANGE
            var device = new DeviceTypeAdd
            {
                Description = "This is a boiler type.",
                Name = "Boiler",
                Notes = "Some type notes.",
                Fields = new List<string>()
                {
                    "MaxTemperature",
                    "MinTemperature",
                    "Capacity"
                }
            };

            // ACT
            var responseId = await TestClient.AddDeviceType(device);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            Assert.That(ObjectId.TryParse(responseId, out _), Is.True);
            // TODO: GET device type and assert properties.
        }
    }
}
