using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace BEIMA.Backend.FT
{
    [TestFixture]
    public class DeviceFT : FunctionalTestBase
    {
        [TestCase("xxx")]
        [TestCase("1234")]
        [TestCase("1234567890abcdef1234567x")]
        public void InvalidId_DeviceGet_ReturnsInvalidId(string id)
        {
            var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
                await TestClient.SendRequest($"api/device/{id}", HttpVerb.GET)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("")]
        [TestCase("1234567890abcdef12345678")]
        public void NoDevicesInDatabase_DeviceGet_ReturnsNotFound(string id)
        {
            var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
                await TestClient.SendRequest($"api/device/{id}", HttpVerb.GET)
            );
            Assert.IsNotNull(ex);
            Assert.That(ex?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
