using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IPlayerAdvertisementRepository
    {
        Task<PlayerAdvertisement> GetPlayerAdvertisement(int playerAdvertisementId);
        Task<IEnumerable<PlayerAdvertisement>> GetAllPlayerAdvertisements();
        Task<IEnumerable<PlayerAdvertisement>> GetActivePlayerAdvertisements();
        Task<int> GetActivePlayerAdvertisementCount();
        Task<IEnumerable<PlayerAdvertisement>> GetInactivePlayerAdvertisements();
        Task CreatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement);
        Task UpdatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement);
        Task DeletePlayerAdvertisement(int playerAdvertisementId);
        Task<MemoryStream> ExportPlayerAdvertisementsToCsv();
    }
}