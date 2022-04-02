using BEIMA.Backend.MongoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.Models
{
    /// <summary>
    /// The DeviceReportProps is a static class that represents all of the props associated with a device except for the
    /// Files, Photo, and Field props. It is used by the report writer to ensure that header and device data values
    /// are written in the same order.
    /// </summary>
    public static class DeviceReportProps
    {
        private static readonly string[] IgnoredPropNames = { "Fields", "Location", "LastModified", "Files", "Photo" };
        public static List<PropertyInfo> GeneralProps { get; } = typeof(Device).GetProperties().Where(prop => !IgnoredPropNames.Contains(prop.Name)).ToList();
        public static List<PropertyInfo> LocationProps { get; } = typeof(MongoService.DeviceLocation).GetProperties().ToList(); // Uses mongoservice namespace as DeviceLocation exists in Requests.cs as a different model.
        public static List<PropertyInfo> LastModifiedProps { get; } = typeof(DeviceLastModified).GetProperties().ToList();
    }
}
