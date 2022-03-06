using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public sealed class AzureStorageProvider : IStorageProvider
    {
        private static BlobContainerClient client;

        public AzureStorageProvider()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            client = new BlobContainerClient(connectionString, "files");
        }
        public Task<string> PutFile(IFormFile file)
        {
            throw new NotImplementedException();
        }
        public Task<MemoryStream> GetFileStream(string fileUid)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetPresignedURL(string fileUid)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteFile(string fileUid)
        {
            throw new NotImplementedException();
        }
    }
}
