using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class FavoritePlayerAdvertisementRepository : IFavoritePlayerAdvertisementRepository
    {
        private readonly MongoDBContext _dbContext;

        public FavoritePlayerAdvertisementRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddToFavorites(FavoritePlayerAdvertisement favoritePlayerAdvertisement)
        {
            await _dbContext.FavoritePlayerAdvertisementsCollection.InsertOneAsync(favoritePlayerAdvertisement);
        }

        public async Task DeleteFromFavorites(int favoritePlayerAdvertisementId)
        {
            var favoritePlayerAdvertisement = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(fpa => fpa.Id == favoritePlayerAdvertisementId)
                .FirstOrDefaultAsync();

            if (favoritePlayerAdvertisement == null)
                throw new ArgumentException($"No Favorite Player Advertisement found with ID {favoritePlayerAdvertisementId}");

            await _dbContext.FavoritePlayerAdvertisementsCollection
                .DeleteOneAsync(fpa => fpa.Id == favoritePlayerAdvertisementId);
        }

        public async Task<int> CheckPlayerAdvertisementIsFavorite(int playerAdvertisementId, string userId)
        {
            var isFavorite = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(pa => pa.PlayerAdvertisementId == playerAdvertisementId && pa.UserId == userId)
                .FirstOrDefaultAsync();

            return isFavorite?.Id ?? 0;
        }
    }
}