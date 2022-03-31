using NUnit.Framework;
using BEIMA.Backend.ReportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEIMA.Backend.MongoService;
using Moq;
using MongoDB.Bson;
using System.IO.Compression;
using System.IO;
using MongoDB.Driver;

namespace BEIMA.Backend.Test.ReportServicesa
{
    [TestFixture]
    public class ReportServiceTest : UnitTestBase
    {

        [Test]
        public void ServiceNotCreated_CallConstructorFiveTimes_FiveInstancesAreEqual()
        {
            //Tests to make sure singleton is working
            var reportService1 = ReportWriter.Instance;
            var reportService2 = ReportWriter.Instance;
            var reportService3 = ReportWriter.Instance;
            var reportService4 = ReportWriter.Instance;
            var reportService5 = ReportWriter.Instance;
            Assert.That(reportService1, Is.EqualTo(reportService2));
            Assert.That(reportService1, Is.EqualTo(reportService3));
            Assert.That(reportService1, Is.EqualTo(reportService4));
            Assert.That(reportService1, Is.EqualTo(reportService5));
        }

        [Test]
        public void InvalidDeviceType_GenerateByDeviceType_NullReturned()
        {
            var invalidDeviceTypeId = ObjectId.GenerateNewId();

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == invalidDeviceTypeId)))
                  .Returns<List<BsonDocument>>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var reportService = ReportWriter.Instance;

            // ACT
            var bytes = reportService.GeneratDeviceReportByDeviceType(invalidDeviceTypeId);

            // Assert
            Assert.That(bytes, Is.Null);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == invalidDeviceTypeId)), Times.Once));
        }

        [Test]
        public void NoDevicesOrDeviceTypesExist_GenerateAll_NullReturned()
        {
            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns<List<BsonDocument>>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns<List<BsonDocument>>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var reportService = ReportWriter.Instance;

            // ACT
            var bytes = reportService.GenerateAllDeviceReports();

            // Assert
            Assert.That(bytes, Is.Null);            
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDeviceTypes(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Never));
        }

        [Test]
        public void ValidDeviceType_GenerateByDeviceType_FileReturned()
        {
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceOne = new Device(ObjectId.GenerateNewId(), deviceTypeOne.Id, "dTag1", "dMan1", "dMod1", "dSer1", 2001, "dNote1");
            deviceOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceOne.SetLocation(new ObjectId("111111111111111111111111"), "dLocNotes1", "1", "1");
            deviceOne.AddField("boilerf1", "BoilerValue1D1");
            deviceOne.AddField("boilerf2", "BoilerValue2D1");
            deviceOne.AddField("boilerf3", "BoilerValue3D1");

            var deviceTwo = new Device(ObjectId.GenerateNewId(), deviceTypeOne.Id, "dTag2", "dMan2", "dMod2", "dSer2", 2002, "dNote2");
            deviceTwo.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTwo.SetLocation(new ObjectId("111111111111111111111111"), "dLocNotes2", "2", "2");
            deviceTwo.AddField("boilerf1", "BoilerValue1D2");
            deviceTwo.AddField("boilerf2", "BoilerValue2D2");
            deviceTwo.AddField("boilerf3", "BoilerValue3D2");

            var deviceDocs = new List<BsonDocument>()
            {
                deviceOne.GetBsonDocument(),
                deviceTwo.GetBsonDocument(),
            };

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == deviceTypeOne.Id)))
                  .Returns(deviceTypeOne.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredDevices(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(deviceDocs)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var reportService = ReportWriter.Instance;

            // ACT
            var bytes = reportService.GeneratDeviceReportByDeviceType(deviceTypeOne.Id);

            // Assert
            Assert.That(bytes, Is.Not.Null);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetDeviceType(It.Is<ObjectId>(oid => oid == deviceTypeOne.Id)), Times.Once));

            var content = Encoding.UTF8.GetString(bytes);
            var contentRows = content.Split(Environment.NewLine);
            Assert.That(contentRows.Count, Is.EqualTo(3));

            // Tests that first row is a header row
            Assert.That(GenerateDeviceHeaders(deviceTypeOne), Is.EqualTo(contentRows[0]));
            Assert.That(GenerateDeviceValues(deviceOne, deviceTypeOne), Is.EqualTo(contentRows[1]));
            Assert.That(GenerateDeviceValues(deviceTwo, deviceTypeOne), Is.EqualTo(contentRows[2]));
        }

        [Test]
        public void NoDevicesExist_GenerateAll_ZipContainesAllDevices()
        {
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceTypeTwo = new DeviceType(ObjectId.GenerateNewId(), "Hvac", "This is a hvac", "Hvac type notes");
            deviceTypeTwo.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeTwo.AddField("havcf1", "HvacField1");
            deviceTypeTwo.AddField("hvacf2", "HvacFeild2");

            var deviceTypeDocs = new List<BsonDocument>()
            {
                deviceTypeOne.GetBsonDocument(),
                deviceTypeTwo.GetBsonDocument(),
            };

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns<List<BsonDocument>>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns(deviceTypeDocs)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var reportService = ReportWriter.Instance;

            // ACT
            var bytes = reportService.GenerateAllDeviceReports();

            // Assert
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDeviceTypes(), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetAllDevices(), Times.Once));

            using (var memStream = new MemoryStream(bytes))
            {
                var zipFile = new ZipArchive(memStream);

                // Test that the zip has the right amount of files and that they have the right names
                Assert.That(zipFile.Entries.Count, Is.EqualTo(2));

                var file1 = zipFile.Entries[0];
                var file2 = zipFile.Entries[1];

                Assert.That(file1.Name, Is.EqualTo($"{deviceTypeOne.Name}.csv"));
                Assert.That(file2.Name, Is.EqualTo($"{deviceTypeTwo.Name}.csv"));

                // Read all the files and validate their content
                string file1Text;
                string file2Text;
                using (var readerOne = new StreamReader(file1.Open()))
                using (var readerTwo = new StreamReader(file2.Open()))
                {
                    file1Text = readerOne.ReadToEnd();
                    file2Text = readerTwo.ReadToEnd();
                }

                // Test that there is a header row and a row for every device
                var file1Rows = file1Text.Split(Environment.NewLine);
                var file2Rows = file2Text.Split(Environment.NewLine);

                Assert.That(file1Rows.Count, Is.EqualTo(1));
                Assert.That(file2Rows.Count, Is.EqualTo(1));

                // Tests that first row is a header row
                Assert.That(GenerateDeviceHeaders(deviceTypeOne), Is.EqualTo(file1Rows[0]));
                Assert.That(GenerateDeviceHeaders(deviceTypeTwo), Is.EqualTo(file2Rows[0]));
            }
        }

        [Test]
        public void DevicesAndDeviceTypesExists_GenerateAll_ZipContainesAllDevices()
        {
            // Test Data
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceTypeTwo = new DeviceType(ObjectId.GenerateNewId(), "Hvac", "This is a hvac", "Hvac type notes");
            deviceTypeTwo.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeTwo.AddField("havcf1", "HvacField1");
            deviceTypeTwo.AddField("hvacf2", "HvacFeild2");

            var deviceOne = new Device(ObjectId.GenerateNewId(), deviceTypeOne.Id, "dTag1", "dMan1", "dMod1", "dSer1", 2001, "dNote1");
            deviceOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceOne.SetLocation(new ObjectId("111111111111111111111111"), "dLocNotes1", "1", "1");
            deviceOne.AddField("boilerf1", "BoilerValue1D1");
            deviceOne.AddField("boilerf2", "BoilerValue2D1");
            deviceOne.AddField("boilerf3", "BoilerValue3D1");

            var deviceTwo = new Device(ObjectId.GenerateNewId(), deviceTypeOne.Id, "dTag2", "dMan2", "dMod2", "dSer2", 2002, "dNote2");
            deviceTwo.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTwo.SetLocation(new ObjectId("111111111111111111111111"), "dLocNotes2", "2", "2");
            deviceTwo.AddField("boilerf1", "BoilerValue1D2");
            deviceTwo.AddField("boilerf2", "BoilerValue2D2");
            deviceTwo.AddField("boilerf3", "BoilerValue3D2");

            var deviceThree = new Device(ObjectId.GenerateNewId(), deviceTypeTwo.Id, "dTag3", "dMan3", "dMod3", "dSer3", 2003, "dNote3");
            deviceThree.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceThree.SetLocation(new ObjectId("111111111111111111111111"), "dLocNotes3", "3", "3");
            deviceThree.AddField("havcf1", "HvacField1D3");
            deviceThree.AddField("hvacf2", "HvacFeild2D3");

            var deviceDocs = new List<BsonDocument>()
            {
                deviceOne.GetBsonDocument(),
                deviceTwo.GetBsonDocument(),
                deviceThree.GetBsonDocument()
            };

            var deviceTypeDocs = new List<BsonDocument>()
            {
                deviceTypeOne.GetBsonDocument(),
                deviceTypeTwo.GetBsonDocument(),
            };

            // Validation Data
            var excludedProps = new List<string>()
            {
                "Fields", "Location", "LastModified", "Photo", "Files"
            };
            var devicePropCount = typeof(Device).GetProperties().Where(val => excludedProps.Contains(val.Name) == false).Count();
            devicePropCount += typeof(DeviceLocation).GetProperties().Count();
            devicePropCount += typeof(DeviceLastModified).GetProperties().Count();

            // Setup mock database client.
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetAllDevices())
                  .Returns(deviceDocs)
                  .Verifiable();
            mockDb.Setup(mock => mock.GetAllDeviceTypes())
                  .Returns(deviceTypeDocs)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var reportService = ReportWriter.Instance;

            // ACT
            var bytes = reportService.GenerateAllDeviceReports();

            //ASSERT 
            using (var memStream = new MemoryStream(bytes)){
                var zipFile = new ZipArchive(memStream);

                // Test that the zip has the right amount of files and that they have the right names
                Assert.That(zipFile.Entries.Count, Is.EqualTo(2));

                var file1 = zipFile.Entries[0];
                var file2 = zipFile.Entries[1];

                Assert.That(file1.Name, Is.EqualTo($"{deviceTypeOne.Name}.csv"));
                Assert.That(file2.Name, Is.EqualTo($"{deviceTypeTwo.Name}.csv"));

                // Read all the files and validate their content
                string file1Text;
                string file2Text;
                using (var readerOne = new StreamReader(file1.Open()))
                using (var readerTwo = new StreamReader(file2.Open()))
                {
                    file1Text = readerOne.ReadToEnd();
                    file2Text = readerTwo.ReadToEnd();
                }

                // Test that there is a header row and a row for every device
                var file1Rows = file1Text.Split(Environment.NewLine);
                var file2Rows = file2Text.Split(Environment.NewLine);

                Assert.That(file1Rows.Count, Is.EqualTo(3));
                Assert.That(file2Rows.Count, Is.EqualTo(2));

                // Tests that first row is a header row
                Assert.That(GenerateDeviceHeaders(deviceTypeOne), Is.EqualTo(file1Rows[0]));
                Assert.That(GenerateDeviceHeaders(deviceTypeTwo), Is.EqualTo(file2Rows[0]));

                // Tests that subsequent rows are device data
                Assert.That(GenerateDeviceValues(deviceOne, deviceTypeOne), Is.EqualTo(file1Rows[1]));
                Assert.That(GenerateDeviceValues(deviceTwo, deviceTypeOne), Is.EqualTo(file1Rows[2]));
                Assert.That(GenerateDeviceValues(deviceThree, deviceTypeTwo), Is.EqualTo(file2Rows[1]));
            }            
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property names in the Device object. The Fields
        /// property is replaced with the field names contained in the parameter deviceType's Fields dictionary
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string GenerateDeviceHeaders(DeviceType deviceType, string delimiter = ",")
        {
            var headers = new List<string>();
            foreach (var deviceProp in typeof(Device).GetProperties())
            {
                if (deviceProp.Name == "Fields")
                {
                    // Add all of the DeviceType field names
                    var fieldProps = deviceType.Fields.ToDictionary().Values.Select(field => { return (string)field; });
                    headers.AddRange(fieldProps);
                }
                else if (deviceProp.Name == "Location")
                {
                    // Add all of the DeviceLocation prop names
                    var locProps = typeof(DeviceLocation).GetProperties().Select(prop => prop.Name);
                    headers.AddRange(locProps);
                }
                else if (deviceProp.Name == "LastModified")
                {
                    // Add all of the DeviceLastModified prop names
                    var modProps = typeof(DeviceLastModified).GetProperties().Select(prop => prop.Name);
                    headers.AddRange(modProps);
                }
                else if (deviceProp.Name != "Photo" && deviceProp.Name != "Files")
                {
                    // Add all 'primative' prop names
                    headers.Add(deviceProp.Name);
                }
            }
            return string.Join(delimiter, headers);
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property values in the parameter Device object. The Fields
        /// property values are based on the keys in the parameter deviceType's fields and the values of the parameter device's fields.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceType"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string GenerateDeviceValues(Device device, DeviceType deviceType, string delimiter = ",")
        {
            var values = new List<string>();
            foreach (var deviceProp in typeof(Device).GetProperties())
            {
                if (deviceProp.Name == "Fields")
                {
                    // Add all DeviceType field values contained in the device
                    var deviceTypeFieldKeys = deviceType.Fields.ToDictionary().Keys;
                    foreach (var key in deviceTypeFieldKeys)
                    {
                        string value = "";
                        if (device.Fields != null && device.Fields.ContainsKey(key))
                        {
                            value = device.Fields[key];
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name == "Location")
                {
                    // Add all of the DeviceLocation prop values contained in the device
                    var locProps = typeof(DeviceLocation).GetProperties();
                    foreach (var locProp in locProps)
                    {
                        string value = "";
                        if (device.Location != null)
                        {
                            var propVal = locProp.GetValue(device.Location);
                            value = propVal?.ToString() ?? "";
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name == "LastModified")
                {
                    // Add all of the DeviceLastModified prop values contained in the device
                    var modProps = typeof(DeviceLastModified).GetProperties();
                    foreach (var modProp in modProps)
                    {
                        string value = "";
                        if (device.LastModified != null)
                        {
                            var propVal = modProp.GetValue(device.LastModified);
                            value = propVal?.ToString() ?? "";
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name != "Photo" && deviceProp.Name != "Files")
                {
                    // Add all of the 'primative' prop values contained in the device                    
                    var propVal = deviceProp.GetValue(device);
                    string value = propVal?.ToString() ?? "";
                    values.Add(value);
                }
            }
            return string.Join(delimiter, values);
        }
    }
}
