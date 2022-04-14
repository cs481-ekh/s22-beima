using JWT.Builder;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class AuthFT : FunctionalTestBase
    {
        [SetUp]
        public async Task SetUpAsync()
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

        [TestCase(null, null)]
        [TestCase("valid", null)]
        [TestCase(null, "valid")]
        public void InvalidParameters_Login_ReturnsHttpStatus401(string username, string password)
        {
            var loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.Login(loginRequest)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase("notvalid", "notvalid")]
        [TestCase("notvalid2", "notvalid2")]
        public void ValidParametersNoUsers_Login_ReturnsHttpStatus401(string username, string password)
        {
            var loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var ex = Assert.ThrowsAsync<BeimaException>(async () =>
                await TestClient.Login(loginRequest)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [TestCase("valid", "Valid01!")]
        [TestCase("valid2", "Valid02!")]
        public async Task ValidParametersWithUsers_Login_ReturnsJwtToken(string username, string password)
        {
            var user = new User()
            {
                Username = username,
                Password = password,
                Role = "admin"
            };

            var addResponse = await TestClient.AddUser(user);
            Assume.That(addResponse, Is.Not.Null);

            var loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var token = await TestClient.Login(loginRequest);
            Assert.That(token, Is.Not.Null);

            var claims = new JwtBuilder().Decode<Claims>(token);
            Assert.That(claims.Username, Is.EqualTo(username));
            Assert.That(claims.Role, Is.EqualTo("admin"));

            // Expiration of token is 7 days after creation. Should be 6 days 23 hours 59min xx seconds greater then datetime now
            var nowPlus6 = DateTime.Now.AddDays(6).AddHours(23).AddMinutes(59).Ticks;
            var nowPlus7 = DateTime.Now.AddDays(7).Ticks;

            Assert.That(claims.Exp, Is.GreaterThan(nowPlus6));
            Assert.That(claims.Exp, Is.LessThan(nowPlus7));
            Assert.That(claims.Sub, Is.EqualTo("User"));
            Assert.That(claims.Iss, Is.EqualTo("Beima"));
        }

        [TestCase("valid", "ValId", "Valid01!")]
        [TestCase("valid2", "vaLID2", "Valid02!")]
        public async Task ValidParametersWithUsersWithCapitalizedUsername_Login_ReturnsJwtToken(string username, string capUsername, string password)
        {
            var user = new User()
            {
                Username = username,
                Password = password,
                Role = "admin"
            };

            var addResponse = await TestClient.AddUser(user);
            Assume.That(addResponse, Is.Not.Null);

            var loginRequest = new LoginRequest()
            {
                Username = capUsername,
                Password = password
            };

            var token = await TestClient.Login(loginRequest);
            Assert.That(token, Is.Not.Null);

            var claims = new JwtBuilder().Decode<Claims>(token);
            Assert.That(claims.Username, Is.EqualTo(username));
            Assert.That(claims.Role, Is.EqualTo("admin"));

            // Expiration of token is 7 days after creation. Should be 6 days 23 hours 59min xx seconds greater then datetime now
            var nowPlus6 = DateTime.Now.AddDays(6).AddHours(23).AddMinutes(59).Ticks;
            var nowPlus7 = DateTime.Now.AddDays(7).Ticks;

            Assert.That(claims.Exp, Is.GreaterThan(nowPlus6));
            Assert.That(claims.Exp, Is.LessThan(nowPlus7));
            Assert.That(claims.Sub, Is.EqualTo("User"));
            Assert.That(claims.Iss, Is.EqualTo("Beima"));
        }
    }
}
