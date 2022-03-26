using BEIMA.Backend.MongoService;
using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BEIMA.Backend.Test.MongoService
{
    [TestFixture]
    public class MongoConnectorTest : UnitTestBase
    {
        #region Class Tests
        [Test]
        public void ConnectorNotCreated_CallConstructorFiveTimes_FiveInstancesAreEqual()
        {
            //Tests to make sure singleton is working
            var connector1 = MongoConnector.Instance;
            var connector2 = MongoConnector.Instance;
            var connector3 = MongoConnector.Instance;
            var connector4 = MongoConnector.Instance;
            var connector5 = MongoConnector.Instance;
            Assert.That(connector1, Is.EqualTo(connector2));
            Assert.That(connector1, Is.EqualTo(connector3));
            Assert.That(connector1, Is.EqualTo(connector4));
            Assert.That(connector1, Is.EqualTo(connector5));
        }

        [Test]
        public void ConnectorCreated_CheckIsConnected_TrueReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.IsConnected();
            Assert.That(result, Is.True);
        }
        #endregion

        #region Device Tests
        [Test]
        public void ConnectorCreated_GetDeviceGivenValidDeviceId_DeviceDocumentReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            BsonDocument doc = new BsonDocument {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve the document)
            var retrievedDoc = mongo.GetDevice((ObjectId)doc["_id"]);
            Assert.IsNotNull(retrievedDoc);
            Assert.That(retrievedDoc, Is.TypeOf(typeof(BsonDocument)));
        }

        [Test]
        public void ConnectorCreated_GetAllDevices_DeviceDocumentListReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            BsonDocument doc = new BsonDocument {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve all documents)
            var retrievedDocs = mongo.GetAllDevices();
            Assert.IsNotNull(retrievedDocs);
            Assert.That(retrievedDocs, Is.TypeOf(typeof(List<BsonDocument>)));
        }

        [Test]
        public void DeviceNotInserted_InsertDevice_DeviceHasBeenInserted()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            var result = mongo.InsertDevice(doc);
            Assert.IsNotNull(result);
            Assert.That(result, Is.TypeOf(typeof(ObjectId)));
        }

        [Test]
        public void InsertDevice_DeleteDevice_DeviceHasBeenDeleted()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument
            {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Delete device
            bool deleteResult = false;
            if (insertResult != null)
            {
                deleteResult = mongo.DeleteDevice((ObjectId)insertResult);
            }
            Assert.That(deleteResult, Is.True);
        }

        [Test]
        public void InsertDevice_UpdateDevice_DeviceHasBeenUpdated()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument
            {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Update device
            doc.AddRange(new BsonDocument { { "updatedDeviceField", "123" } });
            var updateResult = mongo.UpdateDevice(doc);
            Assert.IsNotNull(updateResult);
        }

        [Test]
        public void ConnectorCreated_GetDeviceGivenInvalidDeviceId_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var doc = mongo.GetDevice(new ObjectId("61f4882f42dab933544849d3"));
            Assert.IsNull(doc);
        }

        [Test]
        public void ConnectorCreated_DeleteInvalidDevice_FalseReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var result = mongo.DeleteDevice(new ObjectId("61f4882f42dab933544849d3"));
            Assert.IsFalse(result);
        }

        [Test]
        public void ConnectorCreated_InsertNullDevice_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.InsertDevice(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ConnectorCreated_UpdateNullDevice_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.UpdateDevice(null);
            Assert.IsNull(result);
        }

        [TestCase("serialNumber", "a12345")]
        [TestCase("key1", 12345)]
        [TestCase("key2", true)]
        public void InsertDevice_GetFilteredDevices_DeviceInList(string key, dynamic value)
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument
            {
                { key, value }
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //GetFiltered
            var filter = MongoFilterGenerator.GetEqualsFilter(key, value);
            var list = mongo.GetFilteredDevices(filter);
            foreach (var device in list)
            {
                Assert.That(device.GetElement(key).Value.ToString().ToLower(), Is.EqualTo(value.ToString().ToLower()));
            }
        }

        [TestCase("InvalidKey1", "aaaaaaaaaaaa")]
        [TestCase("aaaaaaaaaaaaaa", 12345)]
        [TestCase("bbbbbbb", true)]
        public void CreateFilterWithNoResults_GetFilteredDevices_NoResultsInList(string key, dynamic value)
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument
            {
                { "item", "isNotInDb" }
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Create filter
            var filter = MongoFilterGenerator.GetEqualsFilter(key, value);

            //GetFilteredDevices
            var list = mongo.GetFilteredDevices(filter);
            
            //NoResultsInList
            Assert.That(list.Count, Is.EqualTo(0));
        }

        #endregion

        #region DeviceType Tests

        [Test]
        public void ConnectorCreated_GetDeviceTypeGivenValidDeviceTypeId_DeviceDocumentReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestDeviceType"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertDeviceType(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve the document)
            var retrievedDoc = mongo.GetDeviceType((ObjectId)doc["_id"]);
            Assert.IsNotNull(retrievedDoc);
            Assert.That(retrievedDoc, Is.TypeOf(typeof(BsonDocument)));
        }

        [Test]
        public void ConnectorCreated_GetAllDeviceTypes_DeviceTypeDocumentListReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestDeviceType"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertDeviceType(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve all documents)
            var retrievedDocs = mongo.GetAllDeviceTypes();
            Assert.IsNotNull(retrievedDocs);
            Assert.That(retrievedDocs, Is.TypeOf(typeof(List<BsonDocument>)));
        }

        [Test]
        public void InsertDeviceType_DeleteDeviceType_DeviceTypeHasBeenDeleted()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestDeviceType"},
                { "description", "This is a test" }
            };
            //Insert device type
            var insertResult = mongo.InsertDeviceType(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Delete device type
            bool deleteResult = false;
            if (insertResult != null)
            {
                deleteResult = mongo.DeleteDeviceType((ObjectId)insertResult);
                Assert.IsNull(mongo.GetDeviceType((ObjectId)insertResult));
            }
            Assert.That(deleteResult, Is.True);
        }

        [Test]
        public void InsertDeviceType_UpdateDeviceType_DeviceTypeHasBeenUpdated()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestDeviceType"},
                { "description", "This is a test" }
            };
            //Insert device type
            var insertResult = mongo.InsertDeviceType(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Update device type
            doc.AddRange(new BsonDocument { { "updatedDeviceTypeField", "123" } });
            var updateResult = mongo.UpdateDeviceType(doc);
            Assert.IsNotNull(updateResult);
        }

        [Test]
        public void ConnectorCreated_GetDeviceTypeGivenInvalidDeviceTypeId_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var doc = mongo.GetDeviceType(ObjectId.GenerateNewId());
            Assert.IsNull(doc);
        }

        [Test]
        public void ConnectorCreated_DeleteInvalidDeviceType_FalseReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var result = mongo.DeleteDeviceType(ObjectId.GenerateNewId());
            Assert.IsFalse(result);
        }

        [Test]
        public void ConnectorCreated_InsertNullDeviceType_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.InsertDeviceType(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ConnectorCreated_UpdateNullDeviceType_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.UpdateDeviceType(null);
            Assert.IsNull(result);
        }

        [TestCase("name", "a12345")]
        [TestCase("key1", 12345)]
        [TestCase("key2", true)]
        public void InsertDeviceType_GetFilteredDeviceTypes_DeviceInList(string key, dynamic value)
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument
            {
                { key, value }
            };
            //Insert device type
            var insertResult = mongo.InsertDeviceType(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //GetFiltered
            var filter = mongo.GetEqualsFilter(key, value);
            var list = mongo.GetFilteredDeviceTypes(filter);
            foreach (var device in list)
            {
                Assert.That(device.GetElement(key).Value.ToString().ToLower(), Is.EqualTo(value.ToString().ToLower()));
            }
        }


        #endregion

        #region Building Tests

        [Test]
        public void ConnectorCreated_GetBuildingGivenValidBuildingId_BuildingDocumentReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestBuilding"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertBuilding(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve the document)
            var retrievedDoc = mongo.GetBuilding((ObjectId)doc["_id"]);
            Assert.IsNotNull(retrievedDoc);
            Assert.That(retrievedDoc, Is.TypeOf(typeof(BsonDocument)));
        }

        [Test]
        public void ConnectorCreated_GetAllBuildings_BuildingDocumentListReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestBuilding"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertBuilding(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve all documents)
            var retrievedDocs = mongo.GetAllBuildings();
            Assert.IsNotNull(retrievedDocs);
            Assert.That(retrievedDocs, Is.TypeOf(typeof(List<BsonDocument>)));
        }

        [Test]
        public void InsertBuilding_DeleteBuilding_BuildingHasBeenDeleted()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestBuilding"},
                { "description", "This is a test" }
            };
            //Insert building
            var insertResult = mongo.InsertBuilding(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Delete building
            bool deleteResult = false;
            if (insertResult != null)
            {
                deleteResult = mongo.DeleteBuilding((ObjectId)insertResult);
                Assert.IsNull(mongo.GetBuilding((ObjectId)insertResult));
            }
            Assert.That(deleteResult, Is.True);
        }

        [Test]
        public void InsertBuilding_UpdateBuilding_BuildingHasBeenUpdated()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestBuilding"},
                { "description", "This is a test" }
            };
            //Insert building
            var insertResult = mongo.InsertBuilding(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Update building
            doc.AddRange(new BsonDocument { { "updatedBuildingField", "123" } });
            var updateResult = mongo.UpdateBuilding(doc);
            Assert.IsNotNull(updateResult);
        }

        [Test]
        public void ConnectorCreated_GetBuildingGivenInvalidBuildingId_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var doc = mongo.GetBuilding(ObjectId.GenerateNewId());
            Assert.IsNull(doc);
        }

        [Test]
        public void ConnectorCreated_DeleteInvalidBuilding_FalseReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var result = mongo.DeleteBuilding(ObjectId.GenerateNewId());
            Assert.IsFalse(result);
        }

        [Test]
        public void ConnectorCreated_InsertNullBuilding_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.InsertBuilding(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ConnectorCreated_UpdateNullBuilding_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.UpdateBuilding(null);
            Assert.IsNull(result);
        }

        #endregion

        #region User Tests

        [Test]
        public void ConnectorCreated_GetUserGivenValidUserId_UserDocumentReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestUser"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertUser(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve the document)
            var retrievedDoc = mongo.GetUser((ObjectId)doc["_id"]);
            Assert.IsNotNull(retrievedDoc);
            Assert.That(retrievedDoc, Is.TypeOf(typeof(BsonDocument)));
        }

        [Test]
        public void ConnectorCreated_GetAllUsers_UserDocumentListReturned()
        {
            var mongo = MongoConnector.Instance;
            //Setup (inserting a document)
            var doc = new BsonDocument {
                { "name", "TestUser"},
                { "description", "This is a test" }
            };
            var insertResult = mongo.InsertUser(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve all documents)
            var retrievedDocs = mongo.GetAllUsers();
            Assert.IsNotNull(retrievedDocs);
            Assert.That(retrievedDocs, Is.TypeOf(typeof(List<BsonDocument>)));
        }

        [Test]
        public void InsertUser_DeleteUser_UserHasBeenDeleted()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestUser"},
                { "description", "This is a test" }
            };
            //Insert user
            var insertResult = mongo.InsertUser(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Delete user
            bool deleteResult = false;
            if (insertResult != null)
            {
                deleteResult = mongo.DeleteUser((ObjectId)insertResult);
                Assert.IsNull(mongo.GetUser((ObjectId)insertResult));
            }
            Assert.That(deleteResult, Is.True);
        }

        [Test]
        public void InsertUser_UpdateUser_UserHasBeenUpdated()
        {
            var mongo = MongoConnector.Instance;
            var doc = new BsonDocument {
                { "name", "TestUser"},
                { "description", "This is a test" }
            };
            //Insert user
            var insertResult = mongo.InsertUser(doc);
            Assume.That(insertResult, Is.Not.Null);
            Assume.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Update user
            doc.AddRange(new BsonDocument { { "updatedUserField", "123" } });
            var updateResult = mongo.UpdateUser(doc);
            Assert.IsNotNull(updateResult);
        }

        [Test]
        public void ConnectorCreated_GetUserGivenInvalidUserId_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var doc = mongo.GetUser(ObjectId.GenerateNewId());
            Assert.IsNull(doc);
        }

        [Test]
        public void ConnectorCreated_DeleteInvalidUser_FalseReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var result = mongo.DeleteUser(ObjectId.GenerateNewId());
            Assert.IsFalse(result);
        }

        [Test]
        public void ConnectorCreated_InsertNullUser_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.InsertUser(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ConnectorCreated_UpdateNullUser_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.UpdateUser(null);
            Assert.IsNull(result);
        }

        #endregion
    }
}
