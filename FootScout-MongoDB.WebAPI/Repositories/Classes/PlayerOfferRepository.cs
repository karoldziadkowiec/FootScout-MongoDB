using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using System.Text;
using FootScout_MongoDB.WebAPI.DbManager;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class PlayerOfferRepository : IPlayerOfferRepository
    {
        private readonly MongoDBContext _dbContext;

        public PlayerOfferRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PlayerOffer> GetPlayerOffer(int playerOfferId)
        {
            return await _dbContext.PlayerOffersCollection
                .Find(po => po.Id == playerOfferId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetPlayerOffers()
        {
            return await _dbContext.PlayerOffersCollection
                .Find(_ => true)
                .SortByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerOffer>> GetActivePlayerOffers()
        {
            return await _dbContext.PlayerOffersCollection
                .Find(po => po.ClubAdvertisement.EndDate >= DateTime.Now)
                .SortByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetActivePlayerOfferCount()
        {
            return (int)await _dbContext.PlayerOffersCollection
                .CountDocumentsAsync(po => po.ClubAdvertisement.EndDate >= DateTime.Now);
        }

        public async Task<IEnumerable<PlayerOffer>> GetInactivePlayerOffers()
        {
            return await _dbContext.PlayerOffersCollection
                .Find(po => po.ClubAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(po => po.CreationDate)
                .ToListAsync();
        }

        public async Task CreatePlayerOffer(PlayerOffer playerOffer)
        {
            playerOffer.CreationDate = DateTime.Now;

            var offeredStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Offered")
                .FirstOrDefaultAsync();

            if (offeredStatus == null)
                throw new Exception("Offered status not found");

            playerOffer.OfferStatusId = offeredStatus.Id;
            playerOffer.OfferStatus = offeredStatus;

            await _dbContext.PlayerOffersCollection.InsertOneAsync(playerOffer);
        }

        public async Task UpdatePlayerOffer(PlayerOffer playerOffer)
        {
            var playerOfferFilter = Builders<PlayerOffer>.Filter.Eq(po => po.Id, playerOffer.Id);
            await _dbContext.PlayerOffersCollection.ReplaceOneAsync(playerOfferFilter, playerOffer);
        }

        public async Task DeletePlayerOffer(int playerOfferId)
        {
            var playerOfferFilter = Builders<PlayerOffer>.Filter.Eq(po => po.Id, playerOfferId);
            await _dbContext.PlayerOffersCollection.DeleteOneAsync(playerOfferFilter);
        }

        public async Task AcceptPlayerOffer(PlayerOffer playerOffer)
        {
            var acceptedStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Accepted")
                .FirstOrDefaultAsync();

            if (acceptedStatus == null)
                throw new Exception("Accepted status not found");

            var playerOfferFilter = Builders<PlayerOffer>.Filter.Eq(po => po.Id, playerOffer.Id);
            var updatedOffer = Builders<PlayerOffer>.Update
                .Set(po => po.OfferStatusId, acceptedStatus.Id)
                .Set(po => po.OfferStatus, acceptedStatus);

            await _dbContext.PlayerOffersCollection.UpdateOneAsync(playerOfferFilter, updatedOffer);
        }

        public async Task RejectPlayerOffer(PlayerOffer playerOffer)
        {
            var rejectedStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Rejected")
                .FirstOrDefaultAsync();

            if (rejectedStatus == null)
                throw new Exception("Rejected status not found");

            var playerOfferFilter = Builders<PlayerOffer>.Filter.Eq(po => po.Id, playerOffer.Id);
            var updatedOffer = Builders<PlayerOffer>.Update
                .Set(po => po.OfferStatusId, rejectedStatus.Id)
                .Set(po => po.OfferStatus, rejectedStatus);

            await _dbContext.PlayerOffersCollection.UpdateOneAsync(playerOfferFilter, updatedOffer);
        }

        public async Task<int> GetPlayerOfferStatusId(int clubAdvertisementId, string userId)
        {
            var playerOfferFilter = Builders<PlayerOffer>.Filter.And(
                Builders<PlayerOffer>.Filter.Eq(po => po.ClubAdvertisementId, clubAdvertisementId),
                Builders<PlayerOffer>.Filter.Eq(po => po.PlayerId, userId)
            );

            var playerOffer = await _dbContext.PlayerOffersCollection
                .Find(playerOfferFilter)
                .Project(po => po.OfferStatusId)
                .FirstOrDefaultAsync();

            return playerOffer;
        }

        public async Task<MemoryStream> ExportPlayerOffersToCsv()
        {
            var playerOffers = await GetPlayerOffers();
            var csv = new StringBuilder();
            csv.AppendLine("Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date");

            foreach (var offer in playerOffers)
            {
                csv.AppendLine($"{offer.OfferStatus.StatusName},{offer.Player.Email},{offer.Player.FirstName},{offer.Player.LastName},{offer.PlayerPosition.PositionName},{offer.Age},{offer.Height},{offer.PlayerFoot.FootName},{offer.Salary},{offer.AdditionalInformation},{offer.ClubAdvertisement.ClubMember.Email},{offer.ClubAdvertisement.ClubMember.FirstName},{offer.ClubAdvertisement.ClubMember.LastName},{offer.ClubAdvertisement.ClubName},{offer.ClubAdvertisement.League},{offer.ClubAdvertisement.Region}{offer.CreationDate:yyyy-MM-dd},{offer.ClubAdvertisement.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}