using MongoDB.Bson;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class BuildingFT : FunctionalTestBase
    {
        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_BuildingGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetBuilding(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoBuildingsInDatabase_BuildingGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetBuilding(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

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
                    Longitude = "12.001",
                    Latitude = "24.321",
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
