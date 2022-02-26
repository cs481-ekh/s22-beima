using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

            [JsonProperty(PropertyName = "fields")]
            public Dictionary<string, object>? Fields;

            [JsonProperty(PropertyName = "lastModified")]
            public LastModified? LastModified { get; set; }

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

        public class LastModified
        {
            [JsonProperty(PropertyName = "date")]
            public DateTime? Date { get; set; }

            [JsonProperty(PropertyName = "user")]
            public string? User { get; set; }
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
