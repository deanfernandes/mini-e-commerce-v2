using Moq;
using AuthService.Api.Repositories;
using AuthService.Api.Services;
using Microsoft.Extensions.Configuration;
using Kafka.Contracts.Messages;
using AuthService.Api.Controllers;
using AuthService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Confluent.Kafka;
using System.IdentityModel.Tokens.Jwt;

namespace AuthService.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IKafkaProducerService> _mockKafka;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IConfigurationSection> _mockJwtSection;
        private readonly IJwtService _jwtService;

        public AuthControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockKafka = new Mock<IKafkaProducerService>();
            _mockJwtSection = new Mock<IConfigurationSection>();
            _mockConfig = new Mock<IConfiguration>();
            _jwtService = new JwtService(_mockConfig.Object);

            _mockJwtSection.Setup(x => x["SecretKey"]).Returns("YourSuperSecretKey1234567890123456");
            _mockJwtSection.Setup(x => x["Issuer"]).Returns("your-issuer");
            _mockJwtSection.Setup(x => x["Audience"]).Returns("your-audience");
            _mockJwtSection.Setup(x => x["ExpiresInMinutes"]).Returns("60");
            _mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(_mockJwtSection.Object);
        }

        private AuthController CreateController()
        {
            return new AuthController(_mockRepo.Object, _mockConfig.Object, _mockKafka.Object, _jwtService);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var users = new List<User>
            {
                new User { Username = "john", Email = "john@example.com", IsEmailVerified = true },
                new User { Username = "jane", Email = "jane@example.com", IsEmailVerified = false }
            };
            _mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
            var controller = CreateController();

            var result = await controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var userList = returnedUsers.Cast<dynamic>().ToList();
            Assert.Equal(2, userList.Count);
            Assert.Equal("john", userList[0].Username);
            Assert.Equal("jane@example.com", userList[1].Email);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Register_ReturnsCreatedAtAction_Success()
        {
            var dto = new RegisterUserDto
            {
                Username = "anon",
                Email = "anon@example.com",
                Password = "Password123!"
            };
            _mockRepo.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
            var controller = CreateController();

            var result = await controller.Register(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            _mockRepo.Verify(r => r.EmailExistsAsync(dto.Email), Times.Once);
            _mockRepo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockKafka.Verify(k => k.ProduceUserRegisteredAsync(It.IsAny<UserRegisteredMessage>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Login_ValidEmailAndPassword_ReturnsJWT()
        {
            var user = new User
            {
                Username = "john",
                Email = "john@example.com",
                PasswordHash = PasswordService.HashPassword("password"),
                IsEmailVerified = true
            };
            var dto = new LoginUserDto
            {
                Email = "john@example.com",
                Password = "password"
            };
            _mockRepo.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);
            var controller = CreateController();

            var result = await controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseUserDto>(okResult.Value);
            Assert.False(string.IsNullOrEmpty(response.Token));
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Login_InvalidEmail_ReturnsUnAuthorized()
        {
            var dto = new LoginUserDto
            {
                Email = "john@example.com",
                Password = "password"
            };
            _mockRepo.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User)null);
            var controller = CreateController();

            var result = await controller.Login(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password", unauthorized.Value);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Login_WrongPassword_ReturnsUnAuthorized()
        {
            var user = new User
            {
                Username = "john",
                Email = "john@example.com",
                PasswordHash = PasswordService.HashPassword("correctpassword")
            };
            var dto = new LoginUserDto
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            };
            _mockRepo.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

            var controller = CreateController();

            var result = await controller.Login(dto);
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password", unauthorized.Value);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Login_ValidUnverifiedEmail_ReturnsUnAuthorized()
        {
            string password = "password";
            var user = new User
            {
                Username = "john",
                Email = "john@example.com",
                PasswordHash = PasswordService.HashPassword(password)
            };
            var dto = new LoginUserDto
            {
                Email = "john@example.com",
                Password = password
            };
            _mockRepo.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);

            var controller = CreateController();

            var result = await controller.Login(dto);
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Email is not verified", unauthorized.Value);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task ConfirmEmail_ValidToken_ReturnsOK()
        {
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser",
                IsEmailVerified = false
            };
            var token = _jwtService.GenerateEmailConfirmationJwt(user);
            _mockRepo.Setup(r => r.GetUserByIdAsync(user.UserId)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var controller = CreateController();

            var result = await controller.ConfirmEmail(token);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email confirmed successfully.", okResult.Value);
            Assert.True(user.IsEmailVerified);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}