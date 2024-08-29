using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IClubAdvertisementRepository
    {
        Task<ClubAdvertisement> GetClubAdvertisement(int clubAdvertisementId);
        Task<IEnumerable<ClubAdvertisement>> GetAllClubAdvertisements();
        Task<IEnumerable<ClubAdvertisement>> GetActiveClubAdvertisements();
        Task<int> GetActiveClubAdvertisementCount();
        Task<IEnumerable<ClubAdvertisement>> GetInactiveClubAdvertisements();
        Task CreateClubAdvertisement(ClubAdvertisement clubAdvertisement);
        Task UpdateClubAdvertisement(ClubAdvertisement clubAdvertisement);
        Task DeleteClubAdvertisement(int clubAdvertisementId);
        Task<MemoryStream> ExportClubAdvertisementsToCsv();
    }
}