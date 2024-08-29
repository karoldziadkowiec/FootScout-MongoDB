using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IClubOfferRepository
    {
        Task<ClubOffer> GetClubOffer(int clubOfferId);
        Task<IEnumerable<ClubOffer>> GetClubOffers();
        Task<IEnumerable<ClubOffer>> GetActiveClubOffers();
        Task<int> GetActiveClubOfferCount();
        Task<IEnumerable<ClubOffer>> GetInactiveClubOffers();
        Task CreateClubOffer(ClubOffer clubOffer);
        Task UpdateClubOffer(ClubOffer clubOffer);
        Task DeleteClubOffer(int clubOfferId);
        Task AcceptClubOffer(ClubOffer clubOffer);
        Task RejectClubOffer(ClubOffer clubOffer);
        Task<int> GetClubOfferStatusId(int playerAdvertisementId, string userId);
        Task<MemoryStream> ExportClubOffersToCsv();
    }
}