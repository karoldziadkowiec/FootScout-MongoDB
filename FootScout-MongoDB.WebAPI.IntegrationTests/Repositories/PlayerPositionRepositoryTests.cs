using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class PlayerPositionRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private PlayerPositionRepository _playerPositionRepository;

        public PlayerPositionRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _playerPositionRepository = new PlayerPositionRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerPositions_ReturnsAllPlayerPositions()
        {
            // Arrange & Act
            var result = await _playerPositionRepository.GetPlayerPositions();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.Count());
        }

        [Fact]
        public async Task GetPlayerPosition_ReturnsCorrectPlayerPosition()
        {
            // Arrange & Act
            var result = await _playerPositionRepository.GetPlayerPosition(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Goalkeeper", result.PositionName);
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _playerPositionRepository.GetPlayerPositionCount();

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsCorrectName()
        {
            // Arrange
            var positionId = 15;

            // Act
            var result = await _playerPositionRepository.GetPlayerPositionName(positionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Striker", result);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists()
        {
            // Arrange
            var positionName = "Striker";

            // Act
            var result = await _playerPositionRepository.CheckPlayerPositionExists(positionName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionName = "Trainer";

            // Act
            var result = await _playerPositionRepository.CheckPlayerPositionExists(positionName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreatePlayerPosition_AddsNewPosition_WhenPositionDoesNotExist()
        {
            // Arrange
            var newPosition = new PlayerPosition { Id = await _newIdGeneratorService.GenerateNewPlayerPositionId(), PositionName = "NewPosition" };

            // Act
            await _playerPositionRepository.CreatePlayerPosition(newPosition);

            // Assert
            var result = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == newPosition.Id).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("NewPosition", result.PositionName);

            await _dbContext.PlayerPositionsCollection.DeleteOneAsync(pp => pp.Id == newPosition.Id);
        }
    }
}