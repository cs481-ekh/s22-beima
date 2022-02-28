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

    }
}
