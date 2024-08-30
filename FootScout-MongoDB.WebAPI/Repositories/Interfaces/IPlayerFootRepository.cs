using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IPlayerFootRepository
    {
        Task<IEnumerable<PlayerFoot>> GetPlayerFeet();
        Task<PlayerFoot> GetPlayerFoot(int footId);
        Task<string> GetPlayerFootName(int footId);
    }
}