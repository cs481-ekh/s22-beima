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

            var getUser = await TestClient.GetUser(responseId);
            Assert.That(getUser.Username, Is.EqualTo(user.Username));
            // GET endpoints should not expose password, will always return empty string
            Assert.That(getUser.Password, Is.EqualTo(string.Empty));
            Assert.That(getUser.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(getUser.LastName, Is.EqualTo(user.LastName));
            Assert.That(getUser.Role, Is.EqualTo(user.Role));

            Assert.That(getUser.LastModified, Is.Not.Null);
            Assert.That(getUser.LastModified?.Date, Is.Not.Null);
            Assert.That(getUser.LastModified?.User, Is.EqualTo("Anonymous"));
        }
    }
}