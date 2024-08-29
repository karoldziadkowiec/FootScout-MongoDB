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
            var clubOffer = await _dbContext.ClubOffersCollection
                .Find(co => co.Id == clubOfferId)
                .FirstOrDefaultAsync();

            if (clubOffer != null)
            {
                clubOffer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                    .Find(pa => pa.Id == clubOffer.PlayerAdvertisementId)
                    .FirstOrDefaultAsync();

                if (clubOffer.PlayerAdvertisement != null)
                {
                    clubOffer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubOffer.PlayerAdvertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == clubOffer.PlayerAdvertisement.PlayerFootId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == clubOffer.PlayerAdvertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                        .Find(p => p.Id == clubOffer.PlayerAdvertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }

                clubOffer.OfferStatus = await _dbContext.OfferStatusesCollection
                    .Find(os => os.Id == clubOffer.OfferStatusId)
                    .FirstOrDefaultAsync();

                clubOffer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubOffer.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubOffer.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubOffer.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return clubOffer;
        }

        public async Task<IEnumerable<ClubOffer>> GetClubOffers()
        {
            var clubOffers = await _dbContext.ClubOffersCollection
                .Find(_ => true)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();

            foreach (var clubOffer in clubOffers)
            {
                clubOffer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                    .Find(pa => pa.Id == clubOffer.PlayerAdvertisementId)
                    .FirstOrDefaultAsync();

                if (clubOffer.PlayerAdvertisement != null)
                {
                    clubOffer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubOffer.PlayerAdvertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == clubOffer.PlayerAdvertisement.PlayerFootId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == clubOffer.PlayerAdvertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                        .Find(p => p.Id == clubOffer.PlayerAdvertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }

                clubOffer.OfferStatus = await _dbContext.OfferStatusesCollection
                    .Find(os => os.Id == clubOffer.OfferStatusId)
                    .FirstOrDefaultAsync();

                clubOffer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubOffer.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubOffer.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubOffer.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return clubOffers;
        }

        public async Task<IEnumerable<ClubOffer>> GetActiveClubOffers()
        {
            var activeClubOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.EndDate >= DateTime.Now)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();

            foreach (var clubOffer in activeClubOffers)
            {
                clubOffer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                    .Find(pa => pa.Id == clubOffer.PlayerAdvertisementId)
                    .FirstOrDefaultAsync();

                if (clubOffer.PlayerAdvertisement != null)
                {
                    clubOffer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubOffer.PlayerAdvertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == clubOffer.PlayerAdvertisement.PlayerFootId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == clubOffer.PlayerAdvertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                        .Find(p => p.Id == clubOffer.PlayerAdvertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }

                clubOffer.OfferStatus = await _dbContext.OfferStatusesCollection
                    .Find(os => os.Id == clubOffer.OfferStatusId)
                    .FirstOrDefaultAsync();

                clubOffer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubOffer.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubOffer.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubOffer.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return activeClubOffers;
        }

        public async Task<int> GetActiveClubOfferCount()
        {
            return (int)await _dbContext.ClubOffersCollection
                .CountDocumentsAsync(co => co.PlayerAdvertisement.EndDate >= DateTime.Now);
        }

        public async Task<IEnumerable<ClubOffer>> GetInactiveClubOffers()
        {
            var inactiveClubOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.PlayerAdvertisement.EndDate < DateTime.Now)
                .SortByDescending(co => co.CreationDate)
                .ToListAsync();

            foreach (var clubOffer in inactiveClubOffers)
            {
                clubOffer.PlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                    .Find(pa => pa.Id == clubOffer.PlayerAdvertisementId)
                    .FirstOrDefaultAsync();

                if (clubOffer.PlayerAdvertisement != null)
                {
                    clubOffer.PlayerAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                        .Find(pp => pp.Id == clubOffer.PlayerAdvertisement.PlayerPositionId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.PlayerFoot = await _dbContext.PlayerFeetCollection
                        .Find(pf => pf.Id == clubOffer.PlayerAdvertisement.PlayerFootId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                        .Find(sr => sr.Id == clubOffer.PlayerAdvertisement.SalaryRangeId)
                        .FirstOrDefaultAsync();

                    clubOffer.PlayerAdvertisement.Player = await _dbContext.UsersCollection
                        .Find(p => p.Id == clubOffer.PlayerAdvertisement.PlayerId)
                        .FirstOrDefaultAsync();
                }

                clubOffer.OfferStatus = await _dbContext.OfferStatusesCollection
                    .Find(os => os.Id == clubOffer.OfferStatusId)
                    .FirstOrDefaultAsync();

                clubOffer.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubOffer.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubOffer.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubOffer.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return inactiveClubOffers;
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

            if (clubOffer.OfferStatus != null && clubOffer.OfferStatus.Id != 0)
            {
                var offerStatusFilter = Builders<OfferStatus>.Filter.Eq(os => os.Id, clubOffer.OfferStatus.Id);
                await _dbContext.OfferStatusesCollection.ReplaceOneAsync(offerStatusFilter, clubOffer.OfferStatus);
            }

            if (clubOffer.PlayerPosition != null && clubOffer.PlayerPosition.Id != 0)
            {
                var playerPositionFilter = Builders<PlayerPosition>.Filter.Eq(pp => pp.Id, clubOffer.PlayerPosition.Id);
                await _dbContext.PlayerPositionsCollection.ReplaceOneAsync(playerPositionFilter, clubOffer.PlayerPosition);
            }
        }

        public async Task DeleteClubOffer(int clubOfferId)
        {
            var filter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOfferId);
            await _dbContext.ClubOffersCollection.DeleteOneAsync(filter);
        }

        public async Task AcceptClubOffer(ClubOffer clubOffer)
        {
            var acceptedStatus = await _dbContext.OfferStatusesCollection
                .Find(os => os.StatusName == "Accepted")
                .FirstOrDefaultAsync();

            if (acceptedStatus == null)
                throw new Exception("Accepted status not found");

            var filter = Builders<ClubOffer>.Filter.Eq(co => co.Id, clubOffer.Id);
            var update = Builders<ClubOffer>.Update
                .Set(co => co.OfferStatusId, acceptedStatus.Id)
                .Set(co => co.OfferStatus, acceptedStatus);

            await _dbContext.ClubOffersCollection.UpdateOneAsync(filter, update);
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
