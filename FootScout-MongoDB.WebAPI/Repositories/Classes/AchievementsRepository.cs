using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly MongoDBContext _dbContext;

        public AchievementsRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAchievements(Achievements achievements)
        {
            await _dbContext.AchievementsCollection.InsertOneAsync(achievements);
        }
    }
}