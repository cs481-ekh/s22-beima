using System.Collections.Generic;

namespace BEIMA.Backend.Models
{
    /// <summary>
    /// Object representation of the data object in an add device request
    /// </summary>
    public class AddDeviceRequest
    {
        public string DeviceTag { get; set; }
        public string DeviceTypeId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNum { get; set; }
        public string SerialNum { get; set; }
        public int YearManufactured { get; set; }
        public string Notes { get; set; }
        public Location Location { get; set; } = new Location();
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Object representation of the data object in an updated device request
    /// </summary>
    public class UpdateDeviceRequest
    {
        public string DeviceTag { get; set; }
        public string DeviceTypeId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNum { get; set; }
        public string SerialNum { get; set; }
        public int YearManufactured { get; set; }
        public string Notes { get; set; }
        public Location Location { get; set; } = new Location();
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
        public List<string> DeletedFiles { get; set; } = new List<string>();
    }

    /// <summary>
    /// Location Object representation of location field in an add device request
    /// </summary>
    public class Location
    {
        public string BuildingId { get; set; }
        public string Notes { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}