using BEIMA.Backend.AuthFunctions;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BEIMA.Backend.Test.AuthFunctions
{
    [TestFixture]
    public class LoginTest : UnitTestBase
    {
        [Test]
        public async Task NullPostBody_Login_ReturnsNull()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testNullLoginRequest;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var token = ((ObjectResult)await Login.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(token);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Never));
            Assert.That(token, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public async Task InvalidParameters_Login_ReturnsNull()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testNullKeysLoginRequest;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var token = ((ObjectResult) await Login.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(token);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Never));
            Assert.That(token, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public async Task ValidParametersNoFilteredUser_Login_ReturnsNull()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(new List<BsonDocument>())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testValidKeysLoginRequest;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var token = ((ObjectResult)await Login.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(token);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.That(token, Is.EqualTo("Invalid credentials."));
        }

        [Test]
        public async Task ValidParametersFilteredUserWrongPassword_Login_ReturnsNull()
        {
            var hashed = BCryptNet.HashPassword("Invalid");
            User user = new User()
            {
                Username = "User",
                Password = hashed
            };

            var filteredUsers = new List<BsonDocument>()
            {
                user.ToBsonDocument()
            };

            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(filteredUsers)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testValidKeysLoginRequest;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var token = ((ObjectResult)await Login.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(token);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.That(token, Is.EqualTo("Invalid credentials."));
        }


        [Test]
        public async Task ValidParametersMatchingUser_Login_ReturnsToken()
        {
            var hashed = BCryptNet.HashPassword("Pass");
            User user = new User()
            {
                Id = ObjectId.GenerateNewId(),
                Username = "User",
                Password = hashed,
                Role = "Admin",
                FirstName = "First",
                LastName = "Last",
                LastModified = new UserLastModified()
                {
                    Date = DateTime.Now,
                    User = "anon"
                }
            };

            var filteredUsers = new List<BsonDocument>()
            {
                user.ToBsonDocument()
            };

            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()))
                  .Returns(filteredUsers)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            var body = TestData._testValidKeysLoginRequest;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var token = ((ObjectResult)await Login.Run(request, logger)).Value?.ToString();

            // ASSERT
            Assert.IsNotNull(token);
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            var claims = new JwtBuilder().Decode<Claims>(token);
            Assert.That(claims.Username, Is.EqualTo(user.Username));
            Assert.That(claims.Role, Is.EqualTo(user.Role));
            Assert.That(claims.Id.Contains(user.Id.ToString()));

            // Expiration of token is 7 days after creation. Should be 6 days 23 hours 59min xx seconds greater then datetime now
            var nowPlus6 = DateTime.Now.AddDays(6).AddHours(23).AddMinutes(59).Ticks;
            var nowPlus7 = DateTime.Now.AddDays(7).Ticks;

            Assert.That(claims.Exp, Is.GreaterThan(nowPlus6));
            Assert.That(claims.Exp, Is.LessThan(nowPlus7));
            Assert.That(claims.Sub, Is.EqualTo("User"));
            Assert.That(claims.Iss, Is.EqualTo("Beima"));
        }

    }
}
