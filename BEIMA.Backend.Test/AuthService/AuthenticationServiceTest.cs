using BEIMA.Backend.AuthService;
using MongoDB.Bson;
using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using NUnit.Framework;
using static BEIMA.Backend.Test.RequestFactory;

namespace BEIMA.Backend.Test.AuthService
{
    [TestFixture]
    public class AuthenticationServiceTest : UnitTestBase
    {
        [Test]
        public void ServiceNotCreated_CallConstructorFiveTimes_FiveInstancesAreEqual()
        {
            //Tests to make sure singleton is working
            var authSerivce1 = AuthenticationService.Instance;
            var authSerivce2 = AuthenticationService.Instance;
            var authSerivce3 = AuthenticationService.Instance;
            var authSerivce4 = AuthenticationService.Instance;
            var authSerivce5 = AuthenticationService.Instance;
            Assert.That(authSerivce1, Is.EqualTo(authSerivce2));
            Assert.That(authSerivce1, Is.EqualTo(authSerivce3));
            Assert.That(authSerivce1, Is.EqualTo(authSerivce4));
            Assert.That(authSerivce1, Is.EqualTo(authSerivce5));
        }

        [Test]
        public void HttpRequestNoHeaders_ParseToken_NoClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            var request = CreateHttpRequest(RequestMethod.GET);

            var claims = authService.ParseToken(request);
            Assert.That(claims, Is.Null);
        }

        [Test]
        public void MultiPartHttpRequestNoHeaders_ParseToken_NoClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            var request = CreateMultiPartHttpRequest("");

            var claims = authService.ParseToken(request);
            Assert.That(claims, Is.Null);
        }

        [Test]
        public void HttpRequestWithHeaders_ParseToken_ClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            User user = new User()
            {
                Username = "username",
                Role = "role",
                Id = new ObjectId("111111111111111111111111")
            };

            var token = authService.CreateToken(user);            
            var request = CreateHttpRequest(RequestMethod.GET, authToken: token);

            var claims = authService.ParseToken(request);
            Assert.That(claims, Is.Not.Null);
            Assert.That(claims.Username, Is.EqualTo(user.Username));
            Assert.That(claims.Role, Is.EqualTo(user.Role));
            Assert.That(claims.Sub, Is.EqualTo(Claims.Subject));
            Assert.That(claims.Iss, Is.EqualTo(Claims.Issuer));
            Assert.That(claims.Id.Contains(user.Id.ToString()));
        }

        [Test]
        public void MultiPartHttpRequestWithHeaders_ParseToken_ClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            User user = new User()
            {
                Username = "username",
                Role = "role",
                Id = new ObjectId("111111111111111111111111")
            };

            var token = authService.CreateToken(user);
            var request = CreateMultiPartHttpRequest("", authToken: token);

            var claims = authService.ParseToken(request);
            Assert.That(claims, Is.Not.Null);
            Assert.That(claims.Username, Is.EqualTo(user.Username));
            Assert.That(claims.Role, Is.EqualTo(user.Role));
            Assert.That(claims.Sub, Is.EqualTo(Claims.Subject));
            Assert.That(claims.Iss, Is.EqualTo(Claims.Issuer));
            Assert.That(claims.Id.Contains(user.Id.ToString()));
        }

        [Test]
        public void HttpRequestWithHeader_ModifyToken_ParseToken_NoClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            User user = new User()
            {
                Username = "username",
                Role = "role",
                Id = new ObjectId("111111111111111111111111")
            };

            var token = authService.CreateToken(user);
            var requestOne = CreateHttpRequest(RequestMethod.GET, authToken: token);

            var claims = authService.ParseToken(requestOne);
            claims.Role = "admin";

            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var base64Encoder = new JwtBase64UrlEncoder();
            var jwtEncoder = new JwtEncoder(algorithm, serializer, base64Encoder);

            var modifiedToken = jwtEncoder.Encode(claims, "invalidSecretKey");

            var requestTwo = CreateHttpRequest(RequestMethod.GET, authToken: modifiedToken);

            var invalidClaims = authService.ParseToken(requestTwo);
            Assert.That(invalidClaims, Is.Null);
        }

        [Test]
        public void MultiParttHttpRequestWithHeader_ModifyToken_ParseToken_NoClaimsReturned()
        {
            var authService = AuthenticationService.Instance;
            User user = new User()
            {
                Username = "username",
                Role = "role",
                Id = new ObjectId("111111111111111111111111")
            };

            var token = authService.CreateToken(user);
            var requestOne = CreateMultiPartHttpRequest("", authToken: token);

            var claims = authService.ParseToken(requestOne);
            Assume.That(claims.Role, Is.EqualTo("role"));
            claims.Role = "admin";

            var algorithm = new HMACSHA256Algorithm();
            var serializer = new JsonNetSerializer();
            var base64Encoder = new JwtBase64UrlEncoder();
            var jwtEncoder = new JwtEncoder(algorithm, serializer, base64Encoder);

            var modifiedToken = jwtEncoder.Encode(claims, "invalidSecretKey");

            var requestTwo = CreateMultiPartHttpRequest("", authToken: modifiedToken);

            var invalidClaims = authService.ParseToken(requestTwo);
            Assert.That(invalidClaims, Is.Null);
        }
    }
}
