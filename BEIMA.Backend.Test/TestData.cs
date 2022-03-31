using BEIMA.Backend.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace BEIMA.Backend.Test
{
    public static class TestData
    {
        public const string _testDevice =
            "{" +
                "\"deviceTag\": \"A-2\"," +
                "\"deviceTypeId\": \"12341234abcdabcd43214321\"," +
                "\"fields\":{" +
                    "\"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\": \"TestValue1\"," +
                    "\"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb\": \"TestValue2\"," +
                "}," +
                "\"location\": {" +
                    "\"buildingId\": \"111111111111111111111111\"," +
                    "\"notes\": \"Some notes\"," +
                    "\"latitude\": \"123.456\"," +
                    "\"longitude\": \"101.101\"" +
                "}," +
                "\"manufacturer\": \"Generic Inc.\"," +
                "\"modelNum\": \"1234\"," +
                "\"notes\": \"Some notes.\"," +
                "\"serialNum\": \"abcd1234\"," +
                "\"yearManufactured\": 2010" +
            "}";

        public const string _testUpdateDevice =
            "{" +
                "\"_id\": \"abcdef123456789012345678\"," +
                "\"deviceTag\": \"A-3\"," +
                "\"deviceTypeId\": \"12341234abcdabcd43214321\"," +
                "\"fields\":{" +
                    "\"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\": \"TestValue3\"," +
                    "\"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb\": \"TestValue4\"," +
                "}," +
                "\"location\": {" +
                    "\"buildingId\": \"111111111111111111111111\"," +
                    "\"notes\": \"Some notes.\"," +
                    "\"latitude\": \"123.456\"," +
                    "\"longitude\": \"101.101\"" +
                "}," +
                "\"manufacturer\": \"Generic Inc.\"," +
                "\"modelNum\": \"1234\"," +
                "\"notes\": \"Some notes.\"," +
                "\"serialNum\": \"abcd1234\"," +
                "\"yearManufactured\": 2004" +
            "}";

        public const string _testDeviceType =
            "{" +
                "\"name\": \"Boiler\"," +
                "\"description\": \"Device type for boilers\"," +
                "\"notes\": \"Some notes.\"," +
                "\"fields\": [" +
                    "\"MaxTemperature\"," +
                    "\"MinTemperature\"," +
                    "\"Capacity\"" +
                "]" +
            "}";

        public const string _testUpdateDeviceType =
            "{" +
                "\"name\": \"Boiler\"," +
                "\"description\": \"Device type for boilers\"," +
                "\"notes\": \"Some more notes.\"," +
                "\"fields\": {" +
                    "\"533ba994-352d-497e-a827-ab78314405a8\":\"MaxTemperature\"," +
                    "\"7331073d-95bb-4940-9643-cdbbcdc3fdc1\":\"MinTemperature\"," +
                    "\"8f3f9a48-53d0-492a-98f7-c770a7736aec\":\"BoilerCapacity\"" +
                "}," +
                "\"newFields\": [" +
                    "\"AddedField\"" +
                "]" +
            "}";

        public const string _testBuilding =
            "{" +
                "\"name\": \"Student Union\"," +
                "\"number\": \"1234\"," +
                "\"notes\": \"Some building notes.\"," +
                "\"location\": {" +
                    "\"latitude\": \"12.345\"," +
                    "\"longitude\": \"10.101\"" +
                "}" +
            "}";

        public const string _testUpdateBuilding =
            "{" +
                "\"name\": \"Student Union Building\"," +
                "\"number\": \"1234\"," +
                "\"notes\": \"Some new building notes.\"," +
                "\"location\": {" +
                    "\"latitude\": \"-12.345\"," +
                    "\"longitude\": \"-10.101\"" +
                "}" +
            "}";

        public static readonly string _testAddDeviceNoLocation = GenerateAddDeviceNoLocation();
        public static readonly string _testUpdateDeviceDeleteFiles = GenerateUpdateDeviceRequest();
        public static readonly string _testUpdateDeviceNoLocation = GenerateUpdateDeviceNoLocationRequest();
        public static readonly byte[] _fileBytes = Encoding.ASCII.GetBytes("TestOne");

        private static string GenerateAddDeviceNoLocation()
        {
            var request = new AddDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "12341234abcdabcd43214321",
                Fields = new Dictionary<string, string>
                {
                    { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestValue1"},
                    { "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestValue2"}
                },
                Manufacturer = "man",
                SerialNum = "serial",
                YearManufactured = 1880,
                Notes = "notes",

            };
            return JsonConvert.SerializeObject(request);
        }

        private static string GenerateUpdateDeviceRequest()
        {
            var request = new UpdateDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "12341234abcdabcd43214321",
                Fields = new Dictionary<string, string>
                {
                    { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestValue1"},
                    { "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestValue2"}
                },
                Manufacturer = "man",
                ModelNum = "1234",
                SerialNum = "ser",
                Notes = "Some notes.",
                Location = new DeviceLocation()
                {
                    BuildingId = "622cf00109137c26f913b281",
                    Notes = "notes",
                    Latitude = "1231232",
                    Longitude = "123213213"
                },
                YearManufactured = 1880,
                DeletedFiles = new List<string>(),
            };
            request.DeletedFiles.Add("fileOneUid");
            request.DeletedFiles.Add("fileTwoUid");
            return JsonConvert.SerializeObject(request);
        }

        private static string GenerateUpdateDeviceNoLocationRequest()
        {
            var request = new UpdateDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "12341234abcdabcd43214321",
                Manufacturer = "man",
                ModelNum = "1234",
                SerialNum = "ser",
                Notes = "Some notes.",
                Fields = new Dictionary<string, string>
                {
                    { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "TestValue1"},
                    { "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "TestValue2"}
                },
                YearManufactured = 1880,
                DeletedFiles = new List<string>()
            };
            return JsonConvert.SerializeObject(request);
        }
    }
}
