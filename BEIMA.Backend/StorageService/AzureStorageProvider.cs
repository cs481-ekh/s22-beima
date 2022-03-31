using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    /// <summary>
    /// This class abstracts basic file storage operations. It is implemented as a 
    /// singleton dependancy injected object that uses Azure Storage. 
    /// </summary>
    public sealed class AzureStorageProvider : IStorageProvider
    {
        private static BlobContainerClient containerClient;

        /// <summary>
        /// Constructor for the MinioStorageProvider
        /// </summary>
        public AzureStorageProvider()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            var container = Environment.GetEnvironmentVariable("AzureContainer");
            containerClient = new BlobContainerClient(connectionString, container);
        }

        /// <summary>
        /// Puts a file into the the storage bucket
        /// </summary>
        /// <param name="file">Corresponds to a file sent as part of a multipart/form-data post</param>
        /// <returns>Uid of the file created or null if request failed</returns>
        public async Task<string> PutFile(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

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

                var client = containerClient.GetBlobClient(fileUid);
                using (Stream stream = file.OpenReadStream())
                {
                    await client.UploadAsync(stream);
                }                   
                return fileUid;
            } catch (Exception)
            {
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
            if (string.IsNullOrEmpty(fileUid))
            {
                return null;
            }

            try
            {
                var exists = await GetFileExists(fileUid);
                if (!exists)
                {
                    return null;
                }
                var client = containerClient.GetBlobClient(fileUid);
                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(4),
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                var url = client.GenerateSasUri(sasBuilder);

                return url.AbsoluteUri;
            }
            catch (Exception)
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
            if (string.IsNullOrEmpty(fileUid))
            {
                return null;
            }

            try
            {
                var client = containerClient.GetBlobClient(fileUid);
                var ms = new MemoryStream();
                var stream = await client.OpenReadAsync();
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms;

            } catch (Exception)
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
            if (string.IsNullOrEmpty(fileUid))
            {
                return false;
            }

            try
            {
                var client = containerClient.GetBlobClient(fileUid);
                var response = await client.ExistsAsync();
                return response.Value;
            } catch (Exception)
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
            if (string.IsNullOrEmpty(fileUid))
            {
                return true;
            }

            try
            {
                var exists = await GetFileExists(fileUid);
                if (!exists)
                {
                    return true;
                }
                var client = containerClient.GetBlobClient(fileUid);
                var result = await client.DeleteIfExistsAsync();
                return result.Value;
            } catch(Exception)
            {
                return false;
            }
        }
    }
}
