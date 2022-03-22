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
    public interface IAuthenticationService
    {
        string CreateToken(User user);
        Claims ParseToken(HttpRequest req);        
    }
}
