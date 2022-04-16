using BEIMA.Backend.AuthService;
using BEIMA.Backend.BuildingFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.BuildingFunctions
{
    [TestFixture]
    public class GetBuildingListTest : UnitTestBase
    {
        [Test]
        public void PopulatedDatabase_GetBuildingList_ReturnsListOfBuildings()
        {
            // ARRANGE
            var building1 = new Building(ObjectId.GenerateNewId(), "Student Union", "1234", "Some notes 1.");
            var building2 = new Building(ObjectId.GenerateNewId(), "Interactive Learning Center", "4321", "Some notes 2.");
            var building3 = new Building(ObjectId.GenerateNewId(), "Albertons Library", "2468", "Some notes 3.");

            building1.SetLastModified(DateTime.UtcNow, "Anonymous");
            building2.SetLastModified(DateTime.UtcNow, "Anonymous");
            building3.SetLastModified(DateTime.UtcNow, "Anonymous");

            building1.SetLocation("0.123", "-0.321");
            building2.SetLocation("1.234", "-4.321");
            building3.SetLocation("1.001", "-2.002");

            var buildingList = new List<BsonDocument>
            {
                building1.GetBsonDocument(),
                building2.GetBsonDocument(),
                building3.GetBsonDocument(),
            };
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllBuildings())
                  .Returns(buildingList)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Setup mock authentication service.
            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (OkObjectResult)GetBuildingList.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllBuildings(), Times.Once));

            var getList = (List<Building>)response.Value;
            Assert.IsNotNull(getList);
            for (int i = 0; i < buildingList.Count; i++)
            {
                var building = getList[i];
                var expectedBuilding = buildingList[i];
                Assert.That(building.Id.ToString(), Is.EqualTo(expectedBuilding["_id"].AsObjectId.ToString()));
                Assert.That(building.Name, Is.EqualTo(expectedBuilding["name"].AsString));
                Assert.That(building.Number, Is.EqualTo(expectedBuilding["number"].AsString));
                Assert.That(building.Notes, Is.EqualTo(expectedBuilding["notes"].AsString));

                var lastMod = building.LastModified;
                var expectedLastMod = expectedBuilding["lastModified"];
                Assert.That(lastMod.Date, Is.EqualTo(expectedLastMod["date"].ToUniversalTime()));
                Assert.That(lastMod.User, Is.EqualTo(expectedLastMod["user"].AsString));

                var location = building.Location;
                var expectedLastLoc = expectedBuilding["location"];
                Assert.That(location.Latitude, Is.EqualTo(expectedLastLoc["latitude"].AsString));
                Assert.That(location.Longitude, Is.EqualTo(expectedLastLoc["longitude"].AsString));
            }
        }
    }
}
