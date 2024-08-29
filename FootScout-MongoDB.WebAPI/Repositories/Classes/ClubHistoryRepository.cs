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
            var clubHistory = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.Id == clubHistoryId)
                .FirstOrDefaultAsync();

            if (clubHistory != null)
            { 
                if (clubHistory.PlayerPositionId != null)
                {
                    clubHistory.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubHistory.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.AchievementsId != null)
                {
                    clubHistory.Achievements = await _dbContext.AchievementsCollection
                        .Find(a => a.Id == clubHistory.AchievementsId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.PlayerId != null)
                {
                    clubHistory.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == clubHistory.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return clubHistory;
        }

        public async Task<IEnumerable<ClubHistory>> GetAllClubHistory()
        {
            var clubHistories = await _dbContext.ClubHistoriesCollection
                .Find(_ => true)
                .ToListAsync();

            foreach (var clubHistory in clubHistories)
            {
                if (clubHistory.PlayerPositionId != null)
                {
                    clubHistory.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubHistory.PlayerPositionId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.AchievementsId != null)
                {
                    clubHistory.Achievements = await _dbContext.AchievementsCollection
                        .Find(a => a.Id == clubHistory.AchievementsId)
                        .FirstOrDefaultAsync();
                }

                if (clubHistory.PlayerId != null)
                {
                    clubHistory.Player = await _dbContext.UsersCollection
                        .Find(u => u.Id == clubHistory.PlayerId)
                        .FirstOrDefaultAsync();
                }
            }

            return clubHistories;
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
            await _dbContext.ClubHistoriesCollection.ReplaceOneAsync(clubHistoryFilter, clubHistory);

            if (clubHistory.Achievements != null && clubHistory.Achievements.Id != 0)
            {
                var achievementsFilter = Builders<Achievements>.Filter.Eq(a => a.Id, clubHistory.Achievements.Id);
                await _dbContext.AchievementsCollection.ReplaceOneAsync(achievementsFilter, clubHistory.Achievements);
            }
        }

        public async Task DeleteClubHistory(int clubHistoryId)
        {
            var clubHistory = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.Id == clubHistoryId)
                .FirstOrDefaultAsync();

            if (clubHistory == null)
                throw new ArgumentException($"No club history found with ID {clubHistoryId}");

            if (clubHistory.AchievementsId != null)
            {
                await _dbContext.AchievementsCollection
                    .DeleteOneAsync(a => a.Id == clubHistory.AchievementsId);
            }

            await _dbContext.ClubHistoriesCollection
                .DeleteOneAsync(ch => ch.Id == clubHistoryId);
        }
    }
}