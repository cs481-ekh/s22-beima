using Newtonsoft.Json;

namespace BEIMA.Backend.FT
{
    public class TestObjects
    {
        public class Device
        {
            [JsonProperty(PropertyName = "_id")]
            public string? Id { get; set; }

            [JsonProperty(PropertyName = "deviceTag")]
            public string? DeviceTag { get; set; }

            [JsonProperty(PropertyName = "deviceTypeId")]
            public string? DeviceTypeId { get; set; }

            [JsonProperty(PropertyName = "location")]
            public Location? Location { get; set; }

            [JsonProperty(PropertyName = "manufacturer")]
            public string? Manufacturer { get; set; }

            [JsonProperty(PropertyName = "modelNum")]
            public string? ModelNum { get; set; }

            [JsonProperty(PropertyName = "notes")]
            public string? Notes { get; set; }

            [JsonProperty(PropertyName = "serialNum")]
            public string? SerialNum { get; set; }

            [JsonProperty(PropertyName = "yearManufactured")]
            public int? YearManufactured { get; set; }
        }

        public class Location
        {
            [JsonProperty(PropertyName = "buildingId")]
            public string? BuildingId { get; set; }

            [JsonProperty(PropertyName = "notes")]
            public string? Notes { get; set; }

            [JsonProperty(PropertyName = "longitude")]
            public string? Longitude { get; set; }

            [JsonProperty(PropertyName = "latitude")]
            public string? Latitude { get; set; }

        }

        public class DeviceType
        {
            // TODO: Add device type testing object
        }
    }
}
