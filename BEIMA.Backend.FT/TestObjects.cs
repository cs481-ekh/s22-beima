using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BEIMA.Backend.FT
{
    public class TestObjects
    {
        public static readonly byte[] _fileBytes = Encoding.ASCII.GetBytes("TestOne");
        public class Device
        {
            [JsonProperty(PropertyName = "_id")]
            public string? Id { get; set; }

            [JsonProperty(PropertyName = "deviceTag")]
            public string? DeviceTag { get; set; }

            [JsonProperty(PropertyName = "deviceTypeId")]
            public string? DeviceTypeId { get; set; }

            [JsonProperty(PropertyName = "fields")]
            public Dictionary<string, string>? Fields;

            [JsonProperty(PropertyName = "lastModified")]
            public LastModified? LastModified { get; set; }

            [JsonProperty(PropertyName = "location")]
            public DeviceLocation? Location { get; set; }

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

            [JsonProperty(PropertyName = "files")]
            public List<DeviceFile>? Files { get; set; }

            [JsonProperty(PropertyName = "photo")]
            public DeviceFile? Photo { get; set; }
        }

        public class DeviceFile
        {
            [JsonProperty(PropertyName = "fileName")]
            public string? FileName { get; set; }
            [JsonProperty(PropertyName = "fileUid")]
            public string? FileUid { get; set; }
            [JsonProperty(PropertyName = "fileUrl")]
            public string? FileUrl { get; set; }
        }

        public class LastModified
        {
            [JsonProperty(PropertyName = "date")]
            public DateTime? Date { get; set; }

            [JsonProperty(PropertyName = "user")]
            public string? User { get; set; }
        }

        public class DeviceLocation : Location
        {
            [JsonProperty(PropertyName = "buildingId")]
            public string? BuildingId { get; set; }

            [JsonProperty(PropertyName = "notes")]
            public string? Notes { get; set; }
        }

        public abstract class DeviceTypeBase
        {
            [JsonProperty(PropertyName = "_id")]
            public string? Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string? Name { get; set; }

            [JsonProperty(PropertyName = "description")]
            public string? Description { get; set; }

            [JsonProperty(PropertyName = "notes")]
            public string? Notes { get; set; }
        }

        public class DeviceUpdate : Device
        {
            [JsonProperty(PropertyName = "deletedFiles")]
            public List<string>? DeletedFiles { get; set; }
        }
        public class DeviceTypeAdd : DeviceTypeBase
        {
            [JsonProperty(PropertyName = "fields")]
            public List<string>? Fields { get; set; }
        }

        public class DeviceTypeUpdate : DeviceTypeBase
        {
            [JsonProperty(PropertyName = "fields")]
            public Dictionary<string, string>? Fields { get; set; }

            [JsonProperty(PropertyName = "newFields")]
            public List<string>? NewFields { get; set; }
        }

        public class DeviceType : DeviceTypeBase
        {
            [JsonProperty(PropertyName = "lastModified")]
            public LastModified? LastModified { get; set; }

            [JsonProperty(PropertyName = "fields")]
            public Dictionary<string, string>? Fields { get; set; }
        }

        public class Building
        {
            [JsonProperty(PropertyName = "_id")]
            public string? Id { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string? Name { get; set; }

            [JsonProperty(PropertyName = "number")]
            public string? Number { get; set; }

            [JsonProperty(PropertyName = "notes")]
            public string? Notes { get; set; }

            [JsonProperty(PropertyName = "location")]
            public Location? Location { get; set; }

            [JsonProperty(PropertyName = "lastModified")]
            public LastModified? LastModified { get; set; }
        }

        public class User
        {
            [JsonProperty(PropertyName = "_id")]
            public string? Id { get; set; }

            [JsonProperty(PropertyName = "username")]
            public string? Username { get; set; }

            [JsonProperty(PropertyName = "password")]
            public string? Password { get; set; }

            [JsonProperty(PropertyName = "firstName")]
            public string? FirstName { get; set; }

            [JsonProperty(PropertyName = "lastName")]
            public string? LastName { get; set; }

            [JsonProperty(PropertyName = "role")]
            public string? Role { get; set; }
        }

        public class Location
        {
            [JsonProperty(PropertyName = "latitude")]
            public string? Latitude { get; set; }

            [JsonProperty(PropertyName = "longitude")]
            public string? Longitude { get; set; }
        }
    }
}
