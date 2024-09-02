using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class ClubHistoryRepository : IClubHistoryRepository
    {
        private readonly MongoDBContext _dbContext;

        public ClubHistoryRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubHistory> GetClubHistory(int clubHistoryId)
        {
            return await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.Id == clubHistoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClubHistory>> GetAllClubHistory()
        {
            return await _dbContext.ClubHistoriesCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<int> GetClubHistoryCount()
        {
            return (int)await _dbContext.ClubHistoriesCollection.CountDocumentsAsync(_ => true);
        }

        public async Task CreateClubHistory(ClubHistory clubHistory)
        {
            await _dbContext.ClubHistoriesCollection.InsertOneAsync(clubHistory);
        }

        public async Task UpdateClubHistory(ClubHistory clubHistory)
        {
            var clubHistoryFilter = Builders<ClubHistory>.Filter.Eq(ch => ch.Id, clubHistory.Id);

            if (clubHistoryFilter != null)
            {
                if (clubHistory.Achievements != null && clubHistory.Achievements.Id != 0)
                {
                    var achievementsFilter = Builders<Achievements>.Filter.Eq(a => a.Id, clubHistory.Achievements.Id);
                    await _dbContext.AchievementsCollection.ReplaceOneAsync(achievementsFilter, clubHistory.Achievements);
                }

                await _dbContext.ClubHistoriesCollection.ReplaceOneAsync(clubHistoryFilter, clubHistory);
            }
        }

        public async Task DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.Id == clubHistoryId)
                .FirstOrDefaultAsync();

            if (clubHistory == null)
                throw new ArgumentException($"No club history found with ID {clubHistoryId}");

            if (clubHistory.AchievementsId != 0)
            {
                await _dbContext.AchievementsCollection
                    .DeleteOneAsync(a => a.Id == clubHistory.AchievementsId);
            }

            await _dbContext.ClubHistoriesCollection
                .DeleteOneAsync(ch => ch.Id == clubHistoryId);
        }
    }
}