using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IClubHistoryRepository
    {
        Task<ClubHistory> GetClubHistory(int clubHistoryId);
        Task<IEnumerable<ClubHistory>> GetAllClubHistory();
        Task<int> GetClubHistoryCount();
        Task CreateClubHistory(ClubHistory clubHistory);
        Task UpdateClubHistory(ClubHistory clubHistory);
        Task DeleteClubHistory(int clubHistoryId);
    }
}