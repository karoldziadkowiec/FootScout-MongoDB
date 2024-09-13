using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class PlayerFootControllerTests
    {
        private readonly Mock<IPlayerFootRepository> _mockPlayerFootRepository;
        private readonly PlayerFootController _playerFootController;

        public PlayerFootControllerTests()
        {
            _mockPlayerFootRepository = new Mock<IPlayerFootRepository>();
            _playerFootController = new PlayerFootController(_mockPlayerFootRepository.Object);
        }

        [Fact]
        public async Task GetPlayerFeet_ReturnsOkResultWithPlayerFeetList()
        {
            // Arrange
            var playerFeet = new List<PlayerFoot>
            {
                new PlayerFoot { Id = 1, FootName = "Left" },
                new PlayerFoot { Id = 2, FootName = "Right" },
                new PlayerFoot { Id = 3, FootName = "Two-Footed" }
            };

            _mockPlayerFootRepository.Setup(repo => repo.GetPlayerFeet()).ReturnsAsync(playerFeet);

            // Act
            var result = await _playerFootController.GetPlayerFeet();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerFoot>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPlayerFeet = Assert.IsType<List<PlayerFoot>>(okResult.Value);
            Assert.Equal(playerFeet.Count, returnedPlayerFeet.Count);
        }

        [Fact]
        public async Task GetPlayerFootName_ValidId_ReturnsOkResultWithFootName()
        {
            // Arrange
            var footId = 1;
            var footName = "Left";

            _mockPlayerFootRepository.Setup(repo => repo.GetPlayerFootName(footId)).ReturnsAsync(footName);

            // Act
            var result = await _playerFootController.GetPlayerFootName(footId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFootName = Assert.IsType<string>(okResult.Value);
            Assert.Equal(footName, returnedFootName);
        }

        [Fact]
        public async Task GetPlayerFootName_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var footId = 999;
            _mockPlayerFootRepository.Setup(repo => repo.GetPlayerFootName(footId)).ReturnsAsync((string)null);

            // Act
            var result = await _playerFootController.GetPlayerFootName(footId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}