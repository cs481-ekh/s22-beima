using MongoDB.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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

        [Test, Explicit("Must run on a database with no users. Will need to implement the delete endpoint to run with the others.")]
        public async Task UsersInDatabase_GetUserList_ReturnsUserList()
        {
            // ARRANGE
            var userList = new List<User>
            {
                new User
                {
                    Username = "user.name1",
                    Password = "Abcdefg12345!1",
                    FirstName = "Alex",
                    LastName = "Smith",
                    Role = "user"
                },
                new User
                {
                    Username = "user.name2",
                    Password = "Abcdefg12345!2",
                    FirstName = "Ali",
                    LastName = "Glenn",
                    Role = "user"
                },
                new User
                {
                    Username = "user.name3",
                    Password = "Abcdefg12345!3",
                    FirstName = "Aaron",
                    LastName = "Bart",
                    Role = "user"
                }
            };

            foreach (var user in userList)
            {
                user.Id = await TestClient.AddUser(user);
            }

            // ACT
            var actualUsers = await TestClient.GetUserList();

            // ASSERT
            Assert.That(actualUsers.Count, Is.EqualTo(3));

            foreach (var user in actualUsers)
            {
                Assert.That(user, Is.Not.Null);
                var expectedUser = userList.Single(b => b.Id?.Equals(user.Id) ?? false);

                Assert.That(user.Username, Is.EqualTo(expectedUser.Username));
                Assert.That(user.Password, Is.EqualTo(string.Empty));
                Assert.That(user.FirstName, Is.EqualTo(expectedUser.FirstName));
                Assert.That(user.LastName, Is.EqualTo(expectedUser.LastName));
                Assert.That(user.Role, Is.EqualTo(expectedUser.Role));

                Assert.That(user.LastModified, Is.Not.Null);
                Assert.That(user.LastModified?.Date, Is.Not.Null);
                Assert.That(user.LastModified?.User, Is.EqualTo("Anonymous"));
            }
        }
    }