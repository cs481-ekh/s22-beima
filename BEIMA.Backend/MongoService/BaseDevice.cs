using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// This class represents a Device. This object contains all the required fields necessary
    /// for a device. This is meant to be used to convert data received from an endpoint, back into a BSON object.
    /// </summary>
    public class BaseDevice
    {

        /// <summary>
        /// Empty constructor, this allows for a device to be instantiated through Object Initializers.
        /// </summary>
        public BaseDevice()
        {

            if (DeviceTag == null)
            {
                throw new ArgumentNullException($"BaseDevice - {0} is null", nameof(DeviceTag));
            }

            if (Manufacturer == null)
            {
                throw new ArgumentNullException($"BaseDevice - {0} is null", nameof(Manufacturer));
            }

            if (ModelNum == null)
            {
                throw new ArgumentNullException($"BaseDevice - {0} is null", nameof(ModelNum));
            }

            if (SerialNum == null)
            {
                throw new ArgumentNullException($"BaseDevice - {0} is null", nameof(SerialNum));
            }

            if (Notes == null)
            {
                throw new ArgumentNullException($"BaseDevice - {0} is null", nameof(Notes));
            }

            Fields = new BsonDocument();
            Location = new BsonDocument();
            LastModified = new BsonDocument();
        }

        /// <summary>
        /// Full constructor to instantiate a Device object.
        /// </summary>
        /// <param name="id">ObjectId of the object itself.</param>
        /// <param name="deviceTypeId">ObjectId of the DeviceType this Device is linked to.</param>
        /// <param name="deviceTag"></param>
        /// <param name="manufacturer"></param>
        /// <param name="modelNum"></param>
        /// <param name="serialNum"></param>
        /// <param name="yearManufactured"></param>
        /// <param name="notes"></param>
        public BaseDevice(ObjectId id, ObjectId deviceTypeId, string deviceTag, string manufacturer, string modelNum, string serialNum, int yearManufactured, string notes)
        {
            //BsonClassMap.RegisterClassMap<BaseDevice>();

            Id = id;
            DeviceTypeId = deviceTypeId;
            DeviceTag = deviceTag ?? string.Empty;
            Manufacturer = manufacturer ?? string.Empty;
            ModelNum = modelNum ?? string.Empty;
            SerialNum = serialNum ?? string.Empty;
            YearManufactured = yearManufactured;
            Notes = notes ?? string.Empty;
            Fields = new BsonDocument();
            Location = new BsonDocument();
            LastModified = new BsonDocument();
        }


        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("deviceTypeId")]
        public ObjectId DeviceTypeId { get; set; }

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

        /// <summary>
        /// Gets a BsonDocument that represents the current state of the BaseDevice object.
        /// </summary>
        /// <returns>BsonDocument that represents the current state of the BaseDevice object. Conforms to the schema located in BEIMA.DB.Schemas.</returns>
        public BsonDocument GetBsonDocument()
        {
            if(Location.ElementCount == 0)
            {
                throw new ArgumentNullException("BaseDevice - SetLocation has not been called yet!");
            }
            if (LastModified.ElementCount == 0)
            {
                throw new ArgumentNullException("BaseDevice - SetLastModified has not been called yet!");
            }
            return this.ToBsonDocument();
        }

        /// <summary>
        /// Adds a custom field to the "fields" object.
        /// </summary>
        /// <param name="key">The ObjectId of the linked field in the linked DeviceType.</param>
        /// <param name="value">The actual value of the field.</param>
        public void AddField(string key, dynamic value)
        {
            Fields.Set(key, value);
        }

        /// <summary>
        /// Sets all of the fields in the "location" object of the BaseDevice.
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="notes"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetLocation(ObjectId buildingId, string notes, string latitude, string longitude)
        {
            //Check if null, if they are, then use empty string
            notes ??= string.Empty;
            latitude ??= string.Empty;
            longitude ??= string.Empty;

            Location.Set("buildingId", buildingId);
            Location.Set("notes", notes);
            Location.Set("latitude", latitude);
            Location.Set("longitude", longitude);
        }

        /// <summary>
        /// Sets all of the fields in the "lastModified" object of the BaseDevice.
        /// </summary>
        /// <param name="date">Date of when the Device was last modified.</param>
        /// <param name="user">Username of the user who last modified the Device.</param>
        public void SetLastModified(DateTime? date, string user)
        {
            //Check if null, if they are, then use defualt values
            date ??= DateTime.Now.ToUniversalTime();
            user ??= string.Empty;

            LastModified.Set("date", date);
            LastModified.Set("user", user);
        }
        
    }
}
