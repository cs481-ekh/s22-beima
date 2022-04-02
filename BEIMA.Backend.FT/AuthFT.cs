using JWT.Builder;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class AuthFT : FunctionalTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            // Add in deleting all the users in the database once functionality implemented
        }

        [TestCase(null, null)]
        [TestCase("valid", null)]
        [TestCase(null, "valid")]
        public void InvalidParameters_Login_ReturnsNull(string username, string password)
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
        public void ValidParametersNoUsers_Login_ReturnsNull(string username, string password)
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

        [TestCase("valid", "valid")]
        [TestCase("valid2", "valid2")]
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
            Assert.That(claims.Exp, Is.GreaterThan(nowPlus6));
            Assert.That(claims.Sub, Is.EqualTo("User"));
            Assert.That(claims.Iss, Is.EqualTo("Beima"));
        }
    }
}
