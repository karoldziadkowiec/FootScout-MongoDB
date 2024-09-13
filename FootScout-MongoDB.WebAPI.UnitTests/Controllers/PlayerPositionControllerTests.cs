using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class PlayerPositionControllerTests
    {
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly PlayerPositionController _playerPositionController;

        public PlayerPositionControllerTests()
        {
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _playerPositionController = new PlayerPositionController(_mockPlayerPositionRepository.Object, _mockNewIdGeneratorService.Object);
        }

        [Fact]
        public async Task GetPlayerPositions_ReturnsOkResultWithPlayerPositionsList()
        {
            // Arrange
            var playerPositions = new List<PlayerPosition>
            {
                new PlayerPosition { Id = 1, PositionName = "Goalkeeper" },
                new PlayerPosition { Id = 15, PositionName = "Striker" }
            };

            _mockPlayerPositionRepository.Setup(repo => repo.GetPlayerPositions()).ReturnsAsync(playerPositions);

            // Act
            var result = await _playerPositionController.GetPlayerPositions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerPosition>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPlayerPositions = Assert.IsType<List<PlayerPosition>>(okResult.Value);
            Assert.Equal(playerPositions.Count, returnedPlayerPositions.Count);
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 10;
            _mockPlayerPositionRepository.Setup(repo => repo.GetPlayerPositionCount()).ReturnsAsync(count);

            // Act
            var result = await _playerPositionController.GetPlayerPositionCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetPlayerPositionName_ValidId_ReturnsOkResultWithPositionName()
        {
            // Arrange
            var positionId = 1;
            var positionName = "Goalkeeper";

            _mockPlayerPositionRepository.Setup(repo => repo.GetPlayerPositionName(positionId)).ReturnsAsync(positionName);

            // Act
            var result = await _playerPositionController.GetPlayerPositionName(positionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPositionName = Assert.IsType<string>(okResult.Value);
            Assert.Equal(positionName, returnedPositionName);
        }

        [Fact]
        public async Task GetPlayerPositionName_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var positionId = 999;
            _mockPlayerPositionRepository.Setup(repo => repo.GetPlayerPositionName(positionId)).ReturnsAsync((string)null);

            // Act
            var result = await _playerPositionController.GetPlayerPositionName(positionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsOkResultWithBoolean()
        {
            // Arrange
            var positionName = "Forward";
            var exists = true;

            _mockPlayerPositionRepository.Setup(repo => repo.CheckPlayerPositionExists(positionName)).ReturnsAsync(exists);

            // Act
            var result = await _playerPositionController.CheckPlayerPositionExists(positionName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExists = Assert.IsType<bool>(okResult.Value);
            Assert.Equal(exists, returnedExists);
        }

        [Fact]
        public async Task CreatePlayerPosition_ValidData_ReturnsOkResultWithPlayerPosition()
        {
            // Arrange
            var playerPosition = new PlayerPosition { Id = 1, PositionName = "Forward" };

            _mockPlayerPositionRepository.Setup(repo => repo.CreatePlayerPosition(playerPosition)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerPositionController.CreatePlayerPosition(playerPosition);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPlayerPosition = Assert.IsType<PlayerPosition>(okResult.Value);
            Assert.Equal(playerPosition.PositionName, returnedPlayerPosition.PositionName);
        }

        [Fact]
        public async Task CreatePlayerPosition_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            PlayerPosition playerPosition = null;

            // Act
            var result = await _playerPositionController.CreatePlayerPosition(playerPosition);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid player position data.", badRequestResult.Value);
        }
    }
}