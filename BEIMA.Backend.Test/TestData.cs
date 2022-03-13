using BEIMA.Backend.Models;
using System.Collections.Generic;

namespace BEIMA.Backend.Test
{
    public static class TestData
    {
        public readonly static AddDeviceRequest _testAddDeviceRequest = CreateAddRequest();
        public readonly static UpdateDeviceRequest _testUpdateDeviceRequest = CreateUpdateRequest();

        public const string _testDevice =
            "{" +
                "\"deviceTag\": \"A-2\"," +
                "\"deviceTypeId\": \"12341234abcdabcd43214321\"," +
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


        private static AddDeviceRequest CreateAddRequest()
        {
            var request = new AddDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "622cf00109137c26f913b282",
                Manufacturer = "man",
                ModelNum = "mod",
                SerialNum = "ser",
                Notes = "notes",
                Location = new Location()
                {
                    BuildingId = "622cf00109137c26f913b281",
                    Notes = "notes",
                    Latitude = "1231232",
                    Longitude = "123213213"
                },
                Fields = new Dictionary<string, string>()
            };
            request.Fields.Add("customIdOne", "valueOne");
            request.Fields.Add("customIdTwo", "valueTwo");
            return request;
        }

        private static UpdateDeviceRequest CreateUpdateRequest()
        {
            var request = new UpdateDeviceRequest()
            {
                DeviceTag = "tag",
                DeviceTypeId = "622cf00109137c26f913b282",
                Manufacturer = "man",
                ModelNum = "mod",
                SerialNum = "ser",
                Notes = "notes",
                Location = new Location()
                {
                    BuildingId = "622cf00109137c26f913b281",
                    Notes = "notes",
                    Latitude = "1231232",
                    Longitude = "123213213"
                },
                Fields = new Dictionary<string, string>(),
                DeletedFiles = new List<string>()
            };
            request.Fields.Add("customIdOne", "valueOne");
            request.Fields.Add("customIdTwo", "valueTwo");
            request.DeletedFiles.Add("fileOneUid");
            request.DeletedFiles.Add("fileTwoUid");
            return request;
        }
    }
}
