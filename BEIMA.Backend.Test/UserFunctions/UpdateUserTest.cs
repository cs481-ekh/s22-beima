using BEIMA.Backend.UserFunctions;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using static BEIMA.Backend.Test.RequestFactory;
using MongoDB.Driver;
using System.Collections.Generic;
using BEIMA.Backend.AuthService;
using Microsoft.AspNetCore.Http;
using BEIMA.Backend.Models;

namespace BEIMA.Backend.Test.UserFunctions
{
    [TestFixture]
    public class UpdateUserTest : UnitTestBase
    {
        [Test]
        public async Task IdNotInDatabase_UpdateUser_ReturnsNotFound()
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            var testId = "abcdef123456789012345678";
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateUser(It.Is<BsonDocument>(u => u["_id"].AsObjectId.ToString().Equals(testId))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateUserWithPassword);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(NotFoundObjectResult)));
            Assert.That(((NotFoundObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            Assert.That(((NotFoundObjectResult)response).Value, Is.EqualTo("User could not be found."));
        }

        [TestCase("")]
        [TestCase("xxx")]
        [TestCase("123")]
        [TestCase("skvmrighs7392mdkfp01mdki")]
        public async Task InvalidId_UpdateUser_ReturnsInvalidId(string id)
        {
            // ARRANGE
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.UpdateUser(It.Is<BsonDocument>(bd => bd["_id"].AsObjectId.ToString().Equals(id))))
                  .Returns<BsonDocument>(null)
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var request = CreateHttpRequest(RequestMethod.POST, body: TestData._testUpdateUserWithPassword);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, id, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value, Is.EqualTo("Invalid Id."));
        }

        [Test]
        public async Task ExistingUser_UpdateUserWithPassword_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var user = new User(new ObjectId(testId), "user.name", "updatedPassword123!", "Aaron", "Doe", "user");
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateUser(It.IsAny<BsonDocument>()))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUpdateUserWithPassword;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var userResponse = (User)((OkObjectResult)response).Value;
            Assert.That(userResponse.Username, Is.EqualTo(user.Username));
            Assert.That(userResponse.Password, Is.EqualTo(string.Empty));
            Assert.That(userResponse.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(userResponse.LastName, Is.EqualTo(user.LastName));
            Assert.That(userResponse.Role, Is.EqualTo(user.Role));
        }

        [Test]
        public async Task ExistingUser_UpdateUserWithoutPassword_ReturnsOKResponse()
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var user = new User(new ObjectId(testId), "new.username", string.Empty, "Pan", "Quartz", "user");
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.UpdateUser(It.IsAny<BsonDocument>()))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUpdateUserWithoutPassword;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Once));

            Assert.IsNotNull(response);
            Assert.That(response, Is.TypeOf(typeof(OkObjectResult)));
            Assert.That(((OkObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var userResponse = (User)((OkObjectResult)response).Value;
            Assert.That(userResponse.Username, Is.EqualTo(user.Username));
            Assert.That(userResponse.Password, Is.EqualTo(user.Password));
            Assert.That(userResponse.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(userResponse.LastName, Is.EqualTo(user.LastName));
            Assert.That(userResponse.Role, Is.EqualTo(user.Role));
        }

        [TestCase("a")]
        [TestCase("Abc1!")]
        [TestCase("abcdefgh")]
        [TestCase("abcdefg1!")]
        [TestCase("Abcdefg!")]
        [TestCase("Abcdefg1")]
        [TestCase("ABCDEFG1!")]
        public async Task ExistingUser_UpdateUserWithBadPassword_ReturnsBadRequestResponse(string badPassword)
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var user = new User(new ObjectId(testId), "user.name", "oldPassword1!", "Aaron", "Doe", "user");
            user.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(user.GetBsonDocument())
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUserBadPassword.Replace("---", badPassword);
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf(typeof(BadRequestObjectResult)));
            Assert.That(((BadRequestObjectResult)response).StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(((BadRequestObjectResult)response).Value?.ToString(),
                        Is.EqualTo("Password is invalid. Password must be at least 8 characters and contain at least an uppercase letter, a number, and a special character."));
        }

        [TestCase("user.name")]
        [TestCase("testUser")]
        [TestCase("12345")]
        [TestCase("true")]
        [TestCase("a")]
        public async Task ExistingUser_UpdateUserWithDuplicateUsername_ReturnsConflictObjectResult(string username)
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";

            var existingUser = new User(ObjectId.GenerateNewId(), username, "ThisIsAPassword1!", "Alex", "Smith", "user");
            var updateUser = new User(new ObjectId(testId), "someOtherUsernameNotInUse", "ThisIsAPassword1!", "Alex", "Smith", "user");
            existingUser.SetLastModified(DateTime.UtcNow, "Anonymous");
            updateUser.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(updateUser.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.Is<FilterDefinition<BsonDocument>>(filter => filter != null)))
                  .Returns(new List<BsonDocument> { existingUser.GetBsonDocument() })
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUserDuplicateUsername.Replace("---", username);
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf(typeof(ConflictObjectResult)));
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
            Assert.That(response.Value?.ToString(), Is.EqualTo("Username already exists."));
        }

        [TestCase("user.name", "User.Name")]
        [TestCase("testUser", "Testuser")]
        [TestCase("12345", "12345")]
        [TestCase("true", "tRue")]
        [TestCase("a", "A")]
        public async Task ExistingUser_UpdateUserWithDuplicateUsernameCapitalized_ReturnsConflictObjectResult(string username, string capUsername)
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            var existingUser = new User(ObjectId.GenerateNewId(), username, "ThisIsAPassword1!", "Alex", "Smith", "user");
            var updateUser = new User(new ObjectId(testId), "someOtherUsernameNotInUse", "ThisIsAPassword1!", "Alex", "Smith", "user");
            existingUser.SetLastModified(DateTime.UtcNow, "Anonymous");
            updateUser.SetLastModified(DateTime.UtcNow, "Anonymous");

            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            mockDb.Setup(mock => mock.GetUser(It.Is<ObjectId>(oid => oid == new ObjectId(testId))))
                  .Returns(updateUser.GetBsonDocument())
                  .Verifiable();
            mockDb.Setup(mock => mock.GetFilteredUsers(It.Is<FilterDefinition<BsonDocument>>(filter => filter != null)))
                  .Returns(new List<BsonDocument> { existingUser.GetBsonDocument() })
                  .Verifiable();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(new Claims { Role = Constants.ADMIN_ROLE, Username = "Bob" })
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUserDuplicateUsername.Replace("---", capUsername);
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.TypeOf(typeof(ConflictObjectResult)));
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
            Assert.That(response.Value?.ToString(), Is.EqualTo("Username already exists."));
        }

        [TestCaseSource(nameof(ClaimsFactory))]
        public async Task InvalidCredentials_UpdateUser_ReturnsUnauthorized(Claims claim)
        {
            // ARRANGE
            var testId = "abcdef123456789012345678";
            Mock<IMongoConnector> mockDb = new Mock<IMongoConnector>();
            MongoDefinition.MongoInstance = mockDb.Object;

            Mock<IAuthenticationService> mockAuth = new Mock<IAuthenticationService>();
            mockAuth.Setup(mock => mock.ParseToken(It.IsAny<HttpRequest>()))
                .Returns(claim)
                .Verifiable();
            AuthenticationDefinition.AuthenticationInstance = mockAuth.Object;

            var body = TestData._testUpdateUserWithPassword;
            var request = CreateHttpRequest(RequestMethod.POST, body: body);
            var logger = (new LoggerFactory()).CreateLogger("Testing");

            // ACT
            var response = (ObjectResult)await UpdateUser.Run(request, testId, logger);

            // ASSERT
            Assert.DoesNotThrow(() => mockAuth.Verify(mock => mock.ParseToken(It.IsAny<HttpRequest>()), Times.Once));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetUser(It.IsAny<ObjectId>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.GetFilteredUsers(It.IsAny<FilterDefinition<BsonDocument>>()), Times.Never));
            Assert.DoesNotThrow(() => mockDb.Verify(mock => mock.UpdateUser(It.IsAny<BsonDocument>()), Times.Never));

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(response.Value, Is.EqualTo("Invalid credentials."));
        }

        private static IEnumerable<Claims?> ClaimsFactory()
        {
            yield return null;
            yield return new Claims { Role = "nonadmin", Username = "Bob" };
        }
    }
}
