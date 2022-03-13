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
        /// <param name="id">ObjectId of the document itself.</param>
        /// <param name="name">Building name</param>
        /// <param name="number">Building number</param>
        /// <param name="notes">Notes </param>
        public Building(ObjectId id, string name, string number, string notes)
        {
            Id = id;
            Name = name ?? string.Empty;
            Number = number ?? string.Empty;
            Notes = notes ?? string.Empty;
            Location = new BuildingLocation();
            LastModified = new BuildingLastModified();
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
                throw new ArgumentNullException($"Building - {name} is null");
            }
        }

        /// <summary>
        /// Gets a BsonDocument that represents the current state of the Device object.
        /// </summary>
        /// <returns>BsonDocument that represents the current state of the Device object. Conforms to the schema located in BEIMA.DB.Schemas.</returns>
        /// <exception cref="ArgumentNullException">Throws exception when any of the required fields are null.</exception>
        public BsonDocument GetBsonDocument()
        {
            CheckNullArgument(Name, nameof(Name));
            CheckNullArgument(Number, nameof(Number));
            CheckNullArgument(Notes, nameof(Notes));
            CheckNullArgument(Location.Longitude, "Location.Longitude");
            CheckNullArgument(Location.Latitude, "Location.Latitude");
            CheckNullArgument(LastModified.User, "LastModified.User");
            CheckNullArgument(LastModified.Date, "LastModified.Date");

            return this.ToBsonDocument();
        }

        /// <summary>
        /// Sets all of the fields in the "location" object of the Building.
        /// </summary>
        /// <param name="latitude">The latitude of the physical location of the building.</param>
        /// <param name="longitude">The longitude of the physical location of the building.</param>
        public void SetLocation(string latitude, string longitude)
        {
            //Check if null, if they are, then use empty string
            latitude ??= string.Empty;
            longitude ??= string.Empty;

            Location.Latitude = latitude;
            Location.Longitude = longitude;
        }

        /// <summary>
        /// Sets all of the fields in the "lastModified" object of the Building.
        /// </summary>
        /// <param name="date">Date of when the Building was last modified.</param>
        /// <param name="user">Username of the user who last modified the Building.</param>
        public void SetLastModified(DateTime? date, string user)
        {
            //Check if null, if they are, then use defualt values
            date ??= DateTime.UtcNow;
            user ??= string.Empty;

            LastModified.Date = (DateTime)date;
            LastModified.User = user;
        }
    }
}
