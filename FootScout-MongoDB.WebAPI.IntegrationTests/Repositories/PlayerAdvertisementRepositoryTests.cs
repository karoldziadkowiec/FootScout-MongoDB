using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class PlayerAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private PlayerAdvertisementRepository _playerAdvertisementRepository;

        public PlayerAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _playerAdvertisementRepository = new PlayerAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerAdvertisement_ReturnsCorrectPlayerAdvertisement()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetPlayerAdvertisement(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetAllPlayerAdvertisements_ReturnsAllPlayerAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetAllPlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First().CreationDate >= result.Last().CreationDate);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetActivePlayerAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetActivePlayerAdvertisementCount();

            // Assert
            var expectedCount = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.EndDate >= DateTime.Now).CountAsync();

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetInactivePlayerAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange & Act
            var result = await _playerAdvertisementRepository.GetInactivePlayerAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            await _dbContext.SalaryRangesCollection.InsertOneAsync(new SalaryRange { Id = await _newIdGeneratorService.GenerateNewSalaryRangeId(), Min = 250, Max = 300 });

            var salaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Min == 250 && sr.Max == 300).FirstOrDefaultAsync();

            var newAd = new PlayerAdvertisement
            {
                Id = await _newIdGeneratorService.GenerateNewPlayerAdvertisementId(),
                PlayerPositionId = 12,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 12).FirstOrDefaultAsync(),
                League = "Serie A",
                Region = "Italy",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = await _dbContext.PlayerFeetCollection.Find(pf => pf.Id == 1).FirstOrDefaultAsync(),
                SalaryRangeId = salaryRange.Id,
                SalaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Id == salaryRange.Id).FirstOrDefaultAsync(),
                CreationDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PlayerId = "leomessi",
                Player = await _dbContext.UsersCollection.Find(u => u.Id == "leomessi").FirstOrDefaultAsync()
            };

            // Act
            await _playerAdvertisementRepository.CreatePlayerAdvertisement(newAd);

            var result = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == newAd.Id).FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAd.League, result.League);
            Assert.Equal(newAd.Region, result.Region);
            Assert.True(result.CreationDate <= DateTime.Now);
            Assert.True(result.EndDate > DateTime.Now);

            await _dbContext.PlayerAdvertisementsCollection.DeleteOneAsync(po => po.Id == result.Id);
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var advertisementToUpdate = await _dbContext.PlayerAdvertisementsCollection.Find(_ => true).FirstOrDefaultAsync();
            advertisementToUpdate.Height = 168;

            // Act
            await _playerAdvertisementRepository.UpdatePlayerAdvertisement(advertisementToUpdate);

            // Assert
            var updatedAd = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == advertisementToUpdate.Id).FirstOrDefaultAsync();
            Assert.NotNull(updatedAd);
            Assert.Equal(168, updatedAd.Height);
        }

        [Fact]
        public async Task DeletePlayerAdvertisement_RemovesPlayerAdvertisementAndRelatedEntities()
        {
            // Arrange
            await _dbContext.SalaryRangesCollection.InsertOneAsync(new SalaryRange { Min = 350, Max = 400 });

            var salaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Min == 350 && sr.Max == 400).FirstOrDefaultAsync();

            await _dbContext.PlayerAdvertisementsCollection.InsertOneAsync(new PlayerAdvertisement
            {
                PlayerPositionId = 11,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 11).FirstOrDefaultAsync(),
                League = "Bundesliga",
                Region = "Germany",
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = await _dbContext.PlayerFeetCollection.Find(pf => pf.Id == 1).FirstOrDefaultAsync(),
                SalaryRangeId = salaryRange.Id,
                SalaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Id == salaryRange.Id).FirstOrDefaultAsync(),
                CreationDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PlayerId = "leomessi",
                Player = await _dbContext.UsersCollection.Find(u => u.Id == "leomessi").FirstOrDefaultAsync()
            });

            var advertisementToDelete = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.League == "Bundesliga" && pa.Region == "Germany" && pa.PlayerId == "leomessi").FirstOrDefaultAsync();

            // Act
            await _playerAdvertisementRepository.DeletePlayerAdvertisement(advertisementToDelete.Id);

            // Assert
            var deletedAdvertisement = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == advertisementToDelete.Id).FirstOrDefaultAsync();
            Assert.Null(deletedAdvertisement);
        }

        [Fact]
        public async Task ExportPlayerAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange & Act
            var csvStream = await _playerAdvertisementRepository.ExportPlayerAdvertisementsToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("E-mail,First Name,Last Name,Position,League,Region,Age,Height,Foot,Min Salary,Max Salary,Creation Date,End Date", csvContent);
                Assert.Contains("lm10@gmail.com,Leo,Messi,Striker", csvContent);
            }
        }
    }
}