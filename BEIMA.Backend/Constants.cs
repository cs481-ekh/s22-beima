namespace BEIMA.Backend
{
    public static class Constants
    {
        public const int MAX_CHARACTER_LENGTH = 1024;
        public const string PASSWORD_REGEX = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        public const string ADMIN_ROLE = "admin";
    }
}
