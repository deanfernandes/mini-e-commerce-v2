using ProductService.Data.Repositories;
using ProductService.Data.Models;
using ProductsService.Data.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ProductService.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
        }

        private ProductsController CreateController()
        {
            return new ProductsController(_mockRepo.Object);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task GetAll_WithListOfProducts_ReturnsOkResult()
        {
            var expectedProducts = new List<Product>
            {
                new Product { ProductId = 1, Name = "Test Product 1" },
                new Product { ProductId = 2, Name = "Test Product 2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedProducts);
            var controller = CreateController();

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, ((List<Product>)returnProducts).Count);
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            int productId = 1;
            var expectedProduct = new Product { ProductId = productId, Name = "Test Product" };
            _mockRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(expectedProduct);
            var controller = CreateController();

            var result = await controller.GetById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(expectedProduct.ProductId, returnProduct.ProductId);
            Assert.Equal(expectedProduct.Name, returnProduct.Name);
            _mockRepo.Verify(r => r.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Create_ValidProduct_ReturnsCreatedAtAction()
        {
            var newProduct = new Product { ProductId = 1, Name = "New Product" };
            _mockRepo.Setup(r => r.AddAsync(newProduct)).Returns(Task.CompletedTask);
            var controller = CreateController();

            var result = await controller.Create(newProduct);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(newProduct.ProductId, returnProduct.ProductId);
            _mockRepo.Verify(r => r.AddAsync(newProduct), Times.Once);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Update_WithValidProduct_ReturnsNoContent()
        {
            int productId = 1;
            var product = new Product { ProductId = productId, Name = "Updated Product" };
            _mockRepo.Setup(r => r.ExistsAsync(productId)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);
            var controller = CreateController();

            var result = await controller.Update(productId, product);

            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(r => r.ExistsAsync(productId), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        [Fact]
        [Trait("Category", "Endpoint")]
        public async Task Delete_ExistingProduct_ReturnsNoContent()
        {
            int productId = 1;
            _mockRepo.Setup(r => r.ExistsAsync(productId)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.DeleteAsync(productId)).Returns(Task.CompletedTask);
            var controller = CreateController();

            var result = await controller.Delete(productId);

            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(r => r.ExistsAsync(productId), Times.Once);
            _mockRepo.Verify(r => r.DeleteAsync(productId), Times.Once);
        }
    }
}