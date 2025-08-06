using AuthService.Api.Services;

namespace AuthService.Tests
{
    public class PasswordServiceTests
    {
        [Fact]
        public void HashPassword_ShouldCreateValidHash_ThatVerifies()
        {
            string password = "MySecret123";

            string hash = PasswordService.HashPassword(password);

            Assert.False(string.IsNullOrEmpty(hash));
            Assert.True(PasswordService.VerifyPassword(password, hash));
        }

        [Fact]
        public void HashPassword_ForDifferentInput_ShouldReturnDifferentHash()
        {
            string password1 = "PasswordOne";
            string password2 = "PasswordTwo";

            string hash1 = PasswordService.HashPassword(password1);
            string hash2 = PasswordService.HashPassword(password2);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_ShouldReturnValidBCryptHashFormat()
        {
            string password = "AnotherPassword";

            string hash = PasswordService.HashPassword(password);

            Assert.False(string.IsNullOrEmpty(hash));
            Assert.StartsWith("$2", hash);
            Assert.Contains("$", hash);
            Assert.True(hash.Length >= 60);
        }

        [Fact]
        public void VerifyPassword_ForMatchingPasswordAndHash_ShouldReturnTrue()
        {
            var password = "TestPass!";
            var hashed = PasswordService.HashPassword(password);

            var result = PasswordService.VerifyPassword(password, hashed);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ForNonMatchingPassword_ShouldReturnFalse()
        {
            var password = "TestPass!";
            var wrongPassword = "WrongPass!";
            var hashed = PasswordService.HashPassword(password);

            var result = PasswordService.VerifyPassword(wrongPassword, hashed);

            Assert.False(result);
        }
    }
}