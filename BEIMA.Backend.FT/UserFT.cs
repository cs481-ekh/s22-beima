using MongoDB.Bson;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class UserFT : FunctionalTestBase
    {
        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_UserGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetUser(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoUsersInDatabase_UserGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.GetUser(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task UserNotInDatabase_AddUser_CreatesNewUser()
        {
            // ARRANGE
            var user = new User
            {
                Username = "user.name",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };

            // ACT
            var responseId = await TestClient.AddUser(user);

            // ASSERT
            Assert.That(responseId, Is.Not.Null);
            Assert.That(ObjectId.TryParse(responseId, out _), Is.True);
            //TODO: add more in depth testing when get endpoint is implemented.
        }
    }
}