using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class PlayerPositionRepository : IPlayerPositionRepository
    {
        private readonly MongoDBContext _dbContext;

        public PlayerPositionRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlayerPosition>> GetPlayerPositions()
            => await _dbContext.PlayerPositionsCollection.Find(_ => true).ToListAsync();

        public async Task<PlayerPosition> GetPlayerPosition(int positionId)
        {
            return await _dbContext.PlayerPositionsCollection
                .Find(pp => pp.Id == positionId)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetPlayerPositionCount()
        {
            return (int)await _dbContext.PlayerPositionsCollection.CountDocumentsAsync(FilterDefinition<PlayerPosition>.Empty);
        }

        public async Task<string> GetPlayerPositionName(int positionId)
        {
            var playerPosition = await _dbContext.PlayerPositionsCollection
                .Find(p => p.Id == positionId)
                .FirstOrDefaultAsync();

            return playerPosition.PositionName;
        }

        public async Task<bool> CheckPlayerPositionExists(string positionName)
        {
            return await _dbContext.PlayerPositionsCollection
                .Find(p => p.PositionName == positionName)
                .AnyAsync();
        }

        public async Task CreatePlayerPosition(PlayerPosition playerPosition)
        {
            var isExists = await _dbContext.PlayerPositionsCollection
                .Find(p => p.PositionName == playerPosition.PositionName)
                .AnyAsync();

            if (isExists)
                throw new ArgumentException($"Position {playerPosition.PositionName} already exists!");

            await _dbContext.PlayerPositionsCollection.InsertOneAsync(playerPosition);
        }
    }
}