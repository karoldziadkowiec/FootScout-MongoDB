using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using MongoDB.Driver;
using System.Text;

namespace FootScout_MongoDB.WebAPI.Repositories.Classes
{
    public class PlayerAdvertisementRepository : IPlayerAdvertisementRepository
    {
        private readonly MongoDBContext _dbContext;

        public PlayerAdvertisementRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PlayerAdvertisement> GetPlayerAdvertisement(int playerAdvertisementId)
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.Id == playerAdvertisementId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetAllPlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(FilterDefinition<PlayerAdvertisement>.Empty)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetActivePlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.EndDate >= DateTime.Now)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task<int> GetActivePlayerAdvertisementCount()
        {
            return (int)await _dbContext.PlayerAdvertisementsCollection
                .CountDocumentsAsync(pa => pa.EndDate >= DateTime.Now);
        }

        public async Task<IEnumerable<PlayerAdvertisement>> GetInactivePlayerAdvertisements()
        {
            return await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.EndDate < DateTime.Now)
                .SortByDescending(pa => pa.EndDate)
                .ToListAsync();
        }

        public async Task CreatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement)
        {
            playerAdvertisement.CreationDate = DateTime.Now;
            playerAdvertisement.EndDate = DateTime.Now.AddDays(30);

            await _dbContext.PlayerAdvertisementsCollection.InsertOneAsync(playerAdvertisement);
        }

        public async Task UpdatePlayerAdvertisement(PlayerAdvertisement playerAdvertisement)
        {
            var playerAdvertisementFilter = Builders<PlayerAdvertisement>.Filter.Eq(pa => pa.Id, playerAdvertisement.Id);

            if (playerAdvertisementFilter != null)
            {
                await _dbContext.PlayerAdvertisementsCollection.ReplaceOneAsync(playerAdvertisementFilter, playerAdvertisement);

                if (playerAdvertisement.SalaryRange != null && playerAdvertisement.SalaryRange.Id != 0)
                {
                    var salaryRangeFilter = Builders<SalaryRange>.Filter.Eq(sr => sr.Id, playerAdvertisement.SalaryRange.Id);
                    await _dbContext.SalaryRangesCollection.ReplaceOneAsync(salaryRangeFilter, playerAdvertisement.SalaryRange);
                }

                var favPlayerAdvertisementsFilter = Builders<FavoritePlayerAdvertisement>.Filter.Eq(fpa => fpa.PlayerAdvertisementId, playerAdvertisement.Id);
                var favPlayerAdvertisementsUpdate = Builders<FavoritePlayerAdvertisement>.Update
                    .Set(fpa => fpa.PlayerAdvertisement, playerAdvertisement);
                await _dbContext.FavoritePlayerAdvertisementsCollection.UpdateManyAsync(favPlayerAdvertisementsFilter, favPlayerAdvertisementsUpdate);

                var clubOffersFilter = Builders<ClubOffer>.Filter.Eq(co => co.PlayerAdvertisementId, playerAdvertisement.Id);
                var clubOffersUpdate = Builders<ClubOffer>.Update
                    .Set(co => co.PlayerAdvertisement, playerAdvertisement);
                await _dbContext.ClubOffersCollection.UpdateManyAsync(clubOffersFilter, clubOffersUpdate);

            }
        }

        public async Task DeletePlayerAdvertisement(int playerAdvertisementId)
        {
            var playerAdvertisement = await GetPlayerAdvertisement(playerAdvertisementId);

            if (playerAdvertisement == null)
                throw new ArgumentException($"No Player Advertisement found with ID {playerAdvertisementId}");

            if (playerAdvertisement.SalaryRangeId != 0)
            {
                await _dbContext.SalaryRangesCollection
                    .DeleteOneAsync(sr => sr.Id == playerAdvertisement.SalaryRangeId);
            }

            await _dbContext.FavoritePlayerAdvertisementsCollection
                .DeleteManyAsync(fpa => fpa.PlayerAdvertisementId == playerAdvertisementId);

            await _dbContext.ClubOffersCollection
                .DeleteManyAsync(co => co.PlayerAdvertisementId == playerAdvertisementId);

            await _dbContext.PlayerAdvertisementsCollection
                .DeleteOneAsync(pa => pa.Id == playerAdvertisementId);
        }

        public async Task<MemoryStream> ExportPlayerAdvertisementsToCsv()
        {
            var playerAdvertisements = await GetAllPlayerAdvertisements();
            var csv = new StringBuilder();
            csv.AppendLine("E-mail,First Name,Last Name,Position,League,Region,Age,Height,Foot,Min Salary,Max Salary,Creation Date,End Date");

            foreach (var ad in playerAdvertisements)
            {
                csv.AppendLine($"{ad.Player.Email},{ad.Player.FirstName},{ad.Player.LastName},{ad.PlayerPosition.PositionName},{ad.League},{ad.Region},{ad.Age},{ad.Height},{ad.PlayerFoot.FootName},{ad.SalaryRange.Min},{ad.SalaryRange.Max},{ad.CreationDate:yyyy-MM-dd},{ad.EndDate:yyyy-MM-dd}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}