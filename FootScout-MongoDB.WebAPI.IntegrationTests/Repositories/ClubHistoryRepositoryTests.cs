using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class ClubHistoryRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private ClubHistoryRepository _clubHistoryRepository;

        public ClubHistoryRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _clubHistoryRepository = new ClubHistoryRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubHistory_ReturnsCorrectClubHistory()
        {
            // Arrange
            var clubHistoryId = 1;

            // Act
            var result = await _clubHistoryRepository.GetClubHistory(clubHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clubHistoryId, result.Id);
            Assert.Equal("FC Barcelona", result.ClubName);
            Assert.Equal("La Liga", result.League);
        }

        [Fact]
        public async Task GetAllClubHistory_ReturnsAllClubHistory()
        {
            // Arrange & Act
            var result = await _clubHistoryRepository.GetAllClubHistory();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetClubHistoryCount_ReturnsCorrectNumberOfClubHistory()
        {
            // Arrange & Act
            var result = await _clubHistoryRepository.GetClubHistoryCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CreateClubHistory_AddsNewClubHistory()
        {
            // Arrange
            await _dbContext.AchievementsCollection.InsertOneAsync(new Achievements { Id = await _newIdGeneratorService.GenerateNewAchievementsId(), NumberOfMatches = 100, Goals = 50, Assists = 50, AdditionalAchievements = "LM x2" });
            var achievements = await _dbContext.AchievementsCollection.Find(ch => ch.AdditionalAchievements == "LM x2").FirstOrDefaultAsync();

            var newClubHistory = new ClubHistory
            {
                Id = await _newIdGeneratorService.GenerateNewClubHistoryId(),
                PlayerPositionId = 14,
                ClubName = "Inter Miami",
                League = "MLS",
                Region = "USA",
                StartDate = DateTime.Now.AddDays(300),
                EndDate = DateTime.Now.AddDays(450),
                AchievementsId = achievements.Id,
                PlayerId = "leomessi"
            };

            // Act
            await _clubHistoryRepository.CreateClubHistory(newClubHistory);

            // Assert
            var result = await _dbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "Inter Miami").FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Inter Miami", result.ClubName);
            Assert.Equal("MLS", result.League);

            await _dbContext.ClubHistoriesCollection.DeleteOneAsync(ch => ch.Id == result.Id);
        }

        [Fact]
        public async Task UpdateClubHistory_UpdatesExistingClubHistory()
        {
            // Arrange
            var clubHistoryId = 2;
            var existingClubHistory = await _dbContext.ClubHistoriesCollection.Find(ch => ch.Id == clubHistoryId).FirstOrDefaultAsync();
            existingClubHistory.ClubName = "Updated Club Name";
            existingClubHistory.League = "Updated League";

            // Act
            await _clubHistoryRepository.UpdateClubHistory(existingClubHistory);

            // Assert
            var result = await _dbContext.ClubHistoriesCollection.Find(ch => ch.Id == clubHistoryId).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Updated Club Name", result.ClubName);
            Assert.Equal("Updated League", result.League);
        }

        [Fact]
        public async Task DeleteClubHistory_RemovesClubHistoryAndAchievements()
        {
            // Arrange
            await _dbContext.AchievementsCollection.InsertOneAsync(new Achievements { NumberOfMatches = 100, Goals = 50, Assists = 50, AdditionalAchievements = "LM" });
            await _dbContext.ClubHistoriesCollection.InsertOneAsync(new ClubHistory { PlayerPositionId = 15, ClubName = "Borussia Dortmund", League = "Bundesliga", Region = "Germany", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(150), AchievementsId = 3, PlayerId = "leomessi" });

            var clubHistory = await _dbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "Borussia Dortmund").FirstOrDefaultAsync();
            if (clubHistory == null)
                throw new Exception("Test club history not found");

            // Act
            await _clubHistoryRepository.DeleteClubHistory(clubHistory.Id);

            // Assert
            var result = await _dbContext.ClubHistoriesCollection.Find(ch => ch.Id == clubHistory.Id).FirstOrDefaultAsync();
            Assert.Null(result);
        }
    }
}