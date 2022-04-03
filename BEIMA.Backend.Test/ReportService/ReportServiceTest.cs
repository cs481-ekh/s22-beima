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

namespace BEIMA.Backend.Test.ReportServices
{
    [TestFixture]
    public class ReportServiceTest : UnitTestBase
    {
        #region Null Device Types

        [Test]
        public void NullDeviceTypeListAndNullDeviceList_GenerateDeviceTypeReport_NullReturned()
        {
            // ACT
            var bytes = ReportWriter.GenerateDeviceTypeReport(null, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void NullDeviceTypeAndNullDeviceList_GenerateReportByDeviceType_NullReturned()
        {
            // ACT
            var bytes = ReportWriter.GeneratDeviceReportByDeviceType(null, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void NullDeviceTypeListAndNullDeviceList_GenerateAllReport_NullReturned()
        {
            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(null, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void EmptyDeviceTypeListAndNullDeviceList_GenerateAllReport_NullReturned()
        {
            // Arrange
            var deviceTypes = new List<DeviceType>();

            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(deviceTypes, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        #endregion

        #region Valid Device Types Empty/Null Device Lists

        [Test]
        public void DeviceTypeListAndEmptyDeviceList_GenerateDeviceTypeReport_NullReturned()
        {
            // Arrange
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceTypeList = new List<DeviceType>() { deviceTypeOne };
            var deviceList = new List<Device>();

            // ACT
            var bytes = ReportWriter.GenerateDeviceTypeReport(deviceTypeList, deviceList);

            // Assert
            Assert.That(bytes, Is.Null);
        }


        [Test]
        public void EmptyDeviceTypeListAndEmptyDeviceList_GenerateDeviceTypeReport_NullReturned()
        {
            // Arrange
            var deviceTypeList = new List<DeviceType>(); ;
            var deviceList = new List<Device>();

            // ACT
            var bytes = ReportWriter.GenerateDeviceTypeReport(deviceTypeList, deviceList);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void DeviceTypeAndNullDeviceList_GenerateReportByDeviceType_FileContainsOnlyHeader()
        {
            // Arrange
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            // ACT
            var bytes = ReportWriter.GeneratDeviceReportByDeviceType(deviceTypeOne, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void DeviceTypeAndEmptyDeviceList_GenerateReportByDeviceType_FileContainsOnlyHeader()
        {
            // Arrange
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceList = new List<Device>();

            // ACT
            var bytes = ReportWriter.GeneratDeviceReportByDeviceType(deviceTypeOne, deviceList);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void DeviceTypeListAndNullDeviceList_GenerateAllReport_ZipFileContainsFileWithOnlyHeader()
        {
            // Arrange
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceTypeList = new List<DeviceType>() { deviceTypeOne };

            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(deviceTypeList, null);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        [Test]
        public void DeviceTypeListAndEmptyDeviceList_GenerateAllReport_ZipFileContainsFileWithOnlyHeader()
        {
            // Arrange
            var deviceTypeOne = new DeviceType(ObjectId.GenerateNewId(), "Boiler", "This is a boiler", "Boiler type notes");
            deviceTypeOne.SetLastModified(DateTime.UtcNow, "Anonymous");
            deviceTypeOne.AddField("boilerf1", "BoilerField1");
            deviceTypeOne.AddField("boilerf2", "BoilerField2");
            deviceTypeOne.AddField("boilerf3", "BoilerField3");

            var deviceTypeList = new List<DeviceType>() { deviceTypeOne };
            var deviceList = new List<Device>();

            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(deviceTypeList, deviceList);

            // Assert
            Assert.That(bytes, Is.Null);
        }

        #endregion

        #region Valid Device Types Filled Device List

        [Test]
        public void DeviceTypeListAndDeviceList_GenerateDeviceTypeReport_FileContainsHeaderAndDeviceTypeData()
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

            var devices = new List<Device>()
            {
                deviceOne,
                deviceTwo,
                deviceThree
            };

            var deviceTypes = new List<DeviceType>() {
                deviceTypeOne,
                deviceTypeTwo
            };

            // ACT
            var bytes = ReportWriter.GenerateDeviceTypeReport(deviceTypes, devices);

            // Assert
            Assert.That(bytes, Is.Not.Null);

            var content = Encoding.UTF8.GetString(bytes);
            var contentRows = content.Split(Environment.NewLine);
            Assert.That(contentRows.Count, Is.EqualTo(3));

            var headers = new List<List<string>>()
            {
                new List<string>() { "Id", "Name", "Description", "Notes" },
                new List<string>() { "Date", "User" },
                new List<string>() { "DeviceCount"}
            };
            var headerString = CombineColumnValues(headers);
            Assert.That(contentRows[0], Is.EqualTo(headerString));

            var rowOneDeviceList = new List<Device>() { 
                deviceOne, deviceTwo 
            };
            var rowOne = DeviceTypeToColumnValues(deviceTypeOne, rowOneDeviceList);
            var rowOneString = CombineColumnValues(rowOne);
            Assert.That(contentRows[1], Is.EqualTo(rowOneString));

            var rowTwoDeviceList = new List<Device>() {
                deviceThree
            };
            var rowTwo = DeviceTypeToColumnValues(deviceTypeTwo, rowTwoDeviceList);
            var rowTwoString = CombineColumnValues(rowTwo);
            Assert.That(contentRows[2], Is.EqualTo(rowTwoString));
        }


        [Test]
        public void DeviceTypeFilledDeviceList_GenerateReportByDeviceType_FileContainsHeaderAndDeviceData()
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

            var devices = new List<Device>()
            {
                deviceOne,
                deviceTwo
            };            

            // ACT
            var bytes = ReportWriter.GeneratDeviceReportByDeviceType(deviceTypeOne, devices);

            // Assert
            Assert.That(bytes, Is.Not.Null);

            var content = Encoding.UTF8.GetString(bytes);
            var contentRows = content.Split(Environment.NewLine);
            Assert.That(contentRows.Count, Is.EqualTo(3));

            var headers = new List<List<string>>()
            {
                new List<string>() { "Id", "DeviceTypeId", "DeviceTag", "Manufacturer", "ModelNum", "SerialNum", "YearManufactured", "Notes" },
                new List<string>() { "BoilerField1", "BoilerField2", "BoilerField3" },
                new List<string>() { "BuildingId", "Notes", "Latitude", "Longitude" },
                new List<string>() { "Date", "User" }
            };
            var headerString = CombineColumnValues(headers);
            Assert.That(contentRows[0], Is.EqualTo(headerString));

            var rowOne = DeviceToColumnValues(deviceOne);
            var rowOneString = CombineColumnValues(rowOne);
            Assert.That(contentRows[1], Is.EqualTo(rowOneString));

            var rowTwo = DeviceToColumnValues(deviceTwo);
            var rowTwoString = CombineColumnValues(rowTwo);
            Assert.That(contentRows[2], Is.EqualTo(rowTwoString));
        }

        [Test]
        public void DevicesAndDeviceTypesExists_GenerateAllReport_ZipContainsAllDeviceTypesWithDeviceData()
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

            var devices = new List<Device>()
            {
                deviceOne,
                deviceTwo,
                deviceThree
            };

            var deviceTypes = new List<DeviceType>() { 
                deviceTypeOne,
                deviceTypeTwo
            };                        

            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(deviceTypes, devices);

            //ASSERT 
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

                Assert.That(file1Rows.Count, Is.EqualTo(3));
                Assert.That(file2Rows.Count, Is.EqualTo(2));

                // Tests that file 1 has correct data
                var file1Headers = new List<List<string>>()
                {
                    new List<string>() { "Id", "DeviceTypeId", "DeviceTag", "Manufacturer", "ModelNum", "SerialNum", "YearManufactured", "Notes" },
                    new List<string>() { "BoilerField1", "BoilerField2", "BoilerField3" },
                    new List<string>() { "BuildingId", "Notes", "Latitude", "Longitude" },
                    new List<string>() { "Date", "User" }
                };
                var file1HeaderString = CombineColumnValues(file1Headers);
                Assert.That(file1Rows[0], Is.EqualTo(file1HeaderString));

                var file1RowOne = DeviceToColumnValues(deviceOne);
                var file1RowOneString = CombineColumnValues(file1RowOne);
                Assert.That(file1Rows[1], Is.EqualTo(file1RowOneString));

                var file1RowTwo = DeviceToColumnValues(deviceTwo);
                var file1RowTwoString = CombineColumnValues(file1RowTwo);
                Assert.That(file1Rows[2], Is.EqualTo(file1RowTwoString));

                // Tests that file 2 has correct data
                var file2Headers = new List<List<string>>()
                {
                    new List<string>() { "Id", "DeviceTypeId", "DeviceTag", "Manufacturer", "ModelNum", "SerialNum", "YearManufactured", "Notes" },
                    new List<string>() { "HvacField1", "HvacFeild2" },
                    new List<string>() { "BuildingId", "Notes", "Latitude", "Longitude" },
                    new List<string>() { "Date", "User" }
                };
                var file2HeaderString = CombineColumnValues(file2Headers);
                Assert.That(file2Rows[0], Is.EqualTo(file2HeaderString));

                var file2RowOne = DeviceToColumnValues(deviceThree);
                var file2RowOneString = CombineColumnValues(file2RowOne);
                Assert.That(file2Rows[1], Is.EqualTo(file2RowOneString));
            }
        }

        [Test]
        public void DevicesAndDeviceTypesExists_OnlyOneDeviceTypeHasDevices_GenerateAllReport_ZipContainsOneDeviceTypeWithDeviceData()
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

            var devices = new List<Device>()
            {
                deviceOne,
                deviceTwo,
            };

            var deviceTypes = new List<DeviceType>() {
                deviceTypeOne,
                deviceTypeTwo
            };

            // ACT
            var bytes = ReportWriter.GenerateAllDeviceReports(deviceTypes, devices);

            //ASSERT 
            using (var memStream = new MemoryStream(bytes))
            {
                var zipFile = new ZipArchive(memStream);

                // Test that the zip has the right amount of files and that they have the right names
                Assert.That(zipFile.Entries.Count, Is.EqualTo(1));

                var file1 = zipFile.Entries[0];

                Assert.That(file1.Name, Is.EqualTo($"{deviceTypeOne.Name}.csv"));

                // Read all the files and validate their content
                string file1Text;
                using (var readerOne = new StreamReader(file1.Open()))
                {
                    file1Text = readerOne.ReadToEnd();
                }

                // Test that there is a header row and a row for every device
                var file1Rows = file1Text.Split(Environment.NewLine);

                Assert.That(file1Rows.Count, Is.EqualTo(3));

                // Tests that file 1 has correct data
                var file1Headers = new List<List<string>>()
                {
                    new List<string>() { "Id", "DeviceTypeId", "DeviceTag", "Manufacturer", "ModelNum", "SerialNum", "YearManufactured", "Notes" },
                    new List<string>() { "BoilerField1", "BoilerField2", "BoilerField3" },
                    new List<string>() { "BuildingId", "Notes", "Latitude", "Longitude" },
                    new List<string>() { "Date", "User" }
                };
                var file1HeaderString = CombineColumnValues(file1Headers);
                Assert.That(file1Rows[0], Is.EqualTo(file1HeaderString));

                var file1RowOne = DeviceToColumnValues(deviceOne);
                var file1RowOneString = CombineColumnValues(file1RowOne);
                Assert.That(file1Rows[1], Is.EqualTo(file1RowOneString));

                var file1RowTwo = DeviceToColumnValues(deviceTwo);
                var file1RowTwoString = CombineColumnValues(file1RowTwo);
                Assert.That(file1Rows[2], Is.EqualTo(file1RowTwoString));
            }
        }

        #endregion

        /// <summary>
        /// Iterates through each list in columnLists and produces a string containing
        /// all of those values seperated by a ','
        /// </summary>
        /// <param name="columnLists"></param>
        /// <returns>String containing all string values in columnlists joined together by a ','</returns>
        private static string CombineColumnValues(List<List<string>> columnLists)
        {
            var columns = new List<string>();
            foreach (var columnList in columnLists)
            {
                foreach(var column in columnList)
                {
                    columns.Add(column);
                }
            }
            return string.Join(",", columns);
        }

        /// <summary>
        /// Takes a devicetype and generates a list of lists based off of its properties
        /// in addition to the count of the passed in devices list.
        /// Is used in conjunction with the CombineColumnValues in order to test if rows
        /// created by the ReportWRtier have the correct data in the correct order.
        /// </summary>
        /// <param name="deviceType">Devices type whos values are being scraped</param>
        /// <param name="devices">List of devices associated with the device type</param>
        /// <returns>List of lists containing devicetype property values</returns>
        private static List<List<string>> DeviceTypeToColumnValues(DeviceType deviceType, List<Device> devices)
        {
            var columns = new List<List<string>>()
            {
                new List<string>()
                {
                    deviceType.Id.ToString(),
                    deviceType.Name,
                    deviceType.Description,
                    deviceType.Notes
                },
                new List<string>()
                {
                    deviceType.LastModified["date"].AsBsonDateTime.ToString(),
                    deviceType.LastModified["user"].AsString
                },
                new List<string>(){
                    devices.Count.ToString(),
                }
            };
            return columns;
        }

        /// <summary>
        /// Takes a device and generates a list of lists based off of its properties.
        /// Is used in conjunction with CombineColumnValues in order to test if rows
        /// created by the ReportWriter have the correct data in the correct order.
        /// </summary>
        /// <param name="device">Devices whose values are being scraped</param>
        /// <returns>List of lists containing device propety values</returns>
        private static List<List<string>> DeviceToColumnValues(Device device)
        {
            var columns = new List<List<string>>()
            {
                new List<string>() { 
                    device.Id.ToString(), 
                    device.DeviceTypeId.ToString(), 
                    device.DeviceTag, device.Manufacturer, 
                    device.ModelNum, 
                    device.SerialNum, 
                    device.YearManufactured.ToString()?? "", 
                    device.Notes
                },
                device.Fields.ToArray().OrderBy(val => val.Key).Select(val => val.Value).ToList(),
                new List<string>() { 
                    device.Location?.BuildingId.ToString() ?? "", 
                    device.Location?.Notes ?? "", 
                    device.Location?.Latitude ?? "",
                    device.Location?.Longitude ?? ""
                },
                new List<string>()
                {
                    device.LastModified?.Date.ToString() ?? "",
                    device.LastModified?.User ?? "",
                }
            };
            return columns;
        }
    }
}
