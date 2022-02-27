using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;

namespace BEIMA.Backend.FT
{
    /// <summary>
    /// Base FT class that all FT classes should inherit from.
    /// </summary>
    public class FunctionalTestBase
    {
        public BeimaClient TestClient = new BeimaClient("http://localhost:7071");

        class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string>? Values { get; set; }
        }

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            // HTTP CLIENT SETUP

            if (TestClient != null)
            {
                TestClient.Dispose();
            }
            // Uncomment for Azure server:
            // BeimaClient Client = new BeimaClient("https://beima-service.azurewebsites.net");
            TestClient = new BeimaClient("http://localhost:7071");
        }

        [OneTimeTearDown]
        public void BaseOneTimeTearDown()
        {
            TestClient.Dispose();
        }
    }
}
