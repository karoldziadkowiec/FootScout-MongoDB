using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class PlayerFootRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private PlayerFootRepository _playerFootRepository;

        public PlayerFootRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _playerFootRepository = new PlayerFootRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerFeet_ReturnsAllPlayerFeet()
        {
            // Arrange & Act
            var result = await _playerFootRepository.GetPlayerFeet();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetPlayerFoot_ReturnsCorrectPlayerFoot()
        {
            // Arrange & Act
            var result = await _playerFootRepository.GetPlayerFoot(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Left", result.FootName);
        }

        [Fact]
        public async Task GetPlayerFootName_ReturnsPlayerFootName()
        {
            // Arrange
            var footId = 1;
            
            // Act
            var result = await _playerFootRepository.GetPlayerFootName(footId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Left", result);
        }
    }
}