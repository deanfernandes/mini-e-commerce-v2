using AuthService.Api.Models;
using AuthService.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuthService.Tests
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IConfigurationSection> _mockJwtSection;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockJwtSection = new Mock<IConfigurationSection>();
            _mockJwtSection.Setup(x => x["SecretKey"]).Returns("YourSuperSecretKey1234567890123456");
            _mockJwtSection.Setup(x => x["Issuer"]).Returns("your-issuer");
            _mockJwtSection.Setup(x => x["Audience"]).Returns("your-audience");
            _mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(_mockJwtSection.Object);

            _jwtService = new JwtService(_mockConfig.Object);
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