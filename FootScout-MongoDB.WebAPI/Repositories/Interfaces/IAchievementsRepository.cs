using FootScout_MongoDB.WebAPI.Entities;

namespace FootScout_MongoDB.WebAPI.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        Task CreateAchievements(Achievements achievements);
    }
}