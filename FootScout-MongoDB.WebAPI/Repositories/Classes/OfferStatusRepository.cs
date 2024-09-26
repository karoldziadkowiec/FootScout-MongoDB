using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class OfferStatusRepository : IOfferStatusRepository
    {
        private readonly MongoDBContext _dbContext;

        public OfferStatusRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OfferStatus>> GetOfferStatuses()
            => await _dbContext.OfferStatusesCollection.Find(_ => true).ToListAsync();

        public async Task<OfferStatus> GetOfferStatus(int offerStatusId)
        {
            return await _dbContext.OfferStatusesCollection
                .Find(os => os.Id == offerStatusId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetOfferStatusName(int statusId)
        {
            var offerStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.Id == statusId)
                .FirstOrDefaultAsync();

            return offerStatus?.StatusName;
        }

        public async Task<int> GetOfferStatusId(string statusName)
        {
            var offerStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == statusName)
                .FirstOrDefaultAsync();

            return offerStatus?.Id ?? 0;
        }
    }
}