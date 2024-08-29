using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IOfferStatusRepository
    {
        Task<IEnumerable<OfferStatus>> GetOfferStatuses();
        Task<OfferStatus> GetOfferStatus(int offerStatusId);
        Task<string> GetOfferStatusName(int offerStatusId);
        Task<int> GetOfferStatusId(string offerStatusName);
    }
}