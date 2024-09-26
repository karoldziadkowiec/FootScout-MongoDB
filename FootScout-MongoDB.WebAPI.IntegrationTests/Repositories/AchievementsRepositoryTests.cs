using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class AchievementsRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private AchievementsRepository _achievementsRepository;

        public AchievementsRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _achievementsRepository = new AchievementsRepository(_dbContext);
        }

        [Fact]
        public async Task CreateAchievements_AddsNewAchievements()
        {
            // Arrange
            var newAchievements = new Achievements
            {
                Id = await _newIdGeneratorService.GenerateNewAchievementsId(),
                NumberOfMatches = 80,
                Goals = 60,
                Assists = 45,
                AdditionalAchievements = "no"
            };

            // Act
            await _achievementsRepository.CreateAchievements(newAchievements);

            // Assert
            var result = await _dbContext.AchievementsCollection.Find(a => a.Id == 3).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(80, result.NumberOfMatches);
            Assert.Equal(60, result.Goals);
            Assert.Equal(45, result.Assists);

            await _dbContext.AchievementsCollection.DeleteOneAsync(a => a.Id == 3);
        }
    }
}