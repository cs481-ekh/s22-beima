using BEIMA.Backend.UserFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.UserFunctions
{
    [TestFixture]
    public class AddUserTest : UnitTestBase
    {
        [Test]
        public async Task NoUser_AddUser_ReturnsValidId()
        {
            // ARRANGE
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.InsertUser(It.IsAny<BsonDocument>()))
                  .Returns(ObjectId.GenerateNewId())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Create request
            var body = TestData._testUser;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var userId = ((ObjectResult)await AddUser.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(userId);
            Assert.That(ObjectId.TryParse(userId, out _), Is.True);
        }
    }
}
