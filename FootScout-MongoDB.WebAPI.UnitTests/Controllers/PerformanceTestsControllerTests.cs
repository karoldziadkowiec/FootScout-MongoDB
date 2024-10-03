using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class PerformanceTestsControllerTests
    {
        private readonly Mock<IPerformanceTestsService> _mockPerformanceTestsService;
        private readonly PerformanceTestsController _performanceTestsController;

        public PerformanceTestsControllerTests()
        {
            _mockPerformanceTestsService = new Mock<IPerformanceTestsService>();
            _performanceTestsController = new PerformanceTestsController(_mockPerformanceTestsService.Object);
        }

        [Fact]
        public async Task SeedComponents_ValidTestCounterNumber_ReturnsNoContentResult()
        {
            // Arrange
            var testCounter = 10;

            _mockPerformanceTestsService.Setup(repo => repo.SeedComponents(testCounter)).Returns(Task.CompletedTask);

            // Act
            var result = await _performanceTestsController.SeedComponents(testCounter);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SeedComponents_InvalidTestCounterNumber_ReturnsBadRequest()
        {
            // Arrange
            int testCounter = -1;

            // Act
            var result = await _performanceTestsController.SeedComponents(testCounter);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_ReturnsNoContentResult()
        {
            // Arrange
            _mockPerformanceTestsService.Setup(repo => repo.ClearDatabaseOfSeededComponents()).Returns(Task.CompletedTask);

            // Act
            var result = await _performanceTestsController.ClearDatabaseOfSeededComponents();

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }
    }
}