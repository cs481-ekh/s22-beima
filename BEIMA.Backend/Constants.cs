namespace BEIMA.Backend
{
    /// <summary>
    /// Class for storing commonly used constant values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The max character length for a default form field.
        /// </summary>
        public const int MAX_CHARACTER_LENGTH = 1024;

        /// <summary>
        /// Regex to check that a password meets the minimum password requirements.
        /// </summary>
        public const string PASSWORD_REGEX = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";

        /// <summary>
        /// String that represents the admin role.
        /// </summary>
        public const string ADMIN_ROLE = "admin";
    }
}
