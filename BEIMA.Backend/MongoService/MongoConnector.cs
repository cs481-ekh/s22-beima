using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoService
{
    public sealed class MongoConnector
    {
        private readonly MongoClient client = null;
        private static MongoConnector instance = null;
        private readonly string dbName = Environment.GetEnvironmentVariable("DatabaseName");
        private readonly string deviceCollection = Environment.GetEnvironmentVariable("DeviceCollectionName");

        //Private constructor, used for singleton pattern
        private MongoConnector() {

            string credentials;

            //TODO: implement check to see if using local or cloud database
            if (Environment.GetEnvironmentVariable("CurrentEnv") == "dev-local")
            {
                credentials = Environment.GetEnvironmentVariable("LocalMongoConnection");
            } 
            else
            {
                credentials = Environment.GetEnvironmentVariable("AzureCosmosConnection");
            }
            client = new MongoClient(credentials);
        }
        
        //Singleton design pattern
        public static MongoConnector Instance
        {
            get { return instance ??= new MongoConnector(); }
        }

        
    }
}
