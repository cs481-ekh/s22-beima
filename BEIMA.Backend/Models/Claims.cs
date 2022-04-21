using System;

namespace BEIMA.Backend.Models
{
    /// <summary>
    /// Object representation of the claims that are
    /// stored in the JWT token given out to authenticate users
    /// </summary>
    public class Claims
    {
        public static readonly string Issuer = "Beima";
        public static readonly string Subject = "User";

        // Registered claims
        public string Iss { get; } = Issuer;
        public string Sub { get; } = Subject;
        public long Exp { get; } = DateTime.Now.AddDays(7).Ticks;

        // Public claims
        public string Username { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
    }
}
