using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.ReportService
{
    /// <summary>
    /// This class abstracts basic report generation operations. It is implemented as a 
    /// singleton class.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Generates a byte[] containing a file object. The
        /// file containes a header and then the data of all devices
        /// related to the specific deviceTypeId. If the device type id
        /// doesn't exist, return null, 
        /// </summary>
        /// <returns>Byte[] containing a file filled with device data associated with the passed in deviceTypeId</returns>
        public byte[] GeneratDeviceReportByDeviceType(ObjectId deviceTypeId);

        /// <summary>
        /// Generates a byte[] containing a zip file with a file entry
        /// per device type. Each of the file entries will contains
        /// a header and then the data of all devices related to that specific
        /// device type. If there are no device types returns then null is returned.
        /// A file entry may contain only the header row if no device is associated
        /// with that specific device type
        /// </summary>
        /// <returns>Byte[] containing a zipfile of device type files filled with associated devices</returns>
        public byte[] GenerateAllDeviceReports();
    }
}
