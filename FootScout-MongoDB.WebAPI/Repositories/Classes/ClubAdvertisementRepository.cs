using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using System.Text;
using FootScout_MongoDB.WebAPI.DbManager;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class ClubAdvertisementRepository : IClubAdvertisementRepository
    {
        private readonly MongoDBContext _dbContext;

        public ClubAdvertisementRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClubAdvertisement> GetClubAdvertisement(int clubAdvertisementId)
        {
            var clubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.Id == clubAdvertisementId)
                .FirstOrDefaultAsync();

            if (clubAdvertisement == null) 
                return null;

            clubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                .Find(pp => pp.Id == clubAdvertisement.PlayerPositionId)
                .FirstOrDefaultAsync();

            clubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                .Find(sr => sr.Id == clubAdvertisement.SalaryRangeId)
                .FirstOrDefaultAsync();

            clubAdvertisement.ClubMember = await _dbContext.UsersCollection
                .Find(cm => cm.Id == clubAdvertisement.ClubMemberId)
                .FirstOrDefaultAsync();

            return clubAdvertisement;
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetAllClubAdvertisements()
        {
            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(FilterDefinition<ClubAdvertisement>.Empty)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();

            foreach (var clubAdvertisement in clubAdvertisements)
            {
                clubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubAdvertisement.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                    .Find(sr => sr.Id == clubAdvertisement.SalaryRangeId)
                    .FirstOrDefaultAsync();

                clubAdvertisement.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubAdvertisement.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return clubAdvertisements;
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetActiveClubAdvertisements()
        {
            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.EndDate >= DateTime.Now)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();

            foreach (var clubAdvertisement in clubAdvertisements)
            {
                clubAdvertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == clubAdvertisement.PlayerPositionId)
                    .FirstOrDefaultAsync();

                clubAdvertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                    .Find(sr => sr.Id == clubAdvertisement.SalaryRangeId)
                    .FirstOrDefaultAsync();

                clubAdvertisement.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == clubAdvertisement.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return clubAdvertisements;
        }

        public async Task<int> GetActiveClubAdvertisementCount()
        {
            return (int)await _dbContext.ClubAdvertisementsCollection
                .CountDocumentsAsync(ca => ca.EndDate >= DateTime.Now);
        }

        public async Task<IEnumerable<ClubAdvertisement>> GetInactiveClubAdvertisements()
        {
            var inactiveAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.EndDate < DateTime.Now)
                .SortByDescending(ca => ca.EndDate)
                .ToListAsync();

            foreach (var advertisement in inactiveAdvertisements)
            {
                advertisement.PlayerPosition = await _dbContext.PlayerPositionsCollection
                    .Find(pp => pp.Id == advertisement.PlayerPositionId)
                    .FirstOrDefaultAsync();

                advertisement.SalaryRange = await _dbContext.SalaryRangesCollection
                    .Find(sr => sr.Id == advertisement.SalaryRangeId)
                    .FirstOrDefaultAsync();

                advertisement.ClubMember = await _dbContext.UsersCollection
                    .Find(cm => cm.Id == advertisement.ClubMemberId)
                    .FirstOrDefaultAsync();
            }

            return inactiveAdvertisements;
        }

        public async Task CreateClubAdvertisement(ClubAdvertisement clubAdvertisement)
        {
            clubAdvertisement.CreationDate = DateTime.Now;
            clubAdvertisement.EndDate = DateTime.Now.AddDays(30);

            await _dbContext.ClubAdvertisementsCollection.InsertOneAsync(clubAdvertisement);
        }

        public async Task UpdateClubAdvertisement(ClubAdvertisement clubAdvertisement)
        {
            var clubAdvertisementFilter = Builders<ClubAdvertisement>.Filter.Eq(ca => ca.Id, clubAdvertisement.Id);
            await _dbContext.ClubAdvertisementsCollection.ReplaceOneAsync(clubAdvertisementFilter, clubAdvertisement);

            if (clubAdvertisement.PlayerPosition != null && clubAdvertisement.PlayerPosition.Id != 0)
            {
                var playerPositionFilter = Builders<PlayerPosition>.Filter.Eq(pp => pp.Id, clubAdvertisement.PlayerPosition.Id);
                await _dbContext.PlayerPositionsCollection.ReplaceOneAsync(playerPositionFilter, clubAdvertisement.PlayerPosition);
            }

            if (clubAdvertisement.SalaryRange != null && clubAdvertisement.SalaryRange.Id != 0)
            {
                var salaryRangeFilter = Builders<SalaryRange>.Filter.Eq(sr => sr.Id, clubAdvertisement.SalaryRange.Id);
                await _dbContext.SalaryRangesCollection.ReplaceOneAsync(salaryRangeFilter, clubAdvertisement.SalaryRange);
            }
        }

        public async Task DeleteClubAdvertisement(int clubAdvertisementId)
        {
            var clubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.Id == clubAdvertisementId)
                .FirstOrDefaultAsync();

            if (clubAdvertisement == null)
                throw new ArgumentException($"No Club Advertisement found with ID {clubAdvertisementId}");

            if (clubAdvertisement.SalaryRangeId != null)
            {
                await _dbContext.SalaryRangesCollection
                    .DeleteOneAsync(sr => sr.Id == clubAdvertisement.SalaryRangeId);
            }

            await _dbContext.FavoriteClubAdvertisementsCollection
                .DeleteManyAsync(fca => fca.ClubAdvertisementId == clubAdvertisementId);

            await _dbContext.PlayerOffersCollection
                .DeleteManyAsync(po => po.ClubAdvertisementId == clubAdvertisementId);

            await _dbContext.ClubAdvertisementsCollection
                .DeleteOneAsync(ca => ca.Id == clubAdvertisementId);
        }

        public async Task<MemoryStream> ExportClubAdvertisementsToCsv()
        {
            var clubAdvertisements = await GetAllClubAdvertisements();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Position,Club Name,League,Region,Min Salary,Max Salary,Creation Date,End Date");

            foreach (var ad in clubAdvertisements)
            {
                csv.AppendLine($"{ad.ClubMember.Email},{ad.ClubMember.FirstName},{ad.ClubMember.LastName},{ad.PlayerPosition.PositionName},{ad.ClubName},{ad.League},{ad.Region},{ad.SalaryRange.Min},{ad.SalaryRange.Max},{ad.CreationDate:yyyy-MM-dd},{ad.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}
