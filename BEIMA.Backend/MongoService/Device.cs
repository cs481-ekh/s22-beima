using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// Object representation of a last modified document in a device document
    /// </summary>
    public class DeviceLastModified
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("user")]
        public string User { get; set; }
    }

    /// <summary>
    /// Object representation of a photo/file document in a device document's photo and file list.
    /// Nothing is stored in Url only used when device is returned as a view model.
    /// </summary>
    public class DeviceFile
    {
        [BsonElement("fileName")]
        public string FileName { get; set; }
        [BsonElement("fileUid")]
        public string FileUid { get; set; }
        [BsonElement("fileUrl")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Object representation of the location field in a device document
    /// </summary>
    public class DeviceLocation
    {
        [BsonElement("buildingId")]
        public ObjectId BuildingId { get; set; }
        [BsonElement("notes")]
        public string Notes { get; set; }
        [BsonElement("latitude")]
        public string Latitude { get; set; }
        [BsonElement("longitude")]
        public string Longitude { get; set; }
    }

    /// <summary>
    /// This class represents a Device. This object contains all the required fields necessary
    /// for a device. This is meant to be used to convert data received from an endpoint, back into a BSON object.
    /// </summary>
    public class Device
    {
        //Properties of a Device object
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
        public int? YearManufactured { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("fields")]
        public Dictionary<string,string> Fields { get; set; }

        [BsonElement("location")]
        public DeviceLocation Location { get; set; }

        [BsonElement("lastModified")]
        public DeviceLastModified LastModified { get; set; }

        [BsonElement("files")]
        public List<DeviceFile> Files { get; set; }

        [BsonElement("photos")]
        public List<DeviceFile> Photos { get; set; }       

        /// <summary>
        /// Empty constructor, this allows for a device to be instantiated through Object Initializers.
        /// </summary>
        public Device()
        {
            Fields = new Dictionary<string, string>();
            LastModified = new DeviceLastModified();
            Location = new DeviceLocation();
            Files = new List<DeviceFile>();
            Photos = new List<DeviceFile>();
        }

        /// <summary>
        /// Full constructor to instantiate a Device object.
        /// </summary>
        /// <param name="id">ObjectId of the object itself.</param>
        /// <param name="deviceTypeId">ObjectId of the DeviceType this Device is linked to.</param>
        /// <param name="deviceTag">The device tag.</param>
        /// <param name="manufacturer">The manufacturer of the device.</param>
        /// <param name="modelNum">The model number of the device.</param>
        /// <param name="serialNum">The serial number of the device.</param>
        /// <param name="yearManufactured">The year the device was manufactured.</param>
        /// <param name="notes">Notes related to the device.</param>
        public Device(ObjectId id, ObjectId deviceTypeId, string deviceTag, string manufacturer, string modelNum, string serialNum, int? yearManufactured, string notes)
        {
            Id = id;
            DeviceTypeId = deviceTypeId;
            DeviceTag = deviceTag ?? string.Empty;
            Manufacturer = manufacturer ?? string.Empty;
            ModelNum = modelNum ?? string.Empty;
            SerialNum = serialNum ?? string.Empty;
            YearManufactured = yearManufactured ?? -1;
            Notes = notes ?? string.Empty;
            Fields = new Dictionary<string, string>();
            Location = new DeviceLocation();
            LastModified = new DeviceLastModified();
            Files = new List<DeviceFile>();
            Photos = new List<DeviceFile>();
        }

        /// <summary>
        /// Internal method to check if a particular property of this class is null
        /// </summary>
        /// <param name="arg">The Property being null checked.</param>
        /// <param name="name">The name of the Property, to be returned in the exception message.</param>
        /// <exception cref="ArgumentNullException">Throws an exception when the passed in argument is null.</exception>
        private void CheckNullArgument(dynamic arg, string name = "")
        {
            if (arg == null)
            {
                throw new ArgumentNullException($"Device - {name} is null");
            }

            if (arg is BsonDocument && arg.ElementCount == 0)
            {
                throw new ArgumentNullException($"Device - Set{name} has not been called yet!");
            }
        }

        /// <summary>
        /// Gets a BsonDocument that represents the current state of the Device object.
        /// </summary>
        /// <returns>BsonDocument that represents the current state of the Device object. Conforms to the schema located in BEIMA.DB.Schemas.</returns>
        /// <exception cref="ArgumentNullException">Throws exception when any of the required fields are null.</exception>
        public BsonDocument GetBsonDocument()
        {
            CheckNullArgument(DeviceTag, nameof(DeviceTag));
            CheckNullArgument(Manufacturer, nameof(Manufacturer));
            CheckNullArgument(ModelNum, nameof(ModelNum));
            CheckNullArgument(SerialNum, nameof(SerialNum));
            CheckNullArgument(YearManufactured, nameof(YearManufactured));
            CheckNullArgument(Notes, nameof(Notes));
            CheckNullArgument(Location, nameof(Location));
            CheckNullArgument(LastModified, nameof(LastModified));

            return this.ToBsonDocument();
        }

        /// <summary>
        /// Adds a custom field to the "fields" object.
        /// </summary>
        /// <param name="key">The ObjectId of the linked field in the linked DeviceType.</param>
        /// <param name="value">The actual value of the field.</param>
        public void AddField(string key, dynamic value)
        {
            Fields.Add(key, value);
        }

        /// <summary>
        /// Sets all of the fields in the "location" object of the Device.
        /// </summary>
        /// <param name="buildingId">ObjectId of the Building document this device is related to.</param>
        /// <param name="notes">Notes related to the location of the device.</param>
        /// <param name="latitude">The latitude of the physical location of the device.</param>
        /// <param name="longitude">The longitude of the physical location of the device.</param>
        public void SetLocation(ObjectId buildingId, string notes, string latitude, string longitude)
        {
            //Check if null, if they are, then use empty string
            notes ??= string.Empty;
            latitude ??= string.Empty;
            longitude ??= string.Empty;

            Location.BuildingId = buildingId;
            Location.Notes = notes;
            Location.Latitude = latitude;   
            Location.Longitude = longitude; 
        }

        /// <summary>
        /// Sets all of the fields in the "lastModified" object of the Device.
        /// </summary>
        /// <param name="date">Date of when the Device was last modified.</param>
        /// <param name="user">Username of the user who last modified the Device.</param>
        public void SetLastModified(DateTime? date, string user)
        {
            //Check if null, if they are, then use defualt values
            var modifiedDate = (date ??= DateTime.Now.ToUniversalTime());
            user ??= string.Empty;

            LastModified.Date = modifiedDate;
            LastModified.User = user;
        }

        public void AddFile(string fileUid, string fileName)
        {
            var file = new DeviceFile()
            {
                FileName = fileName,
                FileUid = fileUid,
            };
            Files.Add(file);
        }

        public void AddPhoto(string fileUid, string fileName)
        {
            var photo = new DeviceFile()
            {
                FileName = fileName,
                FileUid = fileUid,
            };
            Photos.Add(photo);
        }
    }
}
