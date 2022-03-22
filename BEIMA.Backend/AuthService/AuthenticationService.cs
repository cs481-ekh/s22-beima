using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.AuthService
{
    /// <summary>
    /// This class implements basic JWT functions such as creating a token from a user
    /// and parsing headers for a token and its claims. It is implemented as a singleton.
    /// </summary>
    public sealed class AuthenticationService : IAuthenticationService
    {
        private static readonly Lazy<AuthenticationService> instance = new(() => new AuthenticationService());

        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        
        private AuthenticationService()
        {
            _algorithm = new HMACSHA256Algorithm();
            _serializer = new JsonNetSerializer();
            _base64Encoder = new JwtBase64UrlEncoder();
            _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
        }

        public static AuthenticationService Instance { get { return instance.Value; } }

        /// <summary>
        /// Creates a JWT token from a passed in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>string encoding of a JWT token</returns>
        public string CreateToken(User user)
        {
            var claims = new Claims()
            {
                Username = user.Username,
                Role = user.Role,
            };
            var secretKey = Environment.GetEnvironmentVariable("JwtKey");
            var token = _jwtEncoder.Encode(claims, secretKey);
            return token;
        }
        /// <summary>
        /// Parses the headers of an HttpRequest for
        /// a JWT and returnes the claims found inside of it
        /// </summary>
        /// <param name="req"></param>
        /// <returns>JWT claims</returns>
        public Claims ParseToken(HttpRequest req)
        {
            // Check if headers or Authorization header are null;
            if (req.Headers == null || !req.Headers.ContainsKey("Authorization"))
            {
                return null;
            }
            string authHeader = req.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                return null;
            }

            try
            {
                // Strip of frontal 'Bearer ' if it exists
                if (authHeader.StartsWith("Bearer "))
                {
                    authHeader = authHeader.Substring(7);
                }

                var secretKey = Environment.GetEnvironmentVariable("JwtKey");
                var claims = new JwtBuilder()
                    .WithAlgorithm(_algorithm)
                    .WithSecret(secretKey)
                    .MustVerifySignature()
                    .Decode<Claims>(authHeader);

                // Verify if claims have expired
                if (claims.Exp <= DateTime.Now.Ticks)
                {
                    return null;
                }

                return claims;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
