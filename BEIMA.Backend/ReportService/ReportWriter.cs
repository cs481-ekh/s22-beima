using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using CsvHelper;
using CsvHelper.Configuration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace BEIMA.Backend.ReportService
{
    /// <summary>
    /// This class abstracts basic report generation operations.
    /// </summary>
    public static class ReportWriter
    {
        /// <summary>
        /// Generates a byte[] containing a file object. The
        /// file contains a header based on the the props of the deviceType object
        /// and then the data of all devicesTypes as well as a count of the number of
        /// devices that are associated with a device type.
        /// </summary>
        /// <param name="deviceTypes">List of all device types in the database</param>
        /// <param name="devices">List of all devices in the database</param>
        /// <param name="delimiter">String used to seperate every value in the report</param>
        /// <returns>Byte[] containing a file filled with devicetype data and the number of devices associated with that devicetype</returns>
        public static byte[] GenerateDeviceTypeReport(List<DeviceType> deviceTypes, List<Device> devices, string delimiter = ",")
        {
            if (deviceTypes == null || deviceTypes.Count == 0)
            {
                return null;
            }

            // Create dict to group devices based on device type id
            var typeToDevices = new Dictionary<ObjectId, List<Device>>();
            foreach (var deviceType in deviceTypes)
            {
                typeToDevices.Add(deviceType.Id, new List<Device>());
            }

            // Get all devices and add them to the dict based on deviceTypeId
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    // Only add a device if it's deviceType exists in the deviceTypes list. Should always be the case
                    if (typeToDevices.ContainsKey(device.DeviceTypeId))
                    {
                        typeToDevices[device.DeviceTypeId].Add(device);
                    }
                }
            }

            byte[] fileBytes;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))                                  // Create writer to write device/device type data to memory stream
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))                    // Create csv writer to handle escaping characters
            {
                // Write headers
                var headers = GenerateDeviceTypeReportHeaders();
                headers.ForEach(val =>
                {
                    csv.WriteField(val);
                });
                csv.NextRecord(); ;

                // Write all deviceType data                            
                for (var i = 0; i < deviceTypes.Count; i++)
                {
                    var deviceType = deviceTypes[i];
                    var deviceTypeDevices = typeToDevices[deviceType.Id];

                    // Write device's values
                    var values = GenerateDeviceTypeReportValues(deviceType, deviceTypeDevices);
                    values.ForEach(val =>
                    {
                        csv.WriteField(val);
                    });
                    csv.NextRecord();
                }
                // Flush writer and copy data to byte[]
                csv.Flush();
                fileBytes = stream.ToArray();
            }
            return fileBytes;
        }

        /// <summary>
        /// Generates a byte[] containing a file object. The
        /// file contains a header based on the paramter deviceType
        /// and then the data of all devices related to the specific deviceTypeId.
        /// </summary>
        /// <param name="deviceType">DeviceType that the report should be based on</param>
        /// <param name="devices">List of all devices assoicated with deviceType</param>
        /// <param name="buildings">List of all buildings in db</param>
        /// <param name="delimiter">String used to seperate every value in the report</param>
        /// <returns>Byte[] containing a file filled with device data associated with the passed in deviceTypeId</returns>
        public static byte[] GeneratDeviceReportByDeviceType(DeviceType deviceType, List<Device> devices, List<Building> buildings, string delimiter = ",")
        {
            // Return if deviceType is null
            if (deviceType == null)
            {
                return null;
            }

            if (devices == null || devices.Count == 0)
            {
                return null;
            }

            if (buildings == null)
            {
                buildings = new List<Building>();
            }


            byte[] fileBytes;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))                                  // Create writer to write device/device type data to memory stream
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))                    // Create csv writer to handle escaping characters
            {
                // Create object containing all props and fields that need to be printed
                var fieldsKeys = deviceType.Fields.Elements.Select(element => element.Name).ToList();

                // Write headers
                var headers = GenerateDeviceReportHeaders(deviceType);
                headers.ForEach(val =>
                {
                    csv.WriteField(val);
                });
                csv.NextRecord();

                // Write all device data                            
                for (var i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    // Ensure that device doesn't print if it doesn't match deviceType Id. Should not happen.
                    if (device.DeviceTypeId != deviceType.Id)
                    {
                        continue;
                    }

                    // Write device's values
                    var values = GenerateDeviceValues(device, deviceType, buildings);
                    values.ForEach(val =>
                    {
                        csv.WriteField(val);
                    });
                    csv.NextRecord();
                }
                // Flush writer and copy data to byte[]
                csv.Flush();
                fileBytes = stream.ToArray();
            }
            return fileBytes;
        }

        /// <summary>
        /// Generates a byte[] containing a zip file with a file entry
        /// per device type. Each of the file entries will contains
        /// a header and then the data of all devices related to that specific
        /// device type. If there are no device types returns then null is returned.
        /// A file entry may contain only the header row if no device is associated
        /// with that specific device type
        /// </summary>
        /// <param name="deviceTypes">List of all device types in the database</param>
        /// <param name="devices">List of all devices in the database</param>
        /// <param name="buildings">List of all buildings in db</param>
        /// <param name="delimiter">String used to seperate every value in the report</param> 
        /// <returns>Byte[] containing a zipfile of device type files filled with associated devices</returns>
        public static byte[] GenerateAllDeviceReports(List<DeviceType> deviceTypes, List<Device> devices, List<Building> buildings, string delimiter = ",")
        {
            // Return if no devicesTypes
            if (deviceTypes == null || deviceTypes.Count == 0)
            {
                return null;
            }
            // Ensure devices is not null;
            if (devices == null)
            {
                return null;
            }

            // Create dict to group devices based on device type id
            var typeToDevices = new Dictionary<ObjectId, List<Device>>();
            foreach (var deviceType in deviceTypes)
            {
                typeToDevices.Add(deviceType.Id, new List<Device>());
            }

            // Get all devices and add them to the dict based on deviceTypeId            
            foreach (var device in devices)
            {
                // Only add a device if it's deviceType exists in the deviceTypes list. Should always be the case
                if (typeToDevices.ContainsKey(device.DeviceTypeId))
                {
                    typeToDevices[device.DeviceTypeId].Add(device);
                }
            }

            // Remove any deviceTypes that do not have any devices
            typeToDevices = typeToDevices.Where(item => item.Value.Count > 0).ToDictionary(item => item.Key, item => item.Value);
            if (typeToDevices.Keys.Count == 0)
            {
                return null;
            }

            byte[] zipFileBytes;
            using (var outStream = new MemoryStream()) // Create memory stream to convert zip file stream to byte[]
            {
                using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create, true)) // Create zip file stream that files can be streamed into
                {
                    foreach (var deviceType in deviceTypes)
                    {
                        // Device type has no devices associated with it
                        if (typeToDevices.ContainsKey(deviceType.Id) == false)
                        {
                            continue;
                        }

                        var deviceTypeDevices = typeToDevices[deviceType.Id];

                        var entryName = deviceType.Name.Trim().Replace("\"", "").Replace(" ","_"); //Remove whitespace and quotes as it can lead issues with openning archive
                        var shortenedName = entryName.Length > 10 ? entryName.Substring(0, 10) : entryName; //Shorten file name

                        var fileInZip = zipStream.CreateEntry($"{shortenedName}.csv", CompressionLevel.Optimal); // Create new zip file entry that can be streamed into
                        using (var writer = new StreamWriter(fileInZip.Open()))                                  // Create writer to write device/device type data to memory stream
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))                    // Create csv writer to handle escaping characters
                        {
                            // Write file headers                            
                            var headers = GenerateDeviceReportHeaders(deviceType);
                            headers.ForEach(val =>
                            {
                                csv.WriteField(val);
                            });
                            csv.NextRecord();

                            // Write all device data                            
                            for (var i = 0; i < deviceTypeDevices.Count; i++)
                            {
                                // Write device's values
                                var device = deviceTypeDevices[i];
                                var values = GenerateDeviceValues(device, deviceType, buildings);
                                values.ForEach(val =>
                                {
                                    csv.WriteField(val);
                                });
                                csv.NextRecord();
                            }
                        }
                    }
                }
                zipFileBytes = outStream.ToArray();
            }
            return zipFileBytes;
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property names in the DeviceType object
        /// excluding the Fields properties. In addition another header is included for the count
        /// of devices associated with a device type
        /// </summary>
        /// <returns>List of headers for a device type object</returns>
        private static List<string> GenerateDeviceTypeReportHeaders()
        {
            var headers = new List<string>();

            // Add all of the general prop names associated with the Device object
            var generalProps = DeviceTypeReportProps.GeneralProps.Select(p => p.Name);
            headers.AddRange(generalProps);

            // Add all of the last modified values contained in the deviceType
            var lastModifiedProps = DeviceTypeReportProps.LastModifiedProps.OrderBy(val => val.Key).Select(val => val.Value);
            headers.AddRange(lastModifiedProps);

            // Add header for count of all devices associated with the device type
            headers.Add("DeviceCount");

            return headers;
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property values in a DeviceType object. In addition
        /// another value is added for the count of the number of devices associated with the DeviceType.
        /// </summary>
        /// <param name="deviceType">DeviceType that the values should be based on</param>
        /// <param name="associatedDevices">List of devices associated with the DeviceType</param>
        /// <returns>List of device type property values.</returns>
        private static List<string> GenerateDeviceTypeReportValues(DeviceType deviceType, List<Device> associatedDevices)
        {
            var values = new List<string>();

            // Add all of the general prop values contained in the deviceType
            foreach (var prop in DeviceTypeReportProps.GeneralProps)
            {
                var propVal = prop.GetValue(deviceType);
                string value = propVal != null ? propVal.ToString() : "";
                values.Add(value);
            }

            // Add all of the last modified values contained in the deviceType
            var sortedLastModifiedKeys = DeviceTypeReportProps.LastModifiedProps.OrderBy(val => val.Key).Select(val => val.Key);
            foreach (var key in sortedLastModifiedKeys)
            {
                var value = "";
                if (deviceType.LastModified != null && deviceType.LastModified.Contains(key))
                {
                    value = deviceType.LastModified[key].ToString();
                }
                values.Add(value);
            }

            // Add a count of the number of devices associated with the devices
            var count = (associatedDevices?.Count ?? 0).ToString();
            values.Add(count);

            return values;
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property names in the Device object. The Fields
        /// property is replaced with the field names contained in the parameter deviceType's Fields dictionary
        /// </summary>
        /// <param name="deviceType">DeviceType that the headers should be based on</param>
        /// <returns>List of headers for a device object</returns>
        private static List<string> GenerateDeviceReportHeaders(DeviceType deviceType)
        {
            var headers = new List<string>();

            // Add all of the general prop names associated with the Device object
            var generalProps = DeviceReportProps.GeneralProps.Select(p => p.Name).Select(name => name == "DeviceTypeId" ? "DeviceTypeName" : name);
            headers.AddRange(generalProps);

            // Create a list of the device types's field values ordered by the field keys
            var sortedFieldsValues = deviceType.Fields.OrderBy(f => f.Name).Select(f => f.Value.ToString());
            headers.AddRange(sortedFieldsValues);

            // Add all of the location prop names associated with the DeviceLocation object
            var locationProps = DeviceReportProps.LocationProps.Select(p => p.Name).Select(name => name == "BuildingId" ? "BuildingName" : name);
            headers.AddRange(locationProps);

            // Add all of the last modified prop names associated with the DeviceLastModified object
            var lastModifiedProps = DeviceReportProps.LastModifiedProps.Select(p => p.Name);
            headers.AddRange(lastModifiedProps);

            return headers;
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property values in the parameter Device object. The Fields
        /// property values are based on the keys in the parameter deviceType's fields and the values of the parameter device's fields.
        /// </summary>
        /// <param name="device">Device that's data values should be used</param>
        /// <param name="deviceType">DeviceType that the field keys should be based on</param>
        /// <returns>List of device property values.</returns>
        private static List<string> GenerateDeviceValues(Device device, DeviceType deviceType, List<Building> buildings)
        {
            var values = new List<string>();

            // Add all of the general prop values contained in the device
            foreach (var prop in DeviceReportProps.GeneralProps)
            {
                var propVal = prop.GetValue(device);
                string value = propVal != null ? propVal.ToString() : "";
                if (prop.Name == "DeviceTypeId" && propVal != null)
                {
                    value = deviceType.Name;
                }
                values.Add(value);
            }

            // Create a sorted list of the device types's field keys ordered by the field keys
            var sortedFieldKeys = deviceType.Fields.OrderBy(f => f.Name).Select(f => f.Name);
            foreach (var key in sortedFieldKeys)
            {
                // Add all of the field values in the device based on the deviceType
                string value = "";
                if (device.Fields != null && device.Fields.ContainsKey(key))
                {
                    value = device.Fields[key];
                }
                values.Add(value);
            }

            // Add all of the location values contained in the device
            foreach (var prop in DeviceReportProps.LocationProps)
            {
                string value = "";
                if (device.Location != null)
                {
                    var propval = prop.GetValue(device.Location);

                    if (propval != null && !String.IsNullOrEmpty(propval.ToString()))
                    {
                        if (prop.Name == "BuildingId")
                        {
                            try
                            {
                                var id = ObjectId.Parse(propval.ToString());
                                value = buildings.Where(b => b.Id == id).Single().Name;
                            }
                            catch (Exception)
                            {
                                value = "";
                            }

                        }
                        else
                        {
                            value = propval.ToString();
                        }
                    }
                }
                values.Add(value);
            }

            // Add all of the last modified values contained in the device
            foreach (var prop in DeviceReportProps.LastModifiedProps)
            {
                string value = "";
                if (device.LastModified != null)
                {
                    var propval = prop.GetValue(device.LastModified);
                    value = propval != null ? propval.ToString() : "";
                }
                values.Add(value);
            }

            return values;
        }
    }
}
