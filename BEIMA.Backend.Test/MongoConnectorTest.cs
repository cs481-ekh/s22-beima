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
            string basePath = Path.GetFullPath(@"..\..\..\..\BEIMA.Backend");
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void InitialState_Action_ExpectedResult()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ConnectorNotCreated_CallConstructorFiveTimes_FiveInstancesAreEqual()
        {
            var connector1 = MongoConnector.Instance;
            var connector2 = MongoConnector.Instance;
            var connector3 = MongoConnector.Instance;
            var connector4 = MongoConnector.Instance;
            var connector5 = MongoConnector.Instance;
            Assert.IsTrue(connector1 == connector2);
            Assert.IsTrue(connector1 == connector3);
            Assert.IsTrue(connector1 == connector4);
            Assert.IsTrue(connector1 == connector5);
        }

        [Test]
        public void ConnectorCreated_GetDeviceGivenDeviceId_DeviceDocumentReturned()
        {
            var mongo = MongoConnector.Instance;
            var doc = mongo.GetDevice(new ObjectId("61f4882f42dab933544849d3"));
            Assert.IsNotNull(doc);
            Assert.IsTrue(doc is BsonDocument);
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
            Assert.IsTrue(result is ObjectId);
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
            Assert.IsTrue(insertResult is ObjectId);

            //Delete device
            bool deleteResult = false;
            if(insertResult != null)
            {
                deleteResult = mongo.DeleteDevice((ObjectId) insertResult);
            }
            Assert.IsTrue(deleteResult);
        }



    }
}
