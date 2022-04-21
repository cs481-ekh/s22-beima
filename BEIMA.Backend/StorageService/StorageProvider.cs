﻿using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public class StorageProvider : IStorageProvider
    {
        // Contains instance variables
        private readonly IStorageProvider _storage = null;
        private static readonly Lazy<StorageProvider> instance = new(() => new StorageProvider());

        private StorageProvider()
        {
            var environment = Environment.GetEnvironmentVariable("CurrentEnv");

            if (environment == "dev-local" || environment == "deploy")
            {
                _storage = new MinioStorageProvider();
            }
            else
            {
                _storage = new AzureStorageProvider();
            }
        }

        public static StorageProvider Instance { get { return instance.Value; } }

        public Task<string> PutFile(IFormFile file)
        {
            return _storage.PutFile(file);
        }

        public Task<string> GetPresignedURL(string fileUid)
        {
            return _storage.GetPresignedURL(fileUid);
        }

        public Task<MemoryStream> GetFileStream(string fileUid)
        {
            return _storage.GetFileStream(fileUid);
        }

        public Task<bool> GetFileExists(string fileUid)
        {
            return _storage.GetFileExists(fileUid);
        }

        public Task<bool> DeleteFile(string fileUid)
        {
            return _storage.DeleteFile(fileUid);
        }
    }
}
