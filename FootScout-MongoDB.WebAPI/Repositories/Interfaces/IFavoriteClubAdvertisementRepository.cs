using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IFavoriteClubAdvertisementRepository
    {
        Task AddToFavorites(FavoriteClubAdvertisement favoriteClubAdvertisement);
        Task DeleteFromFavorites(int favoriteClubAdvertisementId);
        Task<int> CheckClubAdvertisementIsFavorite(int clubAdvertisementId, string userId);
    }
}