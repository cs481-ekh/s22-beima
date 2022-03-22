using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.AuthService
{
    public static class AuthenticationDefinition
    {
        public static IAuthenticationService AuthticationInstance { get; set; } = AuthenticationService.Instance;
    }
}
