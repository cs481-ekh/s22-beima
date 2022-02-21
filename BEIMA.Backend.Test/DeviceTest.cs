using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class DeviceTest : UnitTestBase
    {

        #region FailureTests

        [Test]
        public async Task IdNotInDatabase_GetDevice_ReturnsNotFound()
        {
            // ARRANGE

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testGuid = "1234567890abcdef12345678";
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testGuid))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            // Set up the http request.
            var request = RequestFactory.CreateHttpRequest("id", testGuid);
            request.Method = "get";
            var logger = new LoggerFactory();

            // ACT
            var response = await Device.Run(request, logger.CreateLogger("Testing"));

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(ObjectResult)));
            Assert.That(((ObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((ObjectResult)response).Value, Is.EqualTo("Device could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        public async Task InvalidId_GetDevice_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = RequestFactory.CreateHttpRequest("id", id);
            request.Method = "get";
            var logger = new LoggerFactory();

            // ACT
            var response = await Device.Run(request, logger.CreateLogger("Testing"));

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid id."));
        }

        #endregion FailureTests

        #region SuccessTests

        [Test]
        public async Task IdIsValid_GetDevice_ReturnsDevice()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
<<<<<<< HEAD
            var testGuid = "1234567890abcdef1234567890abcdef";
=======
            var testGuid = "1234567890abcdef12345678";
>>>>>>> main
            var dictionary = new Dictionary<string, object>()
            {
                //TODO: add more properties
                ["serialNumber"] = "1234"
            };
            mockDb.Setup(mock => mock.GetDevice(It.Is<ObjectId>(oid => oid == new ObjectId(testGuid))))
                  .Returns(new BsonDocument(dictionary))
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = RequestFactory.CreateHttpRequest("id", testGuid);
            request.Method = "get";
            var logger = new LoggerFactory();

            // ACT
            var response = await Device.Run(request, logger.CreateLogger("Testing"));

            // ASSERT
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDevice(It.IsAny<ObjectId>()), Times.Once));

            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var device = ((OkObjectResult)response).Value.ToBsonDocument();
            Assert.That(device["serialNumber"].AsString, Is.EqualTo("1234"));
        }

        #endregion SuccessTests
    }
}
