using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class FavoritePlayerAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private FavoritePlayerAdvertisementRepository _favoritePlayerAdvertisementRepository;

        public FavoritePlayerAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _favoritePlayerAdvertisementRepository = new FavoritePlayerAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var newFavoriteAd = new FavoritePlayerAdvertisement
            {
                PlayerAdvertisementId = 1,
                UserId = "leomessi"
            };

            // Act
            await _favoritePlayerAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var result = await _dbContext.FavoritePlayerAdvertisementsCollection.Find(fpa => fpa.PlayerAdvertisementId == 1 && fpa.UserId == "leomessi").FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PlayerAdvertisementId);
            Assert.Equal("leomessi", result.UserId);

            await _dbContext.FavoritePlayerAdvertisementsCollection.DeleteOneAsync(fpa => fpa.Id == result.Id);
        }

        [Fact]
        public async Task DeleteFromFavorites_DeletePlayerAdFromFavorites()
        {
            // Arrange
            var newFavoriteAd = new FavoritePlayerAdvertisement
            {
                PlayerAdvertisementId = 2,
                UserId = "leomessi"
            };
            await _favoritePlayerAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var favResult = await _dbContext.FavoritePlayerAdvertisementsCollection.Find(fpa => fpa.PlayerAdvertisementId == 2 && fpa.UserId == "leomessi").FirstOrDefaultAsync();

            // Act
            await _favoritePlayerAdvertisementRepository.DeleteFromFavorites(favResult.Id);

            // Assert
            var result = await _dbContext.FavoritePlayerAdvertisementsCollection.Find(fpa => fpa.Id == favResult.Id).FirstOrDefaultAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_CheckIfPlayerAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var userId = "pepguardiola";

            // Act
            var result = await _favoritePlayerAdvertisementRepository.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);

            // Assert
            Assert.Equal(1, result);
        }
    }
}