using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.FT
{
    /// <summary>
    /// Base FT class that all FT classes should inherit from.
    /// </summary>
    [SetUpFixture]
    public class FunctionalTestBase
    {
        public BeimaClient TestClient = new BeimaClient("http://localhost:7071");

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
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
