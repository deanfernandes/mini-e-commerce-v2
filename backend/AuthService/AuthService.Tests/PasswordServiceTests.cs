using AuthService.Api.Services;

namespace AuthService.Tests
{
    public class PasswordServiceTests
    {
        [Fact]
        public void HashPassword_ForSameInput_ShouldReturnConsistentHash()
        {
            string password = "MySecret123";

            string hash1 = PasswordService.HashPassword(password);
            string hash2 = PasswordService.HashPassword(password);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.Equal(hash1, hash2);
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
        public void HashPassword_ShouldReturnBase64EncodedString()
        {
            string password = "AnotherPassword";

            string hash = PasswordService.HashPassword(password);

            byte[] decoded = Convert.FromBase64String(hash);
            Assert.NotNull(decoded);
            Assert.True(decoded.Length > 0);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_ForMatchingPasswordAndHash()
        {
            var password = "TestPass!";
            var hashed = PasswordService.HashPassword(password);

            var result = PasswordService.VerifyPassword(password, hashed);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForNonMatchingPassword()
        {
            var password = "TestPass!";
            var wrongPassword = "WrongPass!";
            var hashed = PasswordService.HashPassword(password);

            var result = PasswordService.VerifyPassword(wrongPassword, hashed);

            Assert.False(result);
        }
    }
}