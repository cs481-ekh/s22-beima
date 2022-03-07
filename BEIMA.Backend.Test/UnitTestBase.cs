using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace BEIMA.Backend.Test
{
    public class UnitTestBase
    {
        private static string? dbName;
        private static string? devicesName;

        class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string>? Values { get; set; }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
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

            // Prevents the environment variable getting changed when running multiple test classes at the same time
            if(dbName == null && devicesName == null)
            {
                dbName = "beima-test" + Guid.NewGuid().ToString();
                devicesName = "devices-test" + Guid.NewGuid().ToString();
            }
            Environment.SetEnvironmentVariable("DatabaseName", dbName);
            Environment.SetEnvironmentVariable("DeviceCollectionName", devicesName);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            string? credentials;

            if (Environment.GetEnvironmentVariable("CurrentEnv") == "dev-local")
            {
                credentials = Environment.GetEnvironmentVariable("LocalMongoConnection");
            }
            else
            {
                credentials = Environment.GetEnvironmentVariable("AzureCosmosConnection");
            }
            var client = new MongoClient(credentials);
            client.DropDatabase(dbName);
        }
    }
}
