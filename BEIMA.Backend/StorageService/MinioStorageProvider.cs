using Microsoft.AspNetCore.Http;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public class MinioStorageProvider : IStorageProvider
    {
        private static MinioClient client;
        private const string bucket = "files";
        public MinioStorageProvider()
        {
            var accessKey = Environment.GetEnvironmentVariable("MinioAccessKey");
            var secretKey = Environment.GetEnvironmentVariable("MinioSecretKey");

            // Will need to have endpoint changed and .WithSSL() added based off of vm configurations
            client = new MinioClient().WithCredentials(accessKey, secretKey).WithEndpoint("localhost",9000);
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

        public async Task<List<string>> GetAllFiles()
        {
            var args = new ListObjectsArgs().WithBucket(bucket);
            var files = await client.ListObjectsAsync(args).ToList();
            var x = 1 + 1;
            return null;

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
