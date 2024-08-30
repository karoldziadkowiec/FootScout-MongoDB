using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using System.Text;
using FootScout_MongoDB.WebAPI.DbManager;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class ClubOfferRepository : IClubOfferRepository
    {
        private readonly MongoDBContext _dbContext;

        public ClubOfferRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubOffer> GetClubOffer(int clubOfferId)
        {
            return await _dbContext.ClubOffersCollection
                .Find(co => co.Id == clubOfferId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetClubOffers()
        {
            return await _dbContext.ClubOffersCollection
                .Find(_ => true)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClubOffer>> GetActiveClubOffers()
        {
            return await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.EndDate >= DateTime.Now)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveClubOfferCount()
        {
            return (int)await _dbContext.ClubOffersCollection
                .CountDocumentsAsync(co => co.PlayerAdvertisement.EndDate >= DateTime.Now);
        }

        public async Task<IEnumerable<ClubOffer>> GetInactiveClubOffers()
        {
            return await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();
        }

        public async Task CreateClubOffer(ClubOffer clubOffer)
        {
            clubOffer.CreationDate = DateTime.Now;

            var offeredStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Offered")
                .FirstOrDefaultAsync();

            if (offeredStatus == null)
                throw new Exception("Offered status not found");

            clubOffer.OfferStatusId = offeredStatus.Id;
            clubOffer.OfferStatus = offeredStatus;

            await _dbContext.ClubOffersCollection.InsertOneAsync(clubOffer);
        }

        public async Task UpdateClubOffer(ClubOffer clubOffer)
        {
            var clubOfferFilter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOffer.Id);
            await _dbContext.ClubOffersCollection.ReplaceOneAsync(clubOfferFilter, clubOffer);
        }

        public async Task DeleteClubOffer(int clubOfferId)
        {
            var clubOfferFilter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOfferId);
            await _dbContext.ClubOffersCollection.DeleteOneAsync(clubOfferFilter);
        }

        public async Task AcceptClubOffer(ClubOffer clubOffer)
        {
            var acceptedStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Accepted")
                .FirstOrDefaultAsync();

            if (acceptedStatus == null)
                throw new Exception("Accepted status not found");

            var clubOfferFilter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOffer.Id);
            var updatedOffer = Builders<ClubOffer>.Update
                .Set(co => co.OfferStatusId, acceptedStatus.Id)
                .Set(co => co.OfferStatus, acceptedStatus);

            await _dbContext.ClubOffersCollection.UpdateOneAsync(clubOfferFilter, updatedOffer);
        }

        public async Task RejectClubOffer(ClubOffer clubOffer)
        {
            var rejectedStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Rejected")
                .FirstOrDefaultAsync();

            if (rejectedStatus == null)
                throw new Exception("Rejected status not found");

            var filter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOffer.Id);
            var update = Builders<ClubOffer>.Update
                .Set(co => co.OfferStatusId, rejectedStatus.Id)
                .Set(co => co.OfferStatus, rejectedStatus);

            await _dbContext.ClubOffersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<int> GetClubOfferStatusId(int playerAdvertisementId, string userId)
        {
            var filter = Builders<ClubOffer>.Filter.And(
                Builders<ClubOffer>.Filter.Eq(co => co.PlayerAdvertisementId, playerAdvertisementId),
                Builders<ClubOffer>.Filter.Eq(co => co.ClubMemberId, userId)
            );

            var clubOffer = await _dbContext.ClubOffersCollection
                .Find(filter)
                .Project(co => co.OfferStatusId)
                .FirstOrDefaultAsync();

            return clubOffer;
        }

        public async Task<MemoryStream> ExportClubOffersToCsv()
        {
            var clubOffers = await GetClubOffers();
            var csv = new StringBuilder();
            csv.AppendLine("Offer Status,E-mail,First Name,Last Name,Position,Club Name,League,Region,Salary,Additional Information,Player's E-mail,Player's First Name,Player's Last Name,Age,Height,Foot,Creation Date,End Date");

            foreach (var offer in clubOffers)
            {
                csv.AppendLine($"{offer.OfferStatus.StatusName},{offer.ClubMember.Email},{offer.ClubMember.FirstName},{offer.ClubMember.LastName},{offer.PlayerPosition.PositionName},{offer.ClubName},{offer.League},{offer.Region},{offer.Salary},{offer.AdditionalInformation},{offer.PlayerAdvertisement.Player.Email},{offer.PlayerAdvertisement.Player.FirstName},{offer.PlayerAdvertisement.Player.LastName},{offer.PlayerAdvertisement.Age},{offer.PlayerAdvertisement.Height},{offer.PlayerAdvertisement.PlayerFoot.FootName}{offer.CreationDate:yyyy-MM-dd},{offer.PlayerAdvertisement.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}
