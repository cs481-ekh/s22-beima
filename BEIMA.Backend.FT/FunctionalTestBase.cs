using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace BEIMA.Backend.FT
{
    /// <summary>
    /// Base FT class that all FT classes should inherit from.
    /// </summary>
    public class FunctionalTestBase
    {
        public BeimaClient TestClient = new BeimaClient("http://localhost:7071");
        public string? dbName;
        public string? devicesName;
        private MongoClient? dbClient;

        class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string>? Values { get; set; }
        }

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            // HTTP CLIENT SETUP

            if (TestClient != null)
            {
                TestClient.Dispose();
            }
            // Uncomment for Azure server:
            // BeimaClient Client = new BeimaClient("https://beima-service.azurewebsites.net");
            TestClient = new BeimaClient("http://localhost:7071");

            // DATABASE SETUP

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

                if (settings != null && settings.Values != null)
                {
                    foreach (var setting in settings.Values)
                    {
                        Environment.SetEnvironmentVariable(setting.Key, setting.Value);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            dbName = "beima";
            devicesName = "devices";

            Environment.SetEnvironmentVariable("DatabaseName", dbName);
            Environment.SetEnvironmentVariable("DeviceCollectionName", devicesName);
            Environment.SetEnvironmentVariable("CurrentEnv", "dev-cloud");

            string? credentials;

            if (Environment.GetEnvironmentVariable("CurrentEnv") == "dev-local")
            {
                credentials = Environment.GetEnvironmentVariable("LocalMongoConnection");
            }
            else
            {
                credentials = Environment.GetEnvironmentVariable("AzureCosmosConnection");
            }
            dbClient = new MongoClient(credentials);
        }

        [OneTimeTearDown]
        public void BaseOneTimeTearDown()
        {
            TestClient.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            dbClient?.DropDatabase(dbName);
        }
    }
}
