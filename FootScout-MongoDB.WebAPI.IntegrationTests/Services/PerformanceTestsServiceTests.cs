using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class PerformanceTestsServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private readonly PerformanceTestsService _performanceTestsService;

        public PerformanceTestsServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            var passwordService = Mock.Of<IPasswordService>();
            INewIdGeneratorService newIdGeneratorService = new NewIdGeneratorService(_dbContext);

            _performanceTestsService = new PerformanceTestsService(_dbContext, passwordService, newIdGeneratorService);
        }

        [Fact]
        public async Task SeedComponents_AddsNewComponents()
        {
            // Arrange
            var testCounter = 10;

            // Act
            await _performanceTestsService.SeedComponents(testCounter);

            // Assert
            var usersCount = await _dbContext.UsersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty); ;
            Assert.True(usersCount >= testCounter);

            var userRolesCount = await _dbContext.UserRolesCollection.CountDocumentsAsync(FilterDefinition<UserRole>.Empty);
            Assert.True(userRolesCount >= testCounter);

            var achievementsCount = await _dbContext.AchievementsCollection.CountDocumentsAsync(FilterDefinition<Achievements>.Empty);
            Assert.True(achievementsCount >= testCounter);

            var clubHistoriesCount = await _dbContext.ClubHistoriesCollection.CountDocumentsAsync(FilterDefinition<ClubHistory>.Empty);
            Assert.True(clubHistoriesCount >= testCounter);

            var problemsCount = await _dbContext.ProblemsCollection.CountDocumentsAsync(FilterDefinition<Problem>.Empty);
            Assert.True(problemsCount >= testCounter);

            var chatsCount = await _dbContext.ChatsCollection.CountDocumentsAsync(FilterDefinition<Chat>.Empty);
            Assert.True(chatsCount >= testCounter);

            var messagesCount = await _dbContext.MessagesCollection.CountDocumentsAsync(FilterDefinition<Message>.Empty);
            Assert.True(messagesCount >= testCounter);

            var playerAdvertisementsCount = await _dbContext.PlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<PlayerAdvertisement>.Empty);
            Assert.True(playerAdvertisementsCount >= testCounter);

            var favoritePlayerAdvertisementsCount = await _dbContext.FavoritePlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<FavoritePlayerAdvertisement>.Empty);
            Assert.True(favoritePlayerAdvertisementsCount >= testCounter);

            var clubOffersCount = await _dbContext.ClubOffersCollection.CountDocumentsAsync(FilterDefinition<ClubOffer>.Empty);
            Assert.True(clubOffersCount >= testCounter);

            var clubAdvertisementsCount = await _dbContext.ClubAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<ClubAdvertisement>.Empty);
            Assert.True(clubAdvertisementsCount >= testCounter);

            var favoriteClubAdvertisementsCount = await _dbContext.FavoriteClubAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<FavoriteClubAdvertisement>.Empty);
            Assert.True(favoriteClubAdvertisementsCount >= testCounter);

            var playerOffersCount = await _dbContext.PlayerOffersCollection.CountDocumentsAsync(FilterDefinition<PlayerOffer>.Empty);
            Assert.True(playerOffersCount >= testCounter);

            var salaryRangesCount = await _dbContext.SalaryRangesCollection.CountDocumentsAsync(FilterDefinition<SalaryRange>.Empty);
            Assert.True(salaryRangesCount >= 2 * testCounter);

            await _performanceTestsService.ClearDatabaseOfSeededComponents();
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_RemovesDatabaseOfSeededComponents()
        {
            // Arrange
            var expectedValue = 0;

            // Act
            await _performanceTestsService.ClearDatabaseOfSeededComponents();

            // Assert
            var usersCount = await _dbContext.UsersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);
            Assert.Equal(2, usersCount);

            var userRolesCount = await _dbContext.UserRolesCollection.CountDocumentsAsync(FilterDefinition<UserRole>.Empty);
            Assert.Equal(2, userRolesCount);

            var achievementsCount = await _dbContext.AchievementsCollection.CountDocumentsAsync(FilterDefinition<Achievements>.Empty);
            Assert.Equal(expectedValue, achievementsCount);

            var clubHistoriesCount = await _dbContext.ClubHistoriesCollection.CountDocumentsAsync(FilterDefinition<ClubHistory>.Empty);
            Assert.Equal(expectedValue, clubHistoriesCount);

            var problemsCount = await _dbContext.ProblemsCollection.CountDocumentsAsync(FilterDefinition<Problem>.Empty);
            Assert.Equal(expectedValue, problemsCount);

            var chatsCount = await _dbContext.ChatsCollection.CountDocumentsAsync(FilterDefinition<Chat>.Empty);
            Assert.Equal(expectedValue, chatsCount);

            var messagesCount = await _dbContext.MessagesCollection.CountDocumentsAsync(FilterDefinition<Message>.Empty);
            Assert.Equal(expectedValue, messagesCount);

            var playerAdvertisementsCount = await _dbContext.PlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<PlayerAdvertisement>.Empty);
            Assert.Equal(expectedValue, playerAdvertisementsCount);

            var favoritePlayerAdvertisementsCount = await _dbContext.FavoritePlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<FavoritePlayerAdvertisement>.Empty);
            Assert.Equal(expectedValue, favoritePlayerAdvertisementsCount);

            var clubOffersCount = await _dbContext.ClubOffersCollection.CountDocumentsAsync(FilterDefinition<ClubOffer>.Empty);
            Assert.Equal(expectedValue, clubOffersCount);

            var clubAdvertisementsCount = await _dbContext.ClubAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<ClubAdvertisement>.Empty);
            Assert.Equal(expectedValue, clubAdvertisementsCount);

            var favoriteClubAdvertisementsCount = await _dbContext.FavoriteClubAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<FavoriteClubAdvertisement>.Empty);
            Assert.Equal(expectedValue, favoriteClubAdvertisementsCount);

            var playerOffersCount = await _dbContext.PlayerOffersCollection.CountDocumentsAsync(FilterDefinition<PlayerOffer>.Empty);
            Assert.Equal(expectedValue, playerOffersCount);

            var salaryRangesCount = await _dbContext.SalaryRangesCollection.CountDocumentsAsync(FilterDefinition<SalaryRange>.Empty);
            Assert.Equal(expectedValue, salaryRangesCount);
        }
    }
}