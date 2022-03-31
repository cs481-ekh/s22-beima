﻿using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
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
        /// file contains a header based on the paramter deviceType
        /// and then the data of all devices related to the specific deviceTypeId.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="devices"></param>
        /// <param name="delimiter"></param>
        /// <returns>Byte[] containing a file filled with device data associated with the passed in deviceTypeId</returns>
        public static byte[] GeneratDeviceReportByDeviceType(DeviceType deviceType, List<Device> devices, string delimiter = ",")
        {
            // Return if deviceType is null
            if(deviceType == null)
            {
                return null;
            }
            // Ensure devices is not null;
            if(devices == null)
            {
                devices = new List<Device>();
            }

            byte[] fileBytes;
            using(var stream = new MemoryStream())
            using (var csvWriter = new StreamWriter(stream))
            {
                // Create object containing all props and fields that need to be printed
                var fieldsKeys = deviceType.Fields.Elements.Select(element => element.Name).ToList();

                // Write headers
                var headers = GenerateDeviceReportHeaders(deviceType);
                var headerString = string.Join(delimiter, headers);
                if (devices.Count > 0)
                {
                    headerString += Environment.NewLine;
                }
                csvWriter.Write(headerString);

                // Write all device data                            
                for (var i = 0; i < devices.Count; i++)
                {                                        
                    var device = devices[i];
                    // Ensure that device doesn't print if it doesn't match deviceType Id. Should not happen.
                    if(device.DeviceTypeId != deviceType.Id)
                    {
                        continue;
                    }

                    // Write device's values
                    var values = GenerateDeviceValues(device, deviceType);
                    var valueString = string.Join(delimiter, values);
                    if (i != devices.Count - 1)
                    {
                        valueString += Environment.NewLine;
                    }
                    csvWriter.Write(valueString);
                }
                // Flush writer and copy data to byte[]
                csvWriter.Flush();
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
        /// <param name="deviceTypes"></param>
        /// <param name="devices"></param>
        /// <param name="delimiter"></param> 
        /// <returns>Byte[] containing a zipfile of device type files filled with associated devices</returns>
        public static byte[] GenerateAllDeviceReports(List<DeviceType> deviceTypes, List<Device> devices, string delimiter = ",")
        {
            // Return if no devicesTypes
            if (deviceTypes == null || deviceTypes.Count == 0)
            {
                return null;
            }
            // Ensure devices is not null;
            if (devices == null)
            {
                devices = new List<Device>();
            }

            // Create dict to group devices based on device type id
            var typeToDevices = new Dictionary<ObjectId, List<Device>>();
            foreach(var deviceType in deviceTypes)
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

            byte[] zipFileBytes;
            using (var outStream = new MemoryStream()) // Create memory stream to convert zip file stream to byte[]
            {
                using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create, true)) // Create zip file stream that files can be streamed into
                {
                    foreach (var deviceType in deviceTypes)
                    {
                        var fileInZip = zipStream.CreateEntry($"{deviceType.Name}.csv", CompressionLevel.Optimal); // Create new zip file entry that can be streamed into
                        using (var fileInZipStream = fileInZip.Open())                                             // Open zip file entry's stream
                        using (var dataStream = new MemoryStream())                                                // Create memory stream that data can be written into
                        using (var csvWriter = new StreamWriter(dataStream))                                       // Create writer to write device/device type data to memory stream
                        {
                            var deviceTypeDevices = typeToDevices[deviceType.Id];

                            // Write file headers                            
                            var headers = GenerateDeviceReportHeaders(deviceType);
                            var headerString = string.Join(delimiter, headers);
                            if (deviceTypeDevices.Count > 0)
                            {
                                headerString += Environment.NewLine;
                            }
                            csvWriter.Write(headerString);

                            // Write all device data                            
                            for (var i = 0; i < deviceTypeDevices.Count; i++)
                            {
                                // Write device's values
                                var device = deviceTypeDevices[i];
                                var values = GenerateDeviceValues(device, deviceType);
                                var valueString = string.Join(delimiter, values);
                                if(i != deviceTypeDevices.Count - 1)
                                {
                                    valueString += Environment.NewLine;
                                }
                                csvWriter.Write(valueString);
                            }
                            // Flush writer and copy data in datastream to zip file entry's stream
                            csvWriter.Flush();
                            dataStream.Position = 0;
                            dataStream.CopyTo(fileInZipStream);
                        }
                    }
                }
                zipFileBytes = outStream.ToArray();
            }
            return zipFileBytes;
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property names in the Device object. The Fields
        /// property is replaced with the field names contained in the parameter deviceType's Fields dictionary
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns>List of headers for a device object</returns>
        private static List<string> GenerateDeviceReportHeaders(DeviceType deviceType)
        {
            var headers = new List<string>();

            // Add all of the general prop names associated with the Device object
            var generalProps = DeviceReportProps.GeneralProps.Select(p => p.Name);
            headers.AddRange(generalProps);

            // Create a list of the device types's field values ordered by the field keys
            var sortedFieldsValues = deviceType.Fields.OrderBy(f => f.Name).Select(f => f.Value.AsString);
            headers.AddRange(sortedFieldsValues);

            // Add all of the location prop names associated with the DeviceLocation object
            var locationProps = DeviceReportProps.LocationProps.Select(p => p.Name);
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
        /// <param name="device"></param>
        /// <param name="deviceType"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static List<string> GenerateDeviceValues(Device device, DeviceType deviceType)
        {
            var values = new List<string>();

            // Add all of the general prop values contained in the device
            foreach (var prop in DeviceReportProps.GeneralProps)
            {
                var propVal = prop.GetValue(device);
                string value = propVal != null ? propVal.ToString() : "";
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
            foreach(var prop in DeviceReportProps.LocationProps)
            {
                string value = "";
                if (device.Location != null)
                {
                    var propval = prop.GetValue(device.Location);
                    value = propval != null ? propval.ToString() : "";
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
