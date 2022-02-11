using NUnit.Framework;
using System.Threading.Tasks;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class SampleFT
    {
        BeimaClient Client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Client = new BeimaClient("http://localhost:3000");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Client.Dispose();
        }

        [Test]
        public async Task BackendRunning_PingBackend_ResponseRecieved()
        {
            var response = await Client.SendRequest("/api/Function1", HttpVerb.GET);
            Assert.IsNotNull(response);
        }
    }
}