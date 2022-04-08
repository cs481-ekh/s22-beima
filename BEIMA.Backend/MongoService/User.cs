using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BEIMA.Backend.MongoService
{
    /// <summary>
    /// Object representation of a last modified document in a User document
    /// </summary>
    public class UserLastModified
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("user")]
        public string User { get; set; }
    }

    /// <summary>
    /// This class represents a User. This object contains all the required fields necessary
    /// for a User. This is meant to be used to convert data received from an endpoint, back into a BSON object.
    /// </summary>
    public class User
    {
        //Properties of a User object
        [BsonId]
        [JsonProperty(PropertyName = "_id")]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        [BsonElement("lastModified")]
        public UserLastModified LastModified { get; set; }

        /// <summary>
        /// Empty constructor, this allows for a User to be instantiated through Object Initializers.
        /// </summary>
        public User()
        {
            LastModified = new UserLastModified();
        }

        /// <summary>
        /// Full constructor to instantiate a User object.
        /// </summary>
        /// <param name="id">ObjectId of the document itself.</param>
        /// <param name="username">Username of the user.</param>
        /// <param name="password">Password of the user. (in hashed form)</param>
        /// <param name="firstName">FirstName of the user.</param>
        /// <param name="lastName">LastName of the user.</param>
        /// <param name="role">Role of the user. (admin, user)</param>
        public User(ObjectId id, string username, string password, string firstName, string lastName, string role)
        {
            Id = id;
            Username = username ?? string.Empty;
            Password = password ?? string.Empty;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Role = role ?? string.Empty;
            LastModified = new UserLastModified();
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
                throw new ArgumentNullException($"User - {name} is null");
            }
        }

        /// <summary>
        /// Gets a BsonDocument that represents the current state of the User object.
        /// </summary>
        /// <returns>BsonDocument that represents the current state of the User object. Conforms to the schema located in BEIMA.DB.Schemas.</returns>
        /// <exception cref="ArgumentNullException">Throws exception when any of the required fields are null.</exception>
        public BsonDocument GetBsonDocument()
        {
            CheckNullArgument(Username, nameof(Username));
            CheckNullArgument(Password, nameof(Password));
            CheckNullArgument(FirstName, nameof(FirstName));
            CheckNullArgument(LastName, nameof(LastName));
            CheckNullArgument(Role, nameof(Role));
            CheckNullArgument(LastModified.User, "LastModified.User");
            CheckNullArgument(LastModified.Date, "LastModified.Date");

            return this.ToBsonDocument();
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
