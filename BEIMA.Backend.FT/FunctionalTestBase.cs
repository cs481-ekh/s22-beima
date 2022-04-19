using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BEIMA.Backend.FT.TestObjects;

namespace BEIMA.Backend.FT
{
    /// <summary>
    /// Base FT class that all FT classes should inherit from.
    /// </summary>
    public class FunctionalTestBase
    {
        public BeimaClient TestClient = new BeimaClient("http://localhost:7071");
        public BeimaClient NonAdminTestClient = new BeimaClient("http://localhost:7071");
        public BeimaClient UnauthorizedTestClient = new BeimaClient("http://localhost:7071");
        public readonly string TestUsername = "first.admin";
        public readonly string NonAdminTestUsername = "non.admin";
        public readonly string TestPassword = "Abcdefg12345!";

        public static bool NonAdminUserAdded = false;

        class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string>? Values { get; set; }
        }

        [OneTimeSetUp]
        public async Task BaseOneTimeSetUp()
        {
            // HTTP CLIENT SETUP

            if (TestClient != null)
            {
                TestClient.Dispose();
            }
            // Uncomment for Azure server:
            // BeimaClient Client = new BeimaClient("https://beima-service.azurewebsites.net");
            TestClient = new BeimaClient("http://localhost:7071");

            // Have at least one admin user in the DB at all times as tests will need it for auth
            var token = await TestClient.Login(new LoginRequest() { Username = TestUsername, Password = TestPassword });
            TestClient.SetAuthenticationHeader(token);

            // Set up a non admin user for testing
            if (!NonAdminUserAdded && (await TestClient.GetUserList()).Any(user => user.Username?.Equals(NonAdminTestUsername) ?? false))
            {
                var user = new User
                {
                    Username = NonAdminTestUsername,
                    Password = TestPassword,
                    FirstName = "Non",
                    LastName = "Admin",
                    Role = "user",
                };
                var nonAdminToken = await NonAdminTestClient.Login(new LoginRequest() { Username = NonAdminTestUsername, Password = TestPassword });
                TestClient.SetAuthenticationHeader(nonAdminToken);
                NonAdminUserAdded = true;
            }
        }

        [OneTimeTearDown]
        public void BaseOneTimeTearDown()
        {
            TestClient.Dispose();
        }
    }
}
