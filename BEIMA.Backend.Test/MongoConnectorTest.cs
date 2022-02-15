using NUnit.Framework;
using BEIMA.Backend.MongoService;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using MongoDB.Bson;

namespace BEIMA.Backend.Test
{
    [TestFixture]
    public class MongoConnectorTest
    {
        //private MongoConnector? mongo;
        class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //For local testing, the try block will be used to instantiate local variables.
            //For cloud testing, this will throw an exception, and it will use the env variables
            //defined in beima.yml.
            try
            {
                string[] paths = { "..", "..", "..", "..", "BEIMA.Backend", "local.settings.json" };
                string combinedPath = Path.Combine(paths);
                string fullPath = Path.GetFullPath(combinedPath);
                var settings = JsonConvert.DeserializeObject<LocalSettings>(
                    File.ReadAllText(fullPath));

                foreach (var setting in settings.Values)
                {
                    Environment.SetEnvironmentVariable(setting.Key, setting.Value);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

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
        public void ConnectorNotCreated_CallConstructorOnce_InstanceConnectedToCorrectEnvironment()
        {
            var mongo = MongoConnector.Instance;
            if (Environment.GetEnvironmentVariable("CurrentEnv") == "dev-local")
            {
                Assert.IsTrue(mongo.CurrentServerType == ServerType.Local);
            }
            else
            {
                Assert.IsTrue(mongo.CurrentServerType == ServerType.Cloud);
            }
        }

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
            Assert.IsNotNull(insertResult);
            Assert.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve the document)
            var retrievedDoc = mongo.GetDevice((ObjectId) doc["_id"]);
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
            Assert.IsNotNull(insertResult);
            Assert.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Test (retrieve all documents)
            var retrievedDocs = mongo.GetAllDevices();
            Assert.IsNotNull(retrievedDocs);
            Assert.That(retrievedDocs, Is.TypeOf(typeof(List<BsonDocument>)));
        }

        [Test]
        public void DocumentNotInserted_InsertDocument_DocumentHasBeenInserted()
        {
            var mongo = MongoConnector.Instance;
            BsonDocument doc = new BsonDocument { 
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            var result = mongo.InsertDevice(doc);
            Console.WriteLine(result.ToString());
            Assert.IsNotNull(result);
            Assert.That(result, Is.TypeOf(typeof(ObjectId)));
        }

        [Test]
        public void DocumentNotInserted_InsertDocumentAndDeleteDocument_DocumentHasBeenDeleted()
        {
            var mongo = MongoConnector.Instance;
            BsonDocument doc = new BsonDocument
            {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assert.IsNotNull(insertResult);
            Assert.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Delete device
            bool deleteResult = false;
            if(insertResult != null)
            {
                deleteResult = mongo.DeleteDevice((ObjectId) insertResult);
            }
            Assert.IsTrue(deleteResult);
        }

        [Test]
        public void DocumentNotInserted_InsertDocumentAndUpdateDocument_DocumentHasBeenUpdated()
        {
            var mongo = MongoConnector.Instance;
            BsonDocument doc = new BsonDocument
            {
                { "deviceTypeId", "a" },
                { "serialNumber", "a12345"}
            };
            //Insert device
            var insertResult = mongo.InsertDevice(doc);
            Assert.IsNotNull(insertResult);
            Assert.That(insertResult, Is.TypeOf(typeof(ObjectId)));

            //Update device
            doc.AddRange(new BsonDocument { { "updatedDeviceField", "123" } });
            var updateResult = mongo.UpdateDevice(doc);
            Assert.IsTrue(updateResult);
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
        public void ConnectorCreated_DeleteInvalidObject_FalseReturned()
        {
            var mongo = MongoConnector.Instance;
            //This is a valid ObjectId, but this is not in the database
            var result = mongo.DeleteDevice(new ObjectId("61f4882f42dab933544849d3"));
            Assert.IsFalse(result);
        }

        [Test]
        public void ConnectorCreated_InsertNull_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.InsertDevice(null);
            Assert.IsNull(result);
        }

        [Test]
        public void ConnectorCreated_UpdateNull_NullReturned()
        {
            var mongo = MongoConnector.Instance;
            var result = mongo.UpdateDevice(null);
            Assert.IsFalse(result);
        }

    }
}
