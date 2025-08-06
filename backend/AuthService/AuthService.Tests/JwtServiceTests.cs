using AuthService.Api.Models;
using AuthService.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuthService.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = "YourSuperSecretKey1234567890123456",
                Issuer = "your-issuer",
                Audience = "your-audience",
                ExpiresInMinutes = 60
            };

            _jwtService = new JwtService(jwtSettings);
        }

        [Fact]
        public void GenerateLoginJwt_ReturnsToken()
        {
            var user = new User
            {
                Email = "test@example.com",
                Username = "tester"
            };

            var token = _jwtService.GenerateLoginJwt(user);

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateEmailConfirmationJwt_ReturnsToken()
        {
            var user = new User
            {
                Email = "test@example.com",
                Username = "tester"
            };

            var token = _jwtService.GenerateEmailConfirmationJwt(user);

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void ValidateEmailConfirmationJwt_ValidToken_ReturnsClaimsPrincipal()
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser"
            };
            var token = _jwtService.GenerateEmailConfirmationJwt(user);

            var principal = _jwtService.ValidateEmailConfirmationJwt(token);

            Assert.NotNull(principal);
            Assert.Equal("email_confirmation", principal.FindFirst("token_type")?.Value);
        }
    }
}