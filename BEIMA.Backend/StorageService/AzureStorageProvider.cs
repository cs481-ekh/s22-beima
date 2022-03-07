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
    public sealed class AzureStorageProvider : IStorageProvider
    {
        private static BlobContainerClient containerClient;

        public AzureStorageProvider()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnection");
            var container = Environment.GetEnvironmentVariable("AzureContainer");
            containerClient = new BlobContainerClient(connectionString, container);
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
                    var client = containerClient.GetBlobClient(fileUid);
                    var response = await client.ExistsAsync();
                    exists = response.Value;

                } while (exists);
                await containerClient.UploadBlobAsync(fileUid, file.OpenReadStream());
                return fileUid;
            } catch (Exception)
            {
                return null;
            }
        }
        public async Task<MemoryStream> GetFileStream(string fileUid)
        {
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
        public Task<string> GetPresignedURL(string fileUid)
        {
            try
            {
                var client = containerClient.GetBlobClient(fileUid);
                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(4),
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                var url = client.GenerateSasUri(sasBuilder);

                return Task.FromResult(url.AbsoluteUri);
            } catch (Exception)
            {
                return null;
            }
           
        }
        public async Task<bool> DeleteFile(string fileUid)
        {
            try
            {
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
