using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace BEIMA.Backend.MongoService
{
    public class BaseDevice
    {

        public BaseDevice()
        {
            if(Location == null)
            {
                Location = new BsonDocument();
            }
            if(Fields == null)
            {
                Fields = new BsonDocument();
            }
        }

        public BaseDevice(ObjectId id, string deviceTypeId, string deviceTag, string manufacturer, string modelNum, string serialNum, int yearManufactured, string notes)
        {
            BsonClassMap.RegisterClassMap<BaseDevice>();

            Id = id;
            DeviceTypeId = deviceTypeId;
            DeviceTag = deviceTag;
            Manufacturer = manufacturer;
            ModelNum = modelNum;
            SerialNum = serialNum;
            YearManufactured = yearManufactured;
            Notes = notes;
            Fields = new BsonDocument();
            Location = new BsonDocument();
            LastModified = new BsonDocument();
        }


        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("deviceTypeId")]
        public string DeviceTypeId { get; set; }

        [BsonElement("deviceTag")]
        public string DeviceTag { get; set; }

        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; }

        [BsonElement("modelNum")]
        public string ModelNum { get; set; }

        [BsonElement("serialNum")]
        public string SerialNum { get; set; }

        [BsonElement("yearManufactured")]
        public int YearManufactured { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("fields")]
        public BsonDocument Fields { get; set; }

        [BsonElement("location")]
        public BsonDocument Location { get; set; }

        [BsonElement("lastModified")]
        public BsonDocument LastModified { get; set; }

        public BsonDocument GetBsonDocument()
        {
            return this.ToBsonDocument();
        }

        public void AddField(string key, dynamic value)
        {
            Fields.Add(key, value);
        }

        public void SetLocation(string buildingId, string notes, string latitude, string longitude)
        {
            //Check if null, if they are, then use empty string
            buildingId ??= string.Empty;
            notes ??= string.Empty;
            latitude ??= string.Empty;
            longitude ??= string.Empty;

            Location.Add("buildingId", buildingId);
            Location.Add("notes", notes);
            Location.Add("latitude", latitude);
            Location.Add("longitude", longitude);
        }

        public void SetLastModified(DateTime date, string user)
        {
            LastModified.Add("date", date);
            LastModified.Add("user", user);
        }
        
    }
}
