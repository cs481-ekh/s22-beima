using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.ReportService
{
    public class ReportWriter : IReportService
    {
        private static readonly Lazy<ReportWriter> instance = new(() => new ReportWriter());
        private readonly IMongoConnector db;

        private ReportWriter()
        {
            db = MongoDefinition.MongoInstance;
        }

        public static ReportWriter Instance { get { return instance.Value; } }

        public byte[] GeneratDeviceReportByDeviceType(ObjectId deviceType)
        {

            return null;
        }

        /// <summary>
        /// Generates a byte[] containing a zip file with a file entry
        /// per device type. Each of the file entries will contains
        /// a header and then the data of all devices related to that specific
        /// device type. If there are no device types returns then null is returned.
        /// A file entry may contain only the header row if no device is associated
        /// with that specific device type
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateAllDeviceReports()
        {
            // Get a list of all device types
            var deviceTypesDocuments = db.GetAllDeviceTypes();
            if (deviceTypesDocuments == null || deviceTypesDocuments.Count == 0)
            {
                return null;
            }

            // Create dict to group devices based on device type id
            var typeToDevices = new Dictionary<ObjectId, List<Device>>();
            foreach(var deviceTypeDoc in deviceTypesDocuments)
            {
                var deviceType = BsonSerializer.Deserialize<DeviceType>(deviceTypeDoc);
                typeToDevices.Add(deviceType.Id, new List<Device>());
            }

            // Get all devices and add them to the dict based on deviceTypeId
            var deviceDocuments = db.GetAllDevices();           
            if(deviceDocuments != null)
            {
                foreach (var deviceDoc in deviceDocuments)
                {
                    var device = BsonSerializer.Deserialize<Device>(deviceDoc);                    
                    typeToDevices[device.DeviceTypeId].Add(device);
                }
            }      

            byte[] zipFileBytes;
            using (var outStream = new MemoryStream()) // Create memory stream to convert zip file stream to byte[]
            {
                using (var zipStream = new ZipArchive(outStream, ZipArchiveMode.Create, true)) // Create zip file stream that files can be streamed into
                {
                    foreach (var deviceTypeDoc in deviceTypesDocuments)
                    {
                        var deviceType = BsonSerializer.Deserialize<DeviceType>(deviceTypeDoc);

                        var fileInZip = zipStream.CreateEntry($"{deviceType.Name}.csv", CompressionLevel.Optimal); // Create new zip file entry that can be streamed into
                        using (var fileInZipStream = fileInZip.Open())                                             // Open zip file entry's stream
                        using (var dataStream = new MemoryStream())                                                // Create memory stream that data can be written into
                        using (var csvWriter = new StreamWriter(dataStream))                                       // Create writer to write device/device type data to memory stream
                        {
                            var deviceTypeDevices = typeToDevices[deviceType.Id];

                            // Write file headers                            
                            var headers = GenerateDeviceHeaders(deviceType);
                            if(deviceTypeDevices.Count == 0)
                            {
                                csvWriter.Write(headers);
                            }
                            else
                            {
                                csvWriter.WriteLine(headers);
                            }

                            // Write all device data                            
                            for(var i = 0; i < deviceTypeDevices.Count; i++)
                            {
                                // Write device's values
                                var device = deviceTypeDevices[i];
                                var values = GenerateDeviceValues(device, deviceType);  

                                // Ensure newline isn't added on last record
                                if(i != deviceTypeDevices.Count - 1)
                                {
                                    csvWriter.WriteLine(values);
                                } else
                                {
                                    csvWriter.Write(values);
                                }                                
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
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string GenerateDeviceHeaders(DeviceType deviceType, string delimiter = ",")
        {
            var headers = new List<string>();
            foreach (var deviceProp in typeof(Device).GetProperties())
            {
                if (deviceProp.Name == "Fields")
                {
                    // Add all of the DeviceType field names
                    var fieldProps = deviceType.Fields.ToDictionary().Values.Select(field => { return (string)field; });
                    headers.AddRange(fieldProps);
                }
                else if (deviceProp.Name == "Location")
                {
                    // Add all of the DeviceLocation prop names
                    var locProps = typeof(DeviceLocation).GetProperties().Select(prop => prop.Name);
                    headers.AddRange(locProps);
                }
                else if (deviceProp.Name == "LastModified")
                {
                    // Add all of the DeviceLastModified prop names
                    var modProps = typeof(DeviceLastModified).GetProperties().Select(prop => prop.Name);
                    headers.AddRange(modProps);
                }
                else if (deviceProp.Name != "Photo" && deviceProp.Name != "Files")
                {
                    // Add all 'primative' prop names
                    headers.Add(deviceProp.Name);
                }
            }
            return string.Join(delimiter, headers);
        }

        /// <summary>
        /// Creates a list of strings filled with all of the property values in the parameter Device object. The Fields
        /// property values are based on the keys in the parameter deviceType's fields and the values of the parameter device's fields.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceType"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        private static string GenerateDeviceValues(Device device, DeviceType deviceType, string delimiter = ",")
        {
            var values = new List<string>();
            foreach (var deviceProp in typeof(Device).GetProperties())
            {
                if (deviceProp.Name == "Fields")
                {
                    // Add all DeviceType field values contained in the device
                    var deviceTypeFieldKeys = deviceType.Fields.ToDictionary().Keys;
                    foreach (var key in deviceTypeFieldKeys)
                    {
                        string value = "";
                        if (device.Fields != null && device.Fields.ContainsKey(key)){
                            value = device.Fields[key];
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name == "Location")
                {
                    // Add all of the DeviceLocation prop values contained in the device
                    var locProps = typeof(DeviceLocation).GetProperties();
                    foreach (var locProp in locProps)
                    {
                        string value = "";
                        if (device.Location != null)
                        {
                            var propVal = locProp.GetValue(device.Location);
                            value = propVal?.ToString() ?? "";
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name == "LastModified")
                {
                    // Add all of the DeviceLastModified prop values contained in the device
                    var modProps = typeof(DeviceLastModified).GetProperties();
                    foreach (var modProp in modProps)
                    {
                        string value = "";
                        if(device.LastModified != null){
                            var propVal = modProp.GetValue(device.LastModified);
                            value = propVal?.ToString() ?? "";
                        }
                        values.Add(value);
                    }
                }
                else if (deviceProp.Name != "Photo" && deviceProp.Name != "Files")
                {
                    // Add all of the 'primative' prop values contained in the device                    
                    var propVal = deviceProp.GetValue(device);
                    string value = propVal?.ToString() ?? "";
                    values.Add(value);
                }
            }
            return string.Join(delimiter, values);
        }
    }
}
