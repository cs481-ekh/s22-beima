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
    public interface IReportService
    {
        public byte[] GeneratDeviceReportByDeviceType(ObjectId deviceType);

        public byte[] GenerateAllDeviceReports();
    }
}
