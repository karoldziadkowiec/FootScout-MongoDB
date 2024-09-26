using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class FavoriteClubAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private FavoriteClubAdvertisementRepository _favoriteClubAdvertisementRepository;

        public FavoriteClubAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _favoriteClubAdvertisementRepository = new FavoriteClubAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var newFavoriteAd = new FavoriteClubAdvertisement
            {
                ClubAdvertisementId = 1,
                UserId = "pepguardiola"
            };

            // Act
            await _favoriteClubAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var result = await _dbContext.FavoriteClubAdvertisementsCollection.Find(fca => fca.ClubAdvertisementId == 1 && fca.UserId == "pepguardiola").FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClubAdvertisementId);
            Assert.Equal("pepguardiola", result.UserId);

            await _dbContext.FavoriteClubAdvertisementsCollection.DeleteOneAsync(fca => fca.Id == result.Id);
        }

        [Fact]
        public async Task DeleteFromFavorites_DeleteClubAdFromFavorites()
        {
            // Arrange
            var newFavoriteAd = new FavoriteClubAdvertisement
            {
                ClubAdvertisementId = 2,
                UserId = "pepguardiola"
            };
            await _favoriteClubAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var favResult = await _dbContext.FavoriteClubAdvertisementsCollection.Find(pa => pa.ClubAdvertisementId == 2 && pa.UserId == "pepguardiola").FirstOrDefaultAsync();

            // Act
            await _favoriteClubAdvertisementRepository.DeleteFromFavorites(favResult.Id);

            // Assert
            var result = await _dbContext.FavoriteClubAdvertisementsCollection.Find(fca => fca.Id == favResult.Id).FirstOrDefaultAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_CheckIfClubAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var userId = "leomessi";

            // Act
            var result = await _favoriteClubAdvertisementRepository.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);

            // Assert
            Assert.Equal(1, result);
        }
    }
}