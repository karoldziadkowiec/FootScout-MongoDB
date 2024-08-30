using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class FavoriteClubAdvertisementRepository : IFavoriteClubAdvertisementRepository
    {
        private readonly MongoDBContext _dbContext;

        public FavoriteClubAdvertisementRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddToFavorites(FavoriteClubAdvertisement favoriteClubAdvertisement)
        {
            await _dbContext.FavoriteClubAdvertisementsCollection.InsertOneAsync(favoriteClubAdvertisement);
        }

        public async Task DeleteFromFavorites(int favoriteClubAdvertisementId)
        {
            var favoriteClubAdvertisement = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fca => fca.Id == favoriteClubAdvertisementId)
                .FirstOrDefaultAsync();

            if (favoriteClubAdvertisement == null)
                throw new ArgumentException($"No Favorite Club Advertisement found with ID {favoriteClubAdvertisementId}");

            await _dbContext.FavoriteClubAdvertisementsCollection
                .DeleteOneAsync(fca => fca.Id == favoriteClubAdvertisementId);
        }

        public async Task<int> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId)
        {
            var isFavorite = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(ca => ca.ClubAdvertisementId == clubAdvertisementId && ca.UserId == userId)
                .FirstOrDefaultAsync();

            return isFavorite?.Id ?? 0;
        }
    }
}