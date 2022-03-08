using Microsoft.AspNetCore.Http;
using Minio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    /// <summary>
    /// This class abstracts basic file storage operations. It is implemented as a 
    /// singleton dependancy injected object that uses Minio Storage. 
    /// </summary>
    public sealed class MinioStorageProvider : IStorageProvider
    {
        private static MinioClient client;
        private string bucket;

        /// <summary>
        /// Constructor for the MinioStorageProvider
        /// </summary>
        public MinioStorageProvider()
        {
            var accessKey = Environment.GetEnvironmentVariable("MinioAccessKey");
            var secretKey = Environment.GetEnvironmentVariable("MinioSecretKey");
            bucket = Environment.GetEnvironmentVariable("MinioBucket");

            // Will need to have endpoint changed and .WithSSL() added based off of vm configurations
            client = new MinioClient().WithCredentials(accessKey, secretKey).WithEndpoint("localhost",9000);
        }

        /// <summary>
        /// Puts a file into the the storage bucket
        /// </summary>
        /// <param name="file">Corresponds to a file sent as part of a multipart/form-data post</param>
        /// <returns>Uid of the file created or null if request failed</returns>
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
                    exists = await GetFileExists(fileUid);
                } while (exists);

                var stream = file.OpenReadStream();
                var putArgs = new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid)
                    .WithContentType(file.ContentType)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length);

                var buckets = await client.ListBucketsAsync();
                Console.Error.WriteLine(buckets.Buckets.Count);
                await client.PutObjectAsync(putArgs);
                return fileUid;
            } catch (Exception e)
            {
                Console.Error.WriteLine(bucket);
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return null;
            } 
        }

        /// <summary>
        /// Creates a presigned url for the specified file
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>Presigned url or null if request failed</returns>
        public async Task<string> GetPresignedURL(string fileUid)
        {
            try
            {
                var exists = await GetFileExists(fileUid);
                if (!exists)
                {
                    return null;
                }
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

        /// <summary>
        /// Gets a stream to the requested file. Used for downloading a file. Will be
        /// used in conjuction with return new FileStreamResult(stream, "application/octet-stream");
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>Memorystream of the target file or null if request failed</returns>
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

        /// <summary>
        /// Checks if the specified file exists in storage
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>True if target file exists or false if it doesn't</returns>
        public async Task<bool> GetFileExists(string fileUid)
        {
            var statArgs = new StatObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(fileUid);
            try
            {
                // Checks if object exists. Will throw exception if it doesn't. There is no objectExists method in minio
                await client.StatObjectAsync(statArgs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the target file from storage
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>True if target file was deleted or false if request failed</returns>
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
