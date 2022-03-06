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
    /// This class abstracts basic CRUD operations for MongoDB. It is implemented as a 
    /// singleton class, so to instantiate, use this syntax: "var mongo = MongoConnector.Instance;".
    /// </summary>
    public interface IStorageProvider
    {
        /**
         * 
         */
        Task<string> PutFile(IFormFile file);
        Task<string> GetPresignedURL(string fileUid);
        Task<MemoryStream> GetFileStream(string fileUid);
        Task<bool> DeleteFile(string fileUid);
    }
}
