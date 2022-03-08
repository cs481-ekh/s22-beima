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
    /// singleton dependancy injected object. See SampleFileStore.cs for example
    /// of how to inject it through the function's constructor
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Puts a file into the the storage bucket
        /// </summary>
        /// <param name="file">Corresponds to a file sent as part of a multipart/form-data post</param>
        /// <returns>Uid of the file created or null if request failed</returns>
        Task<string> PutFile(IFormFile file);
        /// <summary>
        /// Creates a presigned url for the specified file
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>Presigned url or null if request failed</returns>
        Task<string> GetPresignedURL(string fileUid);
        /// <summary>
        /// Gets a stream to the requested file. Used for downloading a file. Will be
        /// used in conjuction with return new FileStreamResult(stream, "application/octet-stream");
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>Memorystream of the target file or null if request failed</returns>
        Task<MemoryStream> GetFileStream(string fileUid);
        /// <summary>
        /// Checks if the specified file exists in storage
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>True if target file exists or false if it doesn't</returns>
        Task<bool> GetFileExists(string fileUid);
        /// <summary>
        /// Deletes the target file from storage
        /// </summary>
        /// <param name="fileUid">Corresponds the target file's uid filename</param>
        /// <returns>True if target file was deleted or false if request failed</returns>
        Task<bool> DeleteFile(string fileUid);
        
    }
}
