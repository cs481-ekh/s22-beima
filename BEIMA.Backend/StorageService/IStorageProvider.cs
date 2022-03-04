using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.StorageService
{
    public interface IStorageProvider
    {
        Task<List<string>> GetDeviceDocuments(string deviceId);
        Task<List<string>> GetDeviceImages(string deviceId);
        Task<List<string>> GetAllFiles();
        Task PutDeviceDocuments(IFormFileCollection documents);
        Task PutDeviceImages(IFormFileCollection images);
        Task DeleteDeviceDocuments(List<string> documents);
        Task DeleteDeviceImages(List<string> images);
        Task DownloadDeviceFiles(string fileName);
    }
}
