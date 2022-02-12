using NUnit.Framework;
using System.Threading.Tasks;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class SampleFT : FunctionalTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [Test]
        public async Task BackendRunning_PingBackend_ResponseRecieved()
        {
            var response = await TestClient.SendRequest("/api/Function1", HttpVerb.GET);
            Assert.IsNotNull(response);
        }
    }
}