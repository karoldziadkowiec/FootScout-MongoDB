using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class ClubOfferRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private ClubOfferRepository _clubOfferRepository;

        public ClubOfferRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _clubOfferRepository = new ClubOfferRepository(_dbContext);
        }

        [Fact]
        public async Task GetClubOffer_ReturnsCorrectClubOffer()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetClubOffer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Manchester City", result.ClubName);
            Assert.Equal("Premier League", result.League);
            Assert.Equal("England", result.Region);
        }

        [Fact]
        public async Task GetClubOffers_ReturnsAllClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var firstOffer = result.First();
            Assert.Equal(2, result.ToList().Count);
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsActiveClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetActiveClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetActiveClubOfferCount_ReturnsCountOfActiveClubOffers()
        {
            // Arrange &  Act
            var result = await _clubOfferRepository.GetActiveClubOfferCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsInactiveClubOffers()
        {
            // Arrange & Act
            var result = await _clubOfferRepository.GetInactiveClubOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.True(co.PlayerAdvertisement.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task CreateClubOffer_AddsClubOfferToDatabase()
        {
            // Arrange
            var newClubOffer = new ClubOffer
            {
                Id = await _newIdGeneratorService.GenerateNewClubOfferId(),
                PlayerAdvertisementId = 1,
                PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == 1).FirstOrDefaultAsync(),
                PlayerPositionId = 5,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 5).FirstOrDefaultAsync(),
                ClubName = "New ClubName",
                League = "New League",
                Region = "New Region",
                Salary = 200,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                ClubMemberId = "pepguardiola",
                ClubMember = await _dbContext.UsersCollection.Find(u => u.Id == "pepguardiola").FirstOrDefaultAsync()

            };

            // Act
            await _clubOfferRepository.CreateClubOffer(newClubOffer);

            var result = await _dbContext.ClubOffersCollection.Find(co => co.ClubName == "New ClubName").FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New ClubName", result.ClubName);

            await _dbContext.ClubOffersCollection.DeleteOneAsync(co => co.Id == result.Id);
        }

        [Fact]
        public async Task UpdateClubOffer_UpdatesClubOfferInDatabase()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();
            clubOffer.OfferStatusId = 2;

            // Act
            await _clubOfferRepository.UpdateClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffersCollection.Find(co => co.Id == clubOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal(2, updatedOffer.OfferStatusId);
        }

        [Fact]
        public async Task DeleteClubOffer_DeletesClubOfferFromDatabase()
        {
            // Arrange
            await _dbContext.ClubOffersCollection.InsertOneAsync(new ClubOffer
            {
                Id = await _newIdGeneratorService.GenerateNewClubOfferId(),
                PlayerAdvertisementId = 1,
                PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection.Find(pa => pa.Id == 1).FirstOrDefaultAsync(),
                OfferStatusId = 1,
                OfferStatus = await _dbContext.OfferStatusesCollection.Find(os => os.Id == 1).FirstOrDefaultAsync(),
                PlayerPositionId = 11,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 5).FirstOrDefaultAsync(),
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 260,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                ClubMemberId = "pepguardiola",
                ClubMember = await _dbContext.UsersCollection.Find(u => u.Id == "pepguardiola").FirstOrDefaultAsync()
            });

            var offerToDelete = await _dbContext.ClubOffersCollection.Find(co => co.PlayerAdvertisementId == 1 && co.ClubName == "Juventus Turyn" && co.League == "Serie A" && co.Region == "Italy" && co.ClubMemberId == "pepguardiola").FirstOrDefaultAsync();

            // Act
            await _clubOfferRepository.DeleteClubOffer(offerToDelete.Id);

            // Assert
            var deletedOffer = await _dbContext.ClubOffersCollection.Find(co => co.Id == offerToDelete.Id).FirstOrDefaultAsync();

            Assert.Null(deletedOffer);
        }

        [Fact]
        public async Task AcceptClubOffer_UpdatesOfferStatusToAccepted()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            await _clubOfferRepository.AcceptClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffersCollection.Find(co => co.Id == clubOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal("Accepted", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task RejectClubOffer_UpdatesOfferStatusToRejected()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            await _clubOfferRepository.RejectClubOffer(clubOffer);

            // Assert
            var updatedOffer = await _dbContext.ClubOffersCollection.Find(co => co.Id == clubOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal("Rejected", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task GetClubOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var clubOffer = await _dbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();
            var playerAdvertisementId = clubOffer.PlayerAdvertisementId;
            var clubMemberId = clubOffer.ClubMemberId;

            // Act
            var result = await _clubOfferRepository.GetClubOfferStatusId(playerAdvertisementId, clubMemberId);

            // Assert
            Assert.Equal(clubOffer.OfferStatusId, result);
        }

        [Fact]
        public async Task ExportClubOffersToCsv_ReturnsValidCsvStream()
        {
            // Arrange && Act
            var csvStream = await _clubOfferRepository.ExportClubOffersToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("Offer Status,E-mail,First Name,Last Name,Position,Club Name,League,Region,Salary,Additional Information,Player's E-mail,Player's First Name,Player's Last Name,Age,Height,Foot,Creation Date,End Date", csvContent);
                Assert.Contains("pg8@gmail.com,Pep,Guardiola", csvContent);
            }
        }
    }
}