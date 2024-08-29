using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class SalaryRangeRepository : ISalaryRangeRepository
    {
        private readonly MongoDBContext _dbContext;

        public SalaryRangeRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateSalaryRange(SalaryRange salaryRange)
        {
            await _dbContext.SalaryRangesCollection.InsertOneAsync(salaryRange);
        }
    }
}