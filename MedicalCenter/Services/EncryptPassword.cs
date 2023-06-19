using BCryptNet = BCrypt.Net.BCrypt;

namespace MedicalCenter.Services
{
    public class EncryptPassword
    {
        public EncryptPassword() { }
        public static string HashPassword(string password)
        {
            // Generate a random salt
            var salt = BCryptNet.GenerateSalt();

            // Hash the password with the salt
            var hashedPassword = BCryptNet.HashPassword(password, salt);

            return hashedPassword;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCryptNet.Verify(password, hashedPassword);
        }
    }
}
