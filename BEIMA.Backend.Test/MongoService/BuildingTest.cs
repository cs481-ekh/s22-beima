using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace BEIMA.Backend.MongoService.Test
{
    [TestFixture]
    public class BuildingTest
    {
        //Instance variables
        readonly ObjectId validObjId = ObjectId.GenerateNewId();
        readonly string validName = "Building Name 123";
        readonly string validNumber = "1209";
        readonly string validNotes = "Example of notes.";

        readonly DateTime validDate = DateTime.UtcNow;
        readonly string validUser = "User1";

        readonly string validLatitude = "-000.21.21";
        readonly string validLongitude = "-323.32.32";

        [Test]
        public void BuildingNotInstantiated_InstantiateUsingFullConstructor_BuildingInstantiatedWithCorrectValues()
        {
            var building = new Building(validObjId, validName, validNumber, validNotes);
            building.SetLastModified(validDate, validUser);
            building.SetLocation(validLatitude, validLongitude);

            Assert.That(building.Id, Is.EqualTo(validObjId));
            Assert.That(building.Name, Is.EqualTo(validName));
            Assert.That(building.Number, Is.EqualTo(validNumber));
            Assert.That(building.Notes, Is.EqualTo(validNotes));

            var location = building.Location;
            Assert.That(location.Latitude, Is.EqualTo(validLatitude));
            Assert.That(location.Longitude, Is.EqualTo(validLongitude));

            var lastModified = building.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));
        }

        [Test]
        public void BuildingNotInstantiated_InstantiateUsingObjectInitializer_DeviceInstantiatedWithCorrectValues()
        {
            var building = new Building
            {
                Id = validObjId,
                Name = validName,
                Number = validNumber,
                Notes = validNotes
            };
            building.SetLastModified(validDate, validUser);
            building.SetLocation(validLatitude, validLongitude);

            Assert.That(building.Id, Is.EqualTo(validObjId));
            Assert.That(building.Name, Is.EqualTo(validName));
            Assert.That(building.Number, Is.EqualTo(validNumber));
            Assert.That(building.Notes, Is.EqualTo(validNotes));

            var location = building.Location;
            Assert.That(location.Latitude, Is.EqualTo(validLatitude));
            Assert.That(location.Longitude, Is.EqualTo(validLongitude));

            var lastModified = building.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(validDate).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(validUser));
        }

        [Test]
        public void BuildingNotInstantiated_InstantiateUsingObjectInitializerWithAllNullStringValues_GetBsonDocumentThrowsException()
        {
            var building = new Building
            {
                Id = validObjId,
                Name = null,
                Number = null,
                Notes = null
            };
            Assert.That(building.Id, Is.EqualTo(validObjId));
            Assert.That(building.Name, Is.Null);
            Assert.That(building.Number, Is.Null);
            Assert.That(building.Notes, Is.Null);

            Assert.Throws<ArgumentNullException>(() => { building.GetBsonDocument(); });
        }

        [Test]
        public void BuildingNotInstantiated_InstantiateUsingFullConstructorWithAllNullStringValues_ValuesAreReplacedWithEmptyStrings()
        {
            var building = new Building(validObjId, null, null, null);
            Assert.That(building.Id, Is.EqualTo(validObjId));
            Assert.That(building.Name, Is.EqualTo(string.Empty));
            Assert.That(building.Number, Is.EqualTo(string.Empty));
            Assert.That(building.Notes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void BuildingInstantiatedWithSomeNullValuesWithFullConstructor_CallGetBsonDocument_ExceptionIsThrown()
        {
            //SetLastModified was not called, so throw an exception.
            var building = new Building(validObjId, null, validNumber, validNotes);
            building.SetLocation(validLatitude, validLongitude);
            Assert.Throws<ArgumentNullException>(() => { building.GetBsonDocument(); });
        }

        [Test]
        public void BuildingInstantiatedWithSomeNullValuesWithObjectInitializer_CallGetBsonDocument_ExceptionIsThrown()
        {
            var building = new Building
            {
                Id = validObjId,
                Name = validName,
                Number = null,
                Notes = null
            };
            building.SetLastModified(validDate, validUser);
            Assert.Throws<ArgumentNullException>(() => { building.GetBsonDocument(); });
        }

        [Test]
        public void BuildingInstantiated_PassInNullsToSetterMethods_NullValuesReplacedWithDefaultValues()
        {
            var building = new Building
            {
                Id = validObjId,
                Name = validName,
                Number = validNumber,
                Notes = validNotes
            };
            building.SetLocation(null, null);
            building.SetLastModified(null, null);

            var location = building.Location;
            Assert.That(location.Latitude, Is.EqualTo(string.Empty));
            Assert.That(location.Longitude, Is.EqualTo(string.Empty));

            var lastModified = building.LastModified;
            Assert.That(lastModified.Date, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
            Assert.That(lastModified.User, Is.EqualTo(string.Empty));
        }
    }
}
