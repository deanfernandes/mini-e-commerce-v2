using Moq;
using AuthService.Api.Repositories;
using AuthService.Api.Services;
using Microsoft.Extensions.Configuration;





using AuthService.Api.Controllers;
using AuthService.Api.Models;


using Microsoft.AspNetCore.Mvc;


using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Routing;

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
            var mockConfig = new Mock<IConfiguration>();
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
            mockKafka.Verify(k => k.ProduceUserRegisteredAsync(It.IsAny<string>()), Times.Once);
        }
    }
}