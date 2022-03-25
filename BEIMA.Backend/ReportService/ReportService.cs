using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.ReportService
{
    public class ReportService : IReportService
    {
        private readonly IMongoConnector db; 

        private ReportService()
        {
            db = MongoDefinition.MongoInstance;
        }

        public byte[] GeneratDeviceReportByDeviceType(ObjectId deviceType)
        {

            return null;
        }

        public byte[] GenerateAllDeviceReports()
        {
            var deviceDocuments = db.GetAllDevices();

            var typeToDevices = new Dictionary<ObjectId, List<Device>>();
            foreach (var deviceDoc in deviceDocuments)
            {
                var device = BsonSerializer.Deserialize<Device>(deviceDoc);
                if (typeToDevices.ContainsKey(device.DeviceTypeId))
                {
                    typeToDevices[device.DeviceTypeId].Add(device);
                } else
                {
                    typeToDevices.Add(device.DeviceTypeId, new List<Device> { device });
                }
            }
            
            var deviceTypes = db.GetAllDeviceTypes();





            return null;
        }
    }
}
