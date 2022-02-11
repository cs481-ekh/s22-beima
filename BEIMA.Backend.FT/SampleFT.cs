using Newtonsoft.Json;
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
            //Client = new BeimaClient("https://beima-service.azurewebsites.net");
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
            var thing = BeimaClient.ExtractObject(response);
            Assert.IsNotNull(response);
        }
    }
}