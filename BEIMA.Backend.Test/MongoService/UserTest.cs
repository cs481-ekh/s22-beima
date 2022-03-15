using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class UserTest
    {
        //Instance variables
        readonly ObjectId validObjId = ObjectId.GenerateNewId();
        readonly string validUsername = "user.name";
        readonly string validPassword = "Thisisavalidpassword123!@";
        readonly string validFirstName = "Randy";
        readonly string validLastName = "Johnson";
        readonly string validRole = "admin";

        readonly DateTime validDate = DateTime.UtcNow;
        readonly string validUser = "User1";

        [Test]
        public void UserNotInstantiated_InstantiateUsingFullConstructor_UserInstantiatedWithCorrectValues()
        {
            var user = new User(validObjId, validUsername, validPassword, validFirstName, validLastName, validRole);
            user.SetLastModified(validDate, validUser);

            Assert.That(user.Id, Is.EqualTo(validObjId));
            Assert.That(user.Username, Is.EqualTo(validUsername));
            Assert.That(user.Password, Is.EqualTo(validPassword));
            Assert.That(user.FirstName, Is.EqualTo(validFirstName));
            Assert.That(user.LastName, Is.EqualTo(validLastName));
            Assert.That(user.Role, Is.EqualTo(validRole));

            var lastModified = user.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));
        }

        [Test]
        public void UserNotInstantiated_InstantiateUsingObjectInitializer_DeviceInstantiatedWithCorrectValues()
        {
            var user = new User
            {
                Id = validObjId,
                Username = validUsername,
                Password = validPassword,
                FirstName = validFirstName,
                LastName = validLastName,
                Role = validRole
            };
            user.SetLastModified(validDate, validUser);

            Assert.That(user.Id, Is.EqualTo(validObjId));
            Assert.That(user.Username, Is.EqualTo(validUsername));
            Assert.That(user.Password, Is.EqualTo(validPassword));
            Assert.That(user.FirstName, Is.EqualTo(validFirstName));
            Assert.That(user.LastName, Is.EqualTo(validLastName));
            Assert.That(user.Role, Is.EqualTo(validRole));

            var lastModified = user.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));
        }

        [Test]
        public void UserNotInstantiated_InstantiateUsingObjectInitializerWithAllNullStringValues_GetBsonDocumentThrowsException()
        {
            var user = new User
            {
                Id = validObjId,
                Username = null,
                Password = null,
                FirstName = null,
                LastName = null,
                Role = null
            };
            Assert.That(user.Id, Is.EqualTo(validObjId));
            Assert.That(user.Username, Is.Null);
            Assert.That(user.Password, Is.Null);
            Assert.That(user.FirstName, Is.Null);
            Assert.That(user.LastName, Is.Null);
            Assert.That(user.Role, Is.Null);

            Assert.Throws<ArgumentNullException>(() => { user.GetBsonDocument(); });
        }

        [Test]
        public void UserNotInstantiated_InstantiateUsingFullConstructorWithAllNullStringValues_ValuesAreReplacedWithEmptyStrings()
        {
            var user = new User(validObjId, null, null, null, null, null);
            Assert.That(user.Id, Is.EqualTo(validObjId));
            Assert.That(user.Username, Is.EqualTo(string.Empty));
            Assert.That(user.Password, Is.EqualTo(string.Empty));
            Assert.That(user.FirstName, Is.EqualTo(string.Empty));
            Assert.That(user.LastName, Is.EqualTo(string.Empty));
            Assert.That(user.Role, Is.EqualTo(string.Empty));
        }

        [Test]
        public void UserInstantiatedWithSomeNullValuesWithFullConstructor_CallGetBsonDocument_ExceptionIsThrown()
        {
            //SetLastModified was not called, so throw an exception.
            var user = new User(validObjId, null, validPassword, validFirstName, null, null);
            Assert.Throws<ArgumentNullException>(() => { user.GetBsonDocument(); });
        }

        [Test]
        public void UserInstantiatedWithSomeNullValuesWithObjectInitializer_CallGetBsonDocument_ExceptionIsThrown()
        {
            var user = new User
            {
                Id = validObjId,
                Username = validUsername,
                Password = null,
                FirstName = null,
                LastName = null,
                Role = validRole
            };
            user.SetLastModified(validDate, validUser);
            Assert.Throws<ArgumentNullException>(() => { user.GetBsonDocument(); });
        }

        [Test]
        public void UserInstantiated_PassInNullsToSetterMethods_NullValuesReplacedWithDefaultValues()
        {
            var user = new User
            {
                Id = validObjId,
                Username = validUsername,
                Password = validPassword,
                FirstName = validFirstName,
                LastName = validLastName,
                Role = validRole
            };
            user.SetLastModified(null, null);

            var lastModified = user.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(string.Empty));
        }
    }
}
