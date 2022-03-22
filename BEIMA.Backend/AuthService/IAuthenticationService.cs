using BEIMA.Backend.Models;
using BEIMA.Backend.MongoService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.AuthService
{
    /// <summary>
    /// This interface abstracts basic JWT operations.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Creates a JWT token from a passed in user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>string encoding of a JWT token</returns>
        string CreateToken(User user);
        /// <summary>
        /// Parses the headers of an HttpRequest for
        /// a JWT and returnes the claims found inside of it
        /// </summary>
        /// <param name="req"></param>
        /// <returns>JWT claims</returns>
        Claims ParseToken(HttpRequest req);        
    }
}
