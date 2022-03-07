using Microsoft.AspNetCore.Http;
using Minio;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public sealed class MinioStorageProvider : IStorageProvider
    {
        private static MinioClient client;
        private string bucket;
        public MinioStorageProvider()
        {
            var accessKey = Environment.GetEnvironmentVariable("MinioAccessKey");
            var secretKey = Environment.GetEnvironmentVariable("MinioSecretKey");
            bucket = Environment.GetEnvironmentVariable("MinioBucket");

            // Will need to have endpoint changed and .WithSSL() added based off of vm configurations
            client = new MinioClient().WithCredentials(accessKey, secretKey).WithEndpoint("localhost",9000);
        }

        public async Task<string> PutFile(IFormFile file)
        {
            try
            {
                string fileUid = null;
                var extension = Path.GetExtension(file.FileName);
                var exists = false;
                do
                {
                    var uid = Guid.NewGuid().ToString();
                    fileUid = uid + extension;
                    var statArgs = new StatObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(fileUid);

                    // Check if object exists. Will throw exception if it doesn't. There is no objectExists method in minio
                    try
                    {
                        await client.StatObjectAsync(statArgs);
                        exists = true;
                    } catch (ObjectNotFoundException)
                    {
                        exists = false;
                    }
                } while (exists);
                var stream = file.OpenReadStream();
                var putArgs = new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid)
                    .WithContentType(file.ContentType)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length);
                await client.PutObjectAsync(putArgs);
                return fileUid;
            } catch (Exception)
            {
                return null;
            } 
        }

        public async Task<string> GetPresignedURL(string fileUid)
        {
            try
            {
                var uriArgs = new PresignedGetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid)
                    .WithExpiry(60 * 60 * 4);
                var uri = await client.PresignedGetObjectAsync(uriArgs);
                return uri;
            } catch (Exception)
            {
                return null;
            }
        }

        public async Task<MemoryStream> GetFileStream(string fileUid)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var getArgs = new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(ms);
                        ms.Position = 0;
                    });
                    
                await client.GetObjectAsync(getArgs);
                return ms;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteFile(string fileUid)
        {
            try
            {
                var delArgs = new RemoveObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid);
                await client.RemoveObjectAsync(delArgs);
                return true;
            } catch (Exception)
            {
                return false;
            }
        }
    }
}
