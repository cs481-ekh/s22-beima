﻿using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BEIMA.Backend.StorageService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace BEIMA.Backend.Test.StorageService
{
    [TestFixture]
    public class MinioStorageTest : UnitTestBase
    {
        private readonly IStorageProvider _storage = new MinioStorageProvider();

        [Test]
        public void SmokeTest()
        {
            Assert.IsNotNull(_storage);
            Assert.IsInstanceOf(typeof(MinioStorageProvider), _storage);
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
        public async Task NullFileUid_GetFileStream_StreamNotExists()
        {
            //Act
            var stream = await _storage.GetFileStream(null);

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
        public async Task NullFileUid_GetPresignedUrl_PresignedUrlNotExists()
        {
            //Act
            var url = await _storage.GetPresignedURL(null);

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
        public async Task NullFile_PutFile_FileUidExists()
        {
            //Act
            var fileUid = await _storage.PutFile(null);

            //Assert
            Assert.That(fileUid, Is.Null);
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

        [Test]
        public async Task Null_DeleteObject_DeletedTrue()
        {
            //Act
            var result = await _storage.DeleteFile(null);

            //Assert
            Assert.That(result, Is.True);

        }
    }
}
