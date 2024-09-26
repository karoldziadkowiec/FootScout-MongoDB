using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Services.Classes;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class PlayerOfferRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private PlayerOfferRepository _playerOfferRepository;

        public PlayerOfferRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            _playerOfferRepository = new PlayerOfferRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerOffer_ReturnsCorrectPlayerOffer()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetPlayerOffer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("lm10@gmail.com", result.Player.Email);
            Assert.Equal(1, result.PlayerFootId);
        }

        [Fact]
        public async Task GetPlayerOffers_ReturnsAllPlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetPlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var firstOffer = result.First();
            Assert.Equal(15, firstOffer.PlayerPositionId);
            Assert.Equal(2, result.ToList().Count);
        }

        [Fact]
        public async Task GetActivePlayerOffers_ReturnsActivePlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetActivePlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.True(po.ClubAdvertisement.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetActivePlayerOfferCount_ReturnsCountOfActivePlayerOffers()
        {
            // Arrange &  Act
            var result = await _playerOfferRepository.GetActivePlayerOfferCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetInactivePlayerOffers_ReturnsInactivePlayerOffers()
        {
            // Arrange & Act
            var result = await _playerOfferRepository.GetInactivePlayerOffers();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.True(po.ClubAdvertisement.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task CreatePlayerOffer_AddsPlayerOfferToDatabase()
        {
            // Arrange
            var newPlayerOffer = new PlayerOffer
            {
                Id = await _newIdGeneratorService.GenerateNewPlayerOfferId(),
                ClubAdvertisementId = 2,
                ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == 2).FirstOrDefaultAsync(),
                PlayerPositionId = 7,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 7).FirstOrDefaultAsync(),
                Age = 37,
                Height = 167,
                PlayerFootId = 1,
                PlayerFoot = await _dbContext.PlayerFeetCollection.Find(pf => pf.Id == 1).FirstOrDefaultAsync(),
                Salary = 180,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                PlayerId = "leomessi",
                Player = await _dbContext.UsersCollection.Find(u => u.Id == "leomessi").FirstOrDefaultAsync()
            };

            // Act
            await _playerOfferRepository.CreatePlayerOffer(newPlayerOffer);

            var result = await _dbContext.PlayerOffersCollection.Find(po => po.Id == newPlayerOffer.Id).FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("leomessi", result.Player.Id);
            Assert.Equal(7, result.PlayerPositionId);

            await _dbContext.PlayerOffersCollection.DeleteOneAsync(po => po.Id == newPlayerOffer.Id);
        }

        [Fact]
        public async Task UpdatePlayerOffer_UpdatesPlayerOfferInDatabase()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffersCollection.Find(_ => true).FirstOrDefaultAsync();
            playerOffer.OfferStatusId = 2;

            // Act
            await _playerOfferRepository.UpdatePlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffersCollection.Find(po => po.Id == playerOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal(2, updatedOffer.OfferStatusId);
        }

        [Fact]
        public async Task DeletePlayerOffer_DeletesPlayerOfferFromDatabase()
        {
            // Arrange
            await _dbContext.PlayerOffersCollection.InsertOneAsync(new PlayerOffer
            {
                Id = await _newIdGeneratorService.GenerateNewPlayerOfferId(),
                ClubAdvertisementId = 2,
                ClubAdvertisement = await _dbContext.ClubAdvertisementsCollection.Find(ca => ca.Id == 2).FirstOrDefaultAsync(),
                OfferStatusId = 1,
                OfferStatus = await _dbContext.OfferStatusesCollection.Find(os => os.Id == 1).FirstOrDefaultAsync(),
                PlayerPositionId = 8,
                PlayerPosition = await _dbContext.PlayerPositionsCollection.Find(pp => pp.Id == 8).FirstOrDefaultAsync(),
                Age = 37,
                Height = 168,
                PlayerFootId = 1,
                PlayerFoot = await _dbContext.PlayerFeetCollection.Find(pf => pf.Id == 1).FirstOrDefaultAsync(),
                Salary = 280,
                AdditionalInformation = "no info",
                CreationDate = DateTime.Now,
                PlayerId = "leomessi",
                Player = await _dbContext.UsersCollection.Find(u => u.Id == "leomessi").FirstOrDefaultAsync()
            });

            var offerToDelete = await _dbContext.PlayerOffersCollection.Find(po => po.ClubAdvertisementId == 2 && po.PlayerPositionId == 8 && po.Height == 168 && po.Salary == 280 && po.PlayerId == "leomessi").FirstOrDefaultAsync();

            // Act
            await _playerOfferRepository.DeletePlayerOffer(offerToDelete.Id);

            // Assert
            var deletedOffer = await _dbContext.PlayerOffersCollection.Find(po => po.Id == offerToDelete.Id).FirstOrDefaultAsync();

            Assert.Null(deletedOffer);
        }

        [Fact]
        public async Task AcceptPlayerOffer_UpdatesOfferStatusToAccepted()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            await _playerOfferRepository.AcceptPlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffersCollection.Find(po => po.Id == playerOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal("Accepted", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task RejectPlayerOffer_UpdatesOfferStatusToRejected()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            await _playerOfferRepository.RejectPlayerOffer(playerOffer);

            // Assert
            var updatedOffer = await _dbContext.PlayerOffersCollection.Find(po => po.Id == playerOffer.Id).FirstOrDefaultAsync();

            Assert.NotNull(updatedOffer);
            Assert.Equal("Rejected", updatedOffer.OfferStatus.StatusName);
        }

        [Fact]
        public async Task GetPlayerOfferStatusId_ReturnsCorrectStatusId()
        {
            // Arrange
            var playerOffer = await _dbContext.PlayerOffersCollection.Find(_ => true).FirstOrDefaultAsync();
            var clubAdvertisementId = playerOffer.ClubAdvertisementId;
            var playerId = playerOffer.PlayerId;

            // Act
            var result = await _playerOfferRepository.GetPlayerOfferStatusId(clubAdvertisementId, playerId);

            // Assert
            Assert.Equal(playerOffer.OfferStatusId, result);
        }

        [Fact]
        public async Task ExportPlayerOffersToCsv_ReturnsValidCsvStream()
        {
            // Arrange && Act
            var csvStream = await _playerOfferRepository.ExportPlayerOffersToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date", csvContent);
                Assert.Contains("lm10@gmail.com,Leo,Messi", csvContent);
            }
        }
    }
}