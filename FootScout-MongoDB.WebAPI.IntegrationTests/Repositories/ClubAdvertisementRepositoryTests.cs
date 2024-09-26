using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class ClubAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private ClubAdvertisementRepository _clubAdvertisementRepository;

        public ClubAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _clubAdvertisementRepository = new ClubAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubAdvertisement_ReturnsCorrectClubAdvertisement()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetClubAdvertisement(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Manchester City", result.ClubName);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetAllClubAdvertisements_ReturnsAllClubAdvertisementsOrderedDESCByEndDate()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetAllClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First().CreationDate >= result.Last().CreationDate);
        }

        [Fact]
        public async Task GetActiveClubAdvertisements_ReturnsActiveAdvertisements()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetActiveClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetActiveClubAdvertisementCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetActiveClubAdvertisementCount();

            // Assert
            var expectedCount = await _dbContext.ClubAdvertisementsCollection.Find(pa => pa.EndDate >= DateTime.Now).CountAsync();

            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetInactiveClubAdvertisements_ReturnsInactiveAdvertisements()
        {
            // Arrange & Act
            var result = await _clubAdvertisementRepository.GetInactiveClubAdvertisements();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ad => Assert.True(ad.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task CreateClubAdvertisement_SuccessfullyCreatesAdvertisement()
        {
            // Arrange
            await _dbContext.SalaryRangesCollection.InsertOneAsync(new SalaryRange { Id = await _newIdGeneratorService.GenerateNewUserRoleId(), Min = 550, Max = 600 });

            var salaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Min == 550 && sr.Max == 600).FirstOrDefaultAsync();

            var newAd = new ClubAdvertisement
            {
                Id = await _newIdGeneratorService.GenerateNewClubAdvertisementId(),
                PlayerPositionId = 1,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 1).FirstOrDefaultAsync(),
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                SalaryRangeId = salaryRange.Id,
                SalaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Id == salaryRange.Id).FirstOrDefaultAsync(),
                CreationDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ClubMemberId = "pepguardiola",
                ClubMember = await _dbContext.UsersCollection.Find(u => u.Id == "pepguardiola").FirstOrDefaultAsync()
            };

            // Act
            await _clubAdvertisementRepository.CreateClubAdvertisement(newAd);

            var result = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == newAd.Id).FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAd.ClubName, result.ClubName);
            Assert.Equal(newAd.League, result.League);
            Assert.Equal(newAd.Region, result.Region);
            Assert.True(result.CreationDate <= DateTime.Now);
            Assert.True(result.EndDate > DateTime.Now);

            await _dbContext.ClubAdvertisementsCollection.DeleteOneAsync(ca => ca.Id == result.Id);
        }

        [Fact]
        public async Task UpdateClubAdvertisement_SuccessfullyUpdatesAdvertisement()
        {
            // Arrange
            var advertisementToUpdate = await _dbContext.ClubAdvertisementsCollection.Find(_ => true).FirstOrDefaultAsync();
            advertisementToUpdate.PlayerPositionId = 12;

            // Act
            await _clubAdvertisementRepository.UpdateClubAdvertisement(advertisementToUpdate);

            // Assert
            var updatedAd = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == advertisementToUpdate.Id).FirstOrDefaultAsync();
            Assert.NotNull(updatedAd);
            Assert.Equal(12, updatedAd.PlayerPositionId);
        }

        [Fact]
        public async Task DeleteClubAdvertisement_RemovesClubAdvertisementAndRelatedEntities()
        {
            // Arrange
            await _dbContext.SalaryRangesCollection.InsertOneAsync(new SalaryRange { Min = 650, Max = 700 });

            var salaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Min == 650 && sr.Max == 700).FirstOrDefaultAsync();

            await _dbContext.ClubAdvertisementsCollection.InsertOneAsync(new ClubAdvertisement
            {
                PlayerPositionId = 1,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 1).FirstOrDefaultAsync(),
                ClubName = "Bayern Monachium",
                League = "Bundesliga",
                Region = "Germany",
                SalaryRangeId = salaryRange.Id,
                SalaryRange = await _dbContext.SalaryRangesCollection.Find(sr => sr.Id == salaryRange.Id).FirstOrDefaultAsync(),
                CreationDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                ClubMemberId = "pepguardiola",
                ClubMember = await _dbContext.UsersCollection.Find(u => u.Id == "pepguardiola").FirstOrDefaultAsync()
            });

            var advertisementToDelete = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.League == "Bundesliga" && ca.Region == "Germany" && ca.ClubMemberId == "pepguardiola").FirstOrDefaultAsync();

            // Act
            await _clubAdvertisementRepository.DeleteClubAdvertisement(advertisementToDelete.Id);

            // Assert
            var deletedAdvertisement = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == advertisementToDelete.Id).FirstOrDefaultAsync();
            Assert.Null(deletedAdvertisement);
        }

        [Fact]
        public async Task ExportClubAdvertisementsToCsv_ReturnsCsvFile()
        {
            // Arrange & Act
            var csvStream = await _clubAdvertisementRepository.ExportClubAdvertisementsToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("E-mail,First Name,Last Name,Position,Club Name,League,Region,Min Salary,Max Salary,Creation Date,End Date", csvContent);
                Assert.Contains("pg8@gmail.com,Pep,Guardiola,Striker", csvContent);
            }
        }
    }
}