using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public class AzureStorageProvider : IStorageProvider
    {
        private static BlobContainerClient client;

        public AzureStorageProvider()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            client = new BlobContainerClient(connectionString, "files");
        }
  
        public Task DeleteDeviceDocuments(List<string> documents)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceImages(List<string> images)
        {
            throw new NotImplementedException();
        }

        public Task DownloadDeviceFiles(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetAllFiles()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetDeviceDocuments(string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetDeviceImages(string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task PutDeviceDocuments(IFormFileCollection documents)
        {
            throw new NotImplementedException();
        }

        public Task PutDeviceImages(IFormFileCollection images)
        {
            throw new NotImplementedException();
        }
    }
}
