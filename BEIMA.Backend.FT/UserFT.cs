using MongoDB.Bson;
using NUnit.Framework;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class UserFT : FunctionalTestBase
    {
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