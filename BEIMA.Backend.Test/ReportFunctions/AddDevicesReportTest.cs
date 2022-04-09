using BEIMA.Backend.MongoService;
using BEIMA.Backend.ReportFunctions;
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
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.ReportFunctions
{
    [TestFixture]
    public class AddDevicesReportTest : UnitTestBase
    {
        [Test]
        public void DeviceAndDeviceTypeInDb_AllDevicesReport_ReturnsValidZipFileByteArray()
        {
            // ARRANGE
            var deviceType = new DeviceType(ObjectId.GenerateNewId(), "Generic", "This is a generic type.", "Some notes.");
            deviceType.AddField(Guid.NewGuid().ToString(), "TestField");
            deviceType.SetLastModified(DateTime.UtcNow, "Anonymous");

            var device = new Device(ObjectId.GenerateNewId(), deviceType.Id, "A-1", "Generic Inc.", "1234", "asdf", 2022, "Some notes");
            device.SetLocation(ObjectId.GenerateNewId(), "Some notes.", "1.234", "5.678");
            device.AddField(deviceType.Fields.Single().Name, "TestValue");
            device.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns(new List<BsonDocument> { deviceType.GetBsonDocument() })
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(new List<BsonDocument> { device.GetBsonDocument() })
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var request = CreateHttpRequest(RequestMethod.GET);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (FileContentResult)AllDevicesReport.Run(request, logger);

            // ASSERT
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
                    Assert.That(line1, Is.EqualTo("Id,DeviceTypeId,DeviceTag,Manufacturer,ModelNum,SerialNum,YearManufactured,Notes,TestField,BuildingId,Notes,Latitude,Longitude,Date,User"));
                    Assert.That(line2, Does.Match(@"^[0-9a-fA-F]{24},[0-9a-fA-F]{24},A-1,Generic Inc.,1234,asdf,2022,Some notes,TestValue,[0-9a-fA-F]{24},Some notes.,1.234,5.678,DATE,Anonymous"
                                                  .Replace("DATE", device.LastModified.Date.ToString())));
                }
            }
        }
    }
}
