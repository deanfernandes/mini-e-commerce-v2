using ProductService.Data.Repositories;
using ProductService.Data.Models;
using ProductsService.Data.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ProductService.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task GetAll_WithListOfProducts_ReturnsOkResult()
        {
            var mockRepo = new Mock<IProductRepository>();
            var expectedProducts = new List<Product>
            {
                new Product { ProductId = 1, Name = "Test Product 1" },
                new Product { ProductId = 2, Name = "Test Product 2" }
            };
            mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(expectedProducts);
            var controller = new ProductsController(mockRepo.Object);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, ((List<Product>)returnProducts).Count);
            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}