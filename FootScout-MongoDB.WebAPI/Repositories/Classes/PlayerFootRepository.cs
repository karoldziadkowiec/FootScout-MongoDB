using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class PlayerFootRepository : IPlayerFootRepository
    {
        private readonly MongoDBContext _dbContext;

        public PlayerFootRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PlayerFoot>> GetPlayerFeet()
            => await _dbContext.PlayerFeetCollection.Find(_ => true).ToListAsync();

        public async Task<string> GetPlayerFootName(int footId)
        {
            var playerFoot = await _dbContext.PlayerFeetCollection
                .Find(pf => pf.Id == footId)
                .FirstOrDefaultAsync();

            return playerFoot.FootName;
        }
    }
}