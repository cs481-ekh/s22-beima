using BEIMA.Backend.AuthService;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using BEIMA.Backend.ReportFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.ReportFunctions
{
    [TestFixture]
    public class AllDevicesReportTest : UnitTestBase
    {
        [Test]
        public void DeviceAndDeviceTypeInDb_AllDevicesReport_ReturnsValidZipFileByteArray()
        {
            // ARRANGE
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), "Generic", "This is a generic type.", "Some notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");

            var buildingOne = new Building()
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Farm",
                Number = "5",
                Notes = "It smells",
                Location = new BuildingLocation()
                {
                    Latitude = "",
                    Longitude = ""
                },
                LastModified = new BuildingLastModified()
                {
                    Date = DateTime.UtcNow,
                    User = "Anonymous"
                }
            };

            var device = new Device(ObjectId.GenerateNewId(), deviceType.Id, "A-1", "Generic Inc.", "1234", "asdf", 2022, "Some notes");
            device.SetLocation(buildingOne.Id, "Some notes.", "1.234", "5.678");
            device.AddField(deviceType.Fields.Single().Name, "TestValue");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns(new List<BsonDocument> { deviceType.GetBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument> { device.GetBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllBuildings())
                .Returns(new List<BsonDocument> { buildingOne.GetBsonDocument() })
                .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (FileContentResult)AllDevicesReport.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDeviceTypes(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));

            Assert.That(response.FileDownloadName, Is.EqualTo("devices_report.zip"));

            // Open the zip archive
            using (MemoryStream mem = new MemoryStream(response.FileContents))
            using (var zipArchive = new ZipArchive(mem, ZipArchiveMode.Read))
            {
                // Open the csv file
                var entry = zipArchive.Entries.Single(e => e.Name == "Generic.csv");
                using (var entryStream = entry.Open())
                using (var streamReader = new StreamReader(entryStream))
                {
                    // Read in and assert on each line of the csv
                    var line1 = streamReader.ReadLine();
                    var line2 = streamReader.ReadLine();
                    Assert.That(line1, Is.EqualTo("Id,DeviceTypeName,DeviceTag,Manufacturer,ModelNum,SerialNum,YearManufactured,Notes,TestField,BuildingName,Notes,Latitude,Longitude,Date,User"));
                    Assert.That(line2, Does.Match(@"^[0-9a-fA-F]{24},Generic,A-1,Generic Inc.,1234,asdf,2022,Some notes,TestValue,Farm,Some notes.,1.234,5.678,DATE,Anonymous"
                                                  .Replace("DATE", device.LastModified.Date.ToString())));
                }
            }
        }

        [Test]
        public void InvalidCredentials_AllDevicesReport_ReturnsUnauthorized()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns<Claims>(null)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)AllDevicesReport.Run(request, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDeviceTypes(), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Value, Is.EqualTo("Invalid credentials."));
        }
    }
}
