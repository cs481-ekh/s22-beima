using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceFT : FunctionalTestBase
    {
        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("1234")]
        public void InvalidId_DeviceGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
                await TestClient.SendRequest($"api/device/${id}", HttpVerb.GET)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void NoDevicesInDatabase_DeviceGet_ReturnsNotFound()
        {
            var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
                await TestClient.SendRequest("api/device/1234567890abcdef12345678", HttpVerb.GET)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
