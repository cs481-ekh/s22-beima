namespace BEIMA.Backend.AuthService
{
    /// <summary>
    /// Helps to store the instance of the authentication service being used.
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to allow us to switch out the default
    /// authentication service with a mock authentication service for testing.
    /// </remarks>
    public static class AuthenticationDefinition
    {
        /// <summary>
        /// Gets and sets the currently defined instance of the authentication service
        /// </summary>
        public static IAuthenticationService AuthenticationInstance { get; set; } = AuthenticationService.Instance;
    }
}
