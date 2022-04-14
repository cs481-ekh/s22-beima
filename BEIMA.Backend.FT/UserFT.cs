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
        #region SetUp and TearDown

        [SetUp]
        public async Task SetUp()
        {
            // Have at least one admin user in the DB at all times
            // since DeleteUser prevents deletion when there is not at least one admin user.
            var initialUserList = await TestClient.GetUserList();
            if (!initialUserList.Any(user => user.Username == "first.admin"))
            {
                var adminUser = new User
                {
                    Username = "first.admin",
                    Password = "Abcdefg12345!",
                    FirstName = "First",
                    LastName = "Last",
                    Role = "admin"
                };
                await TestClient.AddUser(adminUser);
            }

            // Delete all the users in the database

            // Grab a new list
            var userList = await TestClient.GetUserList();
            foreach (var user in userList)
            {
                if (user?.Id is not null && user?.Username != "first.admin")
                {
                    var id = user?.Id ?? string.Empty;
                    await TestClient.DeleteUser(id);
                }
            }
        }

        #endregion SetUp and TearDown

        #region User GET Tests

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

        [TestCase("1234", true)]
        [TestCase("1234", false)]
        [TestCase("1234567890abcdef1234567x", true)]
        [TestCase("1234567890abcdef1234567x", false)]
        [TestCase("1234567890abcdef12345678", true)]
        [TestCase("1234567890abcdef12345678", false)]
        public void NotAuthorized_UserGet_ReturnsUnauthorized(string id, bool isNonAdmin)
        {
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await (isNonAdmin ? NonAdminTestClient : UnauthorizedTestClient).GetUser(id)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        #endregion User GET Tests

        #region User Add Tests

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
            Assert.That(getUser.LastModified?.User, Is.EqualTo(TestUsername));
        }

        [Test]
        public async Task UserInDatabase_AddUserWithDuplicateUsername_ConflictObjectResultReturned()
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
            await TestClient.AddUser(user);

            // ACT
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.AddUser(user)
            );

            // ASSERT
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void UnauthorizedUser_AddUser_ReturnsUnauthorized(bool isNonAdmin)
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
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await (isNonAdmin ? NonAdminTestClient : UnauthorizedTestClient).AddUser(user)
            );

            // ASSERT
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        #endregion User Add Tests

        #region User List Tests

        [Test]
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

            // Factoring in the "first.admin" user
            Assert.That(actualUsers.Count, Is.EqualTo(4));

            foreach (var user in actualUsers)
            {
                // Skip the "first.admin" user since we assume it will always be in there.
                if (user.Username != "first.admin")
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
                    Assert.That(user.LastModified?.User, Is.EqualTo(TestUsername));
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task UnauthorizedUser_GetUserList_ReturnsAnauthorized(bool isNonAdmin)
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
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await (isNonAdmin ? NonAdminTestClient : UnauthorizedTestClient).GetUserList()
            );

            // ASSERT
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        #endregion User List Tests

        #region User Delete Tests

        [Test]
        public async Task UserInDatabase_DeleteUser_UserDeletedSuccessfully()
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

            var userId = await TestClient.AddUser(user);
            Assume.That(await TestClient.GetUser(userId), Is.Not.Null);

            // ACT
            Assert.DoesNotThrowAsync(async () => await TestClient.DeleteUser(userId));

            // ASSERT
            var ex = Assert.ThrowsAsync<BeimaException>(async () => await TestClient.GetUser(userId));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task UnauthorizedUser_DeleteUser_ReturnsUnauthorized(bool isNonAdmin)
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

            var userId = await TestClient.AddUser(user);
            Assume.That(await TestClient.GetUser(userId), Is.Not.Null);

            // ACT
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await (isNonAdmin ? NonAdminTestClient : UnauthorizedTestClient).DeleteUser(userId)
            );

            // ASSERT
            Assert.DoesNotThrowAsync(async () => await TestClient.GetUser(userId));

            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        #endregion User Delete Tests

        #region User Update Tests

        [Test]
        public async Task UserInDatabase_UpdateUserWithPasswordAndTryLogin_ReturnsUpdatedUserAndSuccessfulLogin()
        {
            // ARRANGE
            var origUser = new User
            {
                Username = "user.name",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };

            var userId = await TestClient.AddUser(origUser);
            var origItem = await TestClient.GetUser(userId);

            // Verify login works with current password.
            var originalLoginRequest = new LoginRequest
            {
                Username = origUser.Username,
                Password = origUser.Password
            };
            var originalToken = await TestClient.Login(originalLoginRequest);

            Assume.That(origItem, Is.Not.Null);
            Assume.That(originalToken, Is.Not.Null);
            Assume.That(originalToken, Is.Not.EqualTo(string.Empty));

            var updateItem = new User
            {
                Id = userId,
                Username = "user.name",
                Password = "NewPassword123!",
                FirstName = "Alexis",
                LastName = "Smith",
                Role = "user"
            };

            // ACT
            var updatedUser = await TestClient.UpdateUser(updateItem);

            // Verify the password has been updated successfully.
            var newLoginRequest = new LoginRequest
            {
                Username = updateItem.Username,
                Password = updateItem.Password
            };
            var newToken = await TestClient.Login(newLoginRequest);

            // ASSERT
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser.FirstName, Is.Not.EqualTo(origUser.FirstName));

            Assert.That(updatedUser.Id, Is.EqualTo(updateItem.Id));
            Assert.That(updatedUser.Username, Is.EqualTo(updateItem.Username));
            Assert.That(updatedUser.LastName, Is.EqualTo(updateItem.LastName));
            Assert.That(updatedUser.Role, Is.EqualTo(updateItem.Role));

            Assert.That(newToken, Is.Not.Null);
            Assert.That(newToken, Is.Not.EqualTo(string.Empty));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task UserInDatabase_UpdateUserWithBlankPasswordAndTryLogin_ReturnsUpdatedUserAndSuccessfulLogin(string pw)
        {
            // ARRANGE
            var origUser = new User
            {
                Username = "user.name",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };

            var userId = await TestClient.AddUser(origUser);
            var origItem = await TestClient.GetUser(userId);

            // Verify login works with current password.
            var originalLoginRequest = new LoginRequest
            {
                Username = origUser.Username,
                Password = origUser.Password
            };
            var originalToken = await TestClient.Login(originalLoginRequest);

            Assume.That(origItem, Is.Not.Null);
            Assume.That(originalToken, Is.Not.Null);
            Assume.That(originalToken, Is.Not.EqualTo(string.Empty));

            var updateItem = new User
            {
                Id = userId,
                Username = "user.name",
                Password = pw,
                FirstName = "Alexis",
                LastName = "Smith",
                Role = "user"
            };

            // ACT
            var updatedUser = await TestClient.UpdateUser(updateItem);

            // Verify the password was not updated.
            var newLoginRequest = new LoginRequest
            {
                Username = updateItem.Username,
                Password = origUser.Password
            };
            var newToken = await TestClient.Login(newLoginRequest);

            // ASSERT
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser.FirstName, Is.Not.EqualTo(origUser.FirstName));

            Assert.That(updatedUser.Id, Is.EqualTo(updateItem.Id));
            Assert.That(updatedUser.Username, Is.EqualTo(updateItem.Username));
            Assert.That(updatedUser.LastName, Is.EqualTo(updateItem.LastName));
            Assert.That(updatedUser.Role, Is.EqualTo(updateItem.Role));

            Assert.That(newToken, Is.Not.Null);
            Assert.That(newToken, Is.Not.EqualTo(string.Empty));
        }

        [Test]
        public async Task TwoUsersInDatabase_UpdateUserWithDuplicateUsername_ConflictObjectResultReturned()
        {
            // ARRANGE
            var existingUser = new User
            {
                Username = "user.name",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };
            var existingUserId = await TestClient.AddUser(existingUser);
            Assume.That(existingUserId, Is.Not.Null);
            Assume.That(existingUserId, Is.Not.EqualTo(string.Empty));

            var updateUser = new User
            {
                Username = "someOtherUsername",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };

            var userId = await TestClient.AddUser(updateUser);
            var origItem = await TestClient.GetUser(userId);

            Assume.That(origItem, Is.Not.Null);

            var updateItem = new User
            {
                Id = userId,
                Username = "user.name",
                Password = "",
                FirstName = "Alexis",
                LastName = "Smith",
                Role = "user"
            };

            // ACT
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.UpdateUser(updateItem)
            );

            // ASSERT
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task UnauthorizedUser_UpdateUserWithPassword_ReturnsUnauthorized(bool isNonAdmin)
        {
            // ARRANGE
            var origUser = new User
            {
                Username = "user.name",
                Password = "Abcdefg12345!",
                FirstName = "Alex",
                LastName = "Smith",
                Role = "user"
            };

            var userId = await TestClient.AddUser(origUser);
            var origItem = await TestClient.GetUser(userId);

            // Verify login works with current password.
            var originalLoginRequest = new LoginRequest
            {
                Username = origUser.Username,
                Password = origUser.Password
            };
            var originalToken = await TestClient.Login(originalLoginRequest);

            Assume.That(origItem, Is.Not.Null);
            Assume.That(originalToken, Is.Not.Null);
            Assume.That(originalToken, Is.Not.EqualTo(string.Empty));

            var updateItem = new User
            {
                Id = userId,
                Username = "user.name",
                Password = "NewPassword123!",
                FirstName = "Alexis",
                LastName = "Smith",
                Role = "user"
            };

            // ACT
            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await (isNonAdmin ? NonAdminTestClient : UnauthorizedTestClient).UpdateUser(updateItem)
            );

            // ASSERT
            Assert.IsNotNull(ex);
            Assert.That(ex?.Message, Does.Contain("Invalid credentials."));
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        #endregion User Update Tests
    }
}