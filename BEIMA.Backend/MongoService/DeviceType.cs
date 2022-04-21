using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// This class represents a DeviceType. This object contains all the required fields necessary
    /// for a device type. This is meant to be used to convert data received from an endpoint, back into a BSON object.
    /// </summary>
    public class DeviceType
    {
        // Properties of a DeviceType object
        [BsonId]
        [JsonProperty(PropertyName = "_id")]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("lastModified")]
        public BsonDocument LastModified { get; set; }

        [BsonElement("fields")]
        public BsonDocument Fields { get; set; }

        /// <summary>
        /// Empty constructor, this allows for a device type to be instantiated through Object Initializers.
        /// </summary>
        public DeviceType()
        {
            Fields = new BsonDocument();
            LastModified = new BsonDocument();
        }

        /// <summary>
        /// Full constructor to instantiate a DeviceType object.
        /// </summary>
        /// <param name="id">The ObjectId of the DeviceType.</param>
        /// <param name="name">The name of the DeviceType itself.</param>
        /// <param name="description">The description of the DeviceType itself.</param>
        /// <param name="notes">The notes related to the DeviceType itself.</param>
        public DeviceType(ObjectId id, string name, string description, string notes)
        {
            Id = id;
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            Notes = notes ?? string.Empty;
            Fields = new BsonDocument();
            LastModified = new BsonDocument();
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
        /// Gets a BsonDocument that represents the current state of the DeviceType object.
        /// </summary>
        /// <returns>BsonDocument that represents the current state of the DeviceType object. Conforms to the schema located in BEIMA.DB.Schemas.</returns>
        /// <exception cref="ArgumentNullException">Throws exception when any of the required fields are null.</exception>
        public BsonDocument GetBsonDocument()
        {
            CheckNullArgument(Name, nameof(Name));
            CheckNullArgument(Description, nameof(Description));
            CheckNullArgument(Notes, nameof(Notes));
            CheckNullArgument(LastModified, nameof(LastModified));

            return this.ToBsonDocument();
        }

        /// <summary>
        /// Adds a custom field to the "fields" object.
        /// </summary>
        /// <param name="key">The UID of the custom field defined here in "deviceType.fields".</param>
        /// <param name="value">The actual value of the field.</param>
        public void AddField(string key, dynamic value)
        {
            Fields.Set(key, value);
        }

        /// <summary>
        /// Sets all of the fields in the "lastModified" object of the DeviceType.
        /// </summary>
        /// <param name="date">Date of when the DeviceType was last modified.</param>
        /// <param name="user">Username of the user who last modified the DeviceType.</param>
        public void SetLastModified(DateTime? date, string user)
        {
            // Check if null, if they are, then use defualt values
            date ??= DateTime.UtcNow;
            user ??= string.Empty;

            LastModified.Set("date", date);
            LastModified.Set("user", user);
        }

    }
}
