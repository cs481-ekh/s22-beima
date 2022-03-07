using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BEIMA.Backend.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BEIMA.Backend.Test.StorageService
{
    [TestFixture]
    public class AzureStorageTest : UnitTestBase
    {
        private readonly IStorageProvider _storage;

        public AzureStorageTest()
        { 
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            _storage = serviceProivder.GetRequiredService<IStorageProvider>();
        }

        [Test]
        public void SmokeTest()
        {
            Assert.IsNotNull(_storage);
            Assert.IsInstanceOf(typeof(AzureStorageProvider), _storage);
        }

        [Test]
        public async Task GetFileStream_FileExists()
        {
            //Arrange
            IFormFile file;
            var testString = "Hello World";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
            file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
            var fileUid = await _storage.PutFile(file);

            //Act
            var fileStream = await _storage.GetFileStream(fileUid);

            //Assert
            Assert.IsNotNull(fileStream);
            var fileContent = Encoding.UTF8.GetString(fileStream.ToArray());
            Assert.AreEqual(testString, fileContent);
        }

        [Test]
        public async Task GetFileStream_FileNotExists()
        {
            //Arrange
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var stream = await _storage.GetFileStream(fileUid);

            //Assert
            Assert.IsNull(stream);
        }

        [Test]
        public async Task GetPresignedUrl_ObjectExists()
        {
            //Arrange
            IFormFile file;
            var testString = "Hello World";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
            file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
            var fileUid = await _storage.PutFile(file);

            //Act
            var url = await _storage.GetPresignedURL(fileUid);

            //Assert
            Assert.NotNull(url);
        }

        [Test]
        public async Task GetPresignedUrl_FileNotExists()
        {
            //Arranges
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var url = await _storage.GetPresignedURL(fileUid);

            //Assert
            Assert.IsNull(url);
        }

        [Test]
        public async Task PutFile()
        {
            //Arrange
            IFormFile file;
            var testString = "Hello World";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
            file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            //Act
            var fileUid = await _storage.PutFile(file);

            //Assert
            Assert.NotNull(fileUid);
        }

        [Test]
        public async Task DeleteObject_FileExists()
        {
            //Arrange
            IFormFile file;
            var testString = "Hello World";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
            file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            //Act
            var fileUid = await _storage.PutFile(file);
            var preDelExists = await _storage.GetFileExists(fileUid);
            var delRes = await _storage.DeleteFile(fileUid);
            var postDelExists = await _storage.GetFileExists(fileUid);

            //Assert
            Assert.NotNull(fileUid);
            Assert.IsTrue(preDelExists);
            Assert.IsTrue(delRes);
            Assert.IsFalse(postDelExists);
        }

        [Test]
        public async Task DeleteFile_FileNotExists()
        {
            //Arrange
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var result = await _storage.DeleteFile(fileUid);
            var postDelExists = await _storage.GetFileExists(fileUid);

            //Assert
            Assert.IsTrue(result);
            Assert.IsFalse(postDelExists);
        }
    }
}
