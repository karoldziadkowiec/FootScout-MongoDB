using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class NewIdGeneratorServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;

        public NewIdGeneratorServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
        }

        [Fact]
        public async Task GenerateNewUserRoleId_ShouldReturnNextId_WhenUserRoleExists()
        {
            // Arrange
            var resultCount = 5;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewUserRoleId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewOfferStatusId_ShouldReturnNextId_WhenOfferStatusExists()
        {
            // Arrange
            var resultCount = 4;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewOfferStatusId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewPlayerFootId_ShouldReturnNextId_WhenPlayerFootExists()
        {
            // Arrange
            var resultCount = 4;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewPlayerFootId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewPlayerPositionId_ShouldReturnNextId_WhenPlayerPositionExists()
        {
            // Arrange
            var resultCount = 16;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewPlayerPositionId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewAchievementsId_ShouldReturnNextId_WhenAchievementsExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewAchievementsId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewClubHistoryId_ShouldReturnNextId_WhenClubHistoryExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewClubHistoryId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewSalaryRangeId_ShouldReturnNextId_WhenSalaryRangeExists()
        {
            // Arrange
            var resultCount = 5;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewSalaryRangeId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewPlayerAdvertisementId_ShouldReturnNextId_WhenPlayerAdvertisementExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewPlayerAdvertisementId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewClubOfferId_ShouldReturnNextId_WhenClubOfferExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewClubOfferId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewFavoritePlayerAdvertisementId_ShouldReturnNextId_WhenFavoritePlayerAdvertisementExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewFavoritePlayerAdvertisementId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewClubAdvertisementId_ShouldReturnNextId_WhenClubAdvertisementExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewClubAdvertisementId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewPlayerOfferId_ShouldReturnNextId_WhenPlayerOfferExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewPlayerOfferId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewFavoriteClubAdvertisementId_ShouldReturnNextId_WhenFavoriteClubAdvertisementExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewFavoriteClubAdvertisementId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewProblemId_ShouldReturnNextId_WhenProblemExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewProblemId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewChatId_ShouldReturnNextId_WhenChatExists()
        {
            // Arrange
            var resultCount = 3;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewChatId();

            // Assert
            Assert.Equal(resultCount, newId);
        }

        [Fact]
        public async Task GenerateNewMessageId_ShouldReturnNextId_WhenMessageExists()
        {
            // Arrange
            var resultCount = 4;

            // Act
            var newId = await _newIdGeneratorService.GenerateNewMessageId();

            // Assert
            Assert.Equal(resultCount, newId);
        }
    }
}