namespace BEIMA.Backend.Test
{
    public static class TestData
    {
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

    }
}
