using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IPlayerOfferRepository
    {
        Task<PlayerOffer> GetPlayerOffer(int playerOfferId);
        Task<IEnumerable<PlayerOffer>> GetPlayerOffers();
        Task<IEnumerable<PlayerOffer>> GetActivePlayerOffers();
        Task<int> GetActivePlayerOfferCount();
        Task<IEnumerable<PlayerOffer>> GetInactivePlayerOffers();
        Task CreatePlayerOffer(PlayerOffer playerOffer);
        Task UpdatePlayerOffer(PlayerOffer playerOffer);
        Task DeletePlayerOffer(int playerOfferId);
        Task AcceptPlayerOffer(PlayerOffer playerOffer);
        Task RejectPlayerOffer(PlayerOffer playerOffer);
        Task<int> GetPlayerOfferStatusId(int clubAdvertisementId, string userId);
        Task<MemoryStream> ExportPlayerOffersToCsv();
    }
}