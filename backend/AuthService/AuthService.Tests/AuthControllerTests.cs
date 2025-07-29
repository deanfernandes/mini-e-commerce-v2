using Moq;
using AuthService.Api.Repositories;
using AuthService.Api.Services;
using Microsoft.Extensions.Configuration;
using Kafka.Contracts.Messages;
using AuthService.Api.Controllers;
using AuthService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Register_ReturnsCreatedAtAction_Success()
        {
            var mockRepo = new Mock<IUserRepository>();
            var mockKafka = new Mock<IKafkaProducerService>();

            var mockJwtSection = new Mock<IConfigurationSection>();
            mockJwtSection.Setup(x => x["SecretKey"]).Returns("YourSuperSecretKey1234567890123456");
            mockJwtSection.Setup(x => x["Issuer"]).Returns("your-issuer");
            mockJwtSection.Setup(x => x["Audience"]).Returns("your-audience");
            mockJwtSection.Setup(x => x["ExpiresInMinutes"]).Returns("60");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);

            var dto = new RegisterUserDto
            {
                Username = "anon",
                Email = "anon@example.com",
                Password = "Password123!"
            };

            mockRepo.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);

            var controller = new AuthController(mockRepo.Object, mockConfig.Object, mockKafka.Object);

            var result = await controller.Register(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = createdResult.Value;
            Assert.NotNull(returnValue);

            mockRepo.Verify(r => r.EmailExistsAsync(dto.Email), Times.Once);
            mockRepo.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockKafka.Verify(k => k.ProduceUserRegisteredAsync(It.IsAny<UserRegisteredMessage>()), Times.Once);
        }
    }
}