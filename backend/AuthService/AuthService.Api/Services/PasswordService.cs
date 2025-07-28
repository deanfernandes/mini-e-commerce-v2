using System.Security.Cryptography;
using System.Text;

namespace AuthService.Api.Services
{
    public class PasswordService
    {
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            var hashOfInput = HashPassword(plainPassword);
            return hashOfInput == hashedPassword;
        }
    }
}