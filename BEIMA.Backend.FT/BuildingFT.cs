using MongoDB.Bson;
using NUnit.Framework;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class BuildingFT : FunctionalTestBase
    {
        [Test]
        public async Task BuildingNotInDatabase_AddBuilding_CreatesNewBuilding()
        {
            // ARRANGE
            var building = new Building
            {
                Name = "Student Union",
                Number = "1234",
                Notes = "Some notes.",
                Location = new Location
                {
                    Longitude = "123.001",
                    Latitude = "101.321",
                },
            };

            // ACT
            var responseId = await TestClient.AddBuilding(building);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            Assert.That(ObjectId.TryParse(responseId, out _), Is.True);
            //TODO: add more in depth testing when get endpoint is implemented.
        }
    }
}
