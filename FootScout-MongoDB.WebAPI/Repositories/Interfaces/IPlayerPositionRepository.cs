using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IPlayerPositionRepository
    {
        Task<IEnumerable<PlayerPosition>> GetPlayerPositions();
        Task<PlayerPosition> GetPlayerPosition(int positionId);
        Task<int> GetPlayerPositionCount();
        Task<string> GetPlayerPositionName(int positionId);
        Task<bool> CheckPlayerPositionExists(string positionName);
        Task CreatePlayerPosition(PlayerPosition playerPosition);
    }
}