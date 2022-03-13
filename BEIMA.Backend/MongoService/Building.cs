using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// Object representation of a last modified document in a Building document
    /// </summary>
    public class BuildingLastModified
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("user")]
        public string User { get; set; }
    }

    /// <summary>
    /// Object representation of the location field in a Building document
    /// </summary>
    public class BuildingLocation
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
    /// This class represents a Building. This object contains all the required fields necessary
    /// for a Building. This is meant to be used to convert data received from an endpoint, back into a BSON object.
    /// </summary>
    public class Building
    {
        //Properties of a Device object
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("number")]
        public string Number { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("location")]
        public BuildingLocation Location { get; set; }

        [BsonElement("lastModified")]
        public BuildingLastModified LastModified { get; set; }

        /// <summary>
        /// Empty constructor, this allows for a Building to be instantiated through Object Initializers.
        /// </summary>
        public Building()
        {
            Location = new BuildingLocation();
            LastModified = new BuildingLastModified();
        }

        /// <summary>
        /// Full constructor to instantiate a Building object.
        /// </summary>
        /// <param name="id">ObjectId of the object itself.</param>
        /// <param name="deviceTypeId">ObjectId of the DeviceType this Device is linked to.</param>
        /// <param name="deviceTag">The device tag.</param>
        /// <param name="manufacturer">The manufacturer of the device.</param>
        /// <param name="modelNum">The model number of the device.</param>
        /// <param name="serialNum">The serial number of the device.</param>
        /// <param name="yearManufactured">The year the device was manufactured.</param>
        /// <param name="notes">Notes related to the device.</param>
        public Building(ObjectId id, string name, string number, string notes)
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
            if (DeviceTag == null
                || Manufacturer == null
                || ModelNum == null
                || SerialNum == null
                || YearManufactured == null
                || Notes == null
                || Location == null
                || LastModified == null
            )
            {
                throw new ArgumentNullException();
            }

            return this.ToBsonDocument();
        }

        /// <summary>
        /// Adds a custom field to the "fields" object.
        /// </summary>
        /// <param name="key">The ObjectId of the linked field in the linked DeviceType.</param>
        /// <param name="value">The actual value of the field.</param>
        public void AddField(string key, string value)
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

        /// <summary>
        /// Creates and adds a file to the device's file list
        /// </summary>
        /// <param name="fileUid">File's uid</param>
        /// <param name="fileName">File's filename</param>
        public void AddFile(string fileUid, string fileName)
        {
            var file = new DeviceFile()
            {
                FileName = fileName,
                FileUid = fileUid,
            };
            Files.Add(file);
        }

        /// <summary>
        /// Creates and adds a photo to the device's photo list
        /// </summary>
        /// <param name="fileUid">Photo's uid</param>
        /// <param name="fileName">Photo's filename</param>
        public void SetPhoto(string fileUid, string fileName)
        {
            var photo = new DeviceFile()
            {
                FileName = fileName,
                FileUid = fileUid,
            };
            Photo = photo;
        }
    }
}
