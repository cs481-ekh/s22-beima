using BEIMA.Backend.DeviceFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.DeviceFunctions
{
    [TestFixture]
    public class DeleteDeviceTest : UnitTestBase
    {
        [Test]
        public void IdIsValid_DeleteDevice_DeletionSuccessful()
        {
            // ARRANGE

            var testGuid = "1234567890abcdef12345678";

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.DeleteDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testGuid))))
                  .Returns(true)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = DeleteDevice.Run(request, testGuid, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.DeleteDevice(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkResult)));
            Assert.That(((OkResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
