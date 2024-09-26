using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class SalaryRangeRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private SalaryRangeRepository _salaryRangeRepository;

        public SalaryRangeRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _salaryRangeRepository = new SalaryRangeRepository(_dbContext);
        }

        [Fact]
        public async Task CreateSalaryRange_AddsNewSalaryRange()
        {
            // Arrange
            var newSalaryRange = new SalaryRange
            {
                Id = await _newIdGeneratorService.GenerateNewSalaryRangeId(),
                Min = 80.0,
                Max = 160.0
            };

            // Act
            await _salaryRangeRepository.CreateSalaryRange(newSalaryRange);

            // Assert
            var result = await _dbContext.SalaryRangesCollection.Find(sr => sr.Id == newSalaryRange.Id).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(newSalaryRange.Id, result.Id);
            Assert.Equal(newSalaryRange.Min, result.Min);
            Assert.Equal(newSalaryRange.Max, result.Max);

            await _dbContext.SalaryRangesCollection.DeleteOneAsync(sr => sr.Id == 5);
        }
    }
}