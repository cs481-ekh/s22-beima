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
        private readonly IStorageProvider _storage = SetupStorageProvider();

        private static IStorageProvider SetupStorageProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IStorageProvider, AzureStorageProvider>();
            var serviceProivder = services.BuildServiceProvider();
            return serviceProivder.GetRequiredService<IStorageProvider>();
        }

        [Test]
        public void SmokeTest()
        {
            Assert.That(_storage, Is.Not.Null);
            Assert.That(_storage, Is.InstanceOf(typeof(AzureStorageProvider)));
        }

        [Test]
        public async Task FileExists_GetFileStream_StreamExists()
        {
            //Arrange
            string fileUid;
            var testString = "Hello World";
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
            {
                var file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };

                // Needs to be inside using as closing memory stream won't allow file stream to be openned.
                fileUid = await _storage.PutFile(file);
            }

            //Act
            var fileStream = await _storage.GetFileStream(fileUid);
            var fileContent = Encoding.UTF8.GetString(fileStream.ToArray());

            //Assert
            Assert.That(fileStream, Is.Not.Null);
            Assert.That(testString, Is.EqualTo(fileContent));
        }

        [Test]
        public async Task FileNotExists_GetFileStream_StreamNotExists()
        {
            //Arrange
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var stream = await _storage.GetFileStream(fileUid);

            //Assert
            Assert.That(stream, Is.Null);
        }

        [Test]
        public async Task FileExists_GetPresignedUrl_PresignedUrlExists()
        {
            //Arrange
            string fileUid;
            var testString = "Hello World";
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
            {
                var file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };

                // Needs to be inside using as closing memory stream won't allow file stream to be openned.
                fileUid = await _storage.PutFile(file);
            }

            //Act
            var url = await _storage.GetPresignedURL(fileUid);

            //Assert
            Assert.That(url, Is.Not.Null);
        }

        [Test]
        public async Task FileNotExists_GetPresignedUrl_PresignedUrlNotExists()
        {
            //Arranges
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var url = await _storage.GetPresignedURL(fileUid);

            //Assert
            Assert.That(url, Is.Null);
        }

        [Test]
        public async Task FileNotExists_PutFile_FileUidExists()
        {
            //Arrange
            string fileUid;
            var testString = "Hello World";
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
            {
                var file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };

                //Act
                // Needs to be inside using as closing memory stream won't allow file stream to be openned.
                fileUid = await _storage.PutFile(file);
            }

            //Assert
            Assert.That(fileUid, Is.Not.Null);
        }

        [Test]
        public async Task FileExists_DeleteObject_DeletedTrue()
        {
            //Arrange
            string fileUid;
            var testString = "Hello World";
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
            {
                var file = new FormFile(stream, 0, stream.Length, "testfile", "helloworld.txt")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };

                // Needs to be inside using as closing memory stream won't allow file stream to be openned.
                fileUid = await _storage.PutFile(file);
            }

            //Act
            var preDelExists = await _storage.GetFileExists(fileUid);
            var delRes = await _storage.DeleteFile(fileUid);
            var postDelExists = await _storage.GetFileExists(fileUid);

            //Assert
            Assert.That(fileUid, Is.Not.Null);
            Assert.That(preDelExists, Is.True);
            Assert.That(delRes, Is.True);
            Assert.That(postDelExists, Is.False);
        }

        [Test]
        public async Task FileNotExists_DeleteObject_DeletedTrue()
        {
            //Arrange
            var fileUid = Guid.NewGuid().ToString();

            //Act
            var result = await _storage.DeleteFile(fileUid);
            var postDelExists = await _storage.GetFileExists(fileUid);

            //Assert
            Assert.That(result, Is.True);
            Assert.That(postDelExists, Is.False);
        }
    }
}
