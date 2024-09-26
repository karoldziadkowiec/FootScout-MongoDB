using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class OfferStatusRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private OfferStatusRepository _offerStatusRepository;

        public OfferStatusRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _offerStatusRepository = new OfferStatusRepository(_dbContext);
        }

        [Fact]
        public async Task GetOfferStatuses_ReturnsAllOfferStatuses()
        {
            // Arrange & Act
            var result = await _offerStatusRepository.GetOfferStatuses();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsCorrectOfferStatus()
        {
            // Arrange
            var statusId = 1;

            // Act
            var result = await _offerStatusRepository.GetOfferStatus(statusId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Offered", result.StatusName);
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsNull_WhenStatusDoesNotExist()
        {
            // Arrange
            var statusId = 100;

            // Act
            var result = await _offerStatusRepository.GetOfferStatus(statusId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOfferStatusName_ReturnsCorrectStatusName()
        {
            // Arrange
            var statusId = 1;
            
            // Act
            var result = await _offerStatusRepository.GetOfferStatusName(statusId);

            // Assert
            Assert.Equal("Offered", result);
        }

        [Fact]
        public async Task GetOfferStatusName_ReturnsNull_WhenStatusIdDoesNotExist()
        {
            // Arrange
            var statusId = 100;

            // Act
            var result = await _offerStatusRepository.GetOfferStatusName(statusId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var statusName = "Accepted";

            // Act
            var result = await _offerStatusRepository.GetOfferStatusId(statusName);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetOfferStatusId_ReturnsZero_WhenStatusNameDoesNotExist()
        {
            // Arrange
            var statusName = "NonExistentStatus";

            // Act
            var result = await _offerStatusRepository.GetOfferStatusId(statusName);

            // Assert
            Assert.Equal(0, result);
        }
    }
}