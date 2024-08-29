using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class NewIdGeneratorService : INewIdGeneratorService
    {
        private readonly MongoDBContext _dbContext;

        public NewIdGeneratorService(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GenerateNewUserRoleId()
        {
            var lastItem = await _dbContext.UserRolesCollection
                .Find(_ => true)
                .SortByDescending(ur => ur.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewOfferStatusId()
        {
            var lastItem = await _dbContext.OfferStatusesCollection
                .Find(_ => true)
                .SortByDescending(os => os.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerFootId()
        {
            var lastItem = await _dbContext.PlayerFeetCollection
                .Find(_ => true)
                .SortByDescending(pf => pf.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerPositionId()
        {
            var lastItem = await _dbContext.PlayerPositionsCollection
                .Find(_ => true)
                .SortByDescending(pp => pp.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewAchievementsId()
        {
            var lastItem = await _dbContext.AchievementsCollection
                .Find(_ => true)
                .SortByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubHistoryId()
        {
            var lastItem = await _dbContext.ClubHistoriesCollection
                .Find(_ => true)
                .SortByDescending(ch => ch.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewSalaryRangeId()
        {
            var lastItem = await _dbContext.ClubHistoriesCollection
                .Find(_ => true)
                .SortByDescending(sr => sr.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerAdvertisementId()
        {
            var lastItem = await _dbContext.PlayerAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(pa => pa.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewFavoritePlayerAdvertisementId()
        {
            var lastItem = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(fpa => fpa.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubOfferId()
        {
            var lastItem = await _dbContext.ClubOffersCollection
                .Find(_ => true)
                .SortByDescending(co => co.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubAdvertisementId()
        {
            var lastItem = await _dbContext.ClubAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(ca => ca.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewFavoriteClubAdvertisementId()
        {
            var lastItem = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(fca => fca.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerOfferId()
        {
            var lastItem = await _dbContext.PlayerOffersCollection
                .Find(_ => true)
                .SortByDescending(po => po.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewProblemId()
        {
            var lastItem = await _dbContext.ProblemsCollection
                .Find(_ => true)
                .SortByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewChatId()
        {
            var lastItem = await _dbContext.ChatsCollection
                .Find(_ => true)
                .SortByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewMessageId()
        {
            var lastItem = await _dbContext.MessagesCollection
                .Find(_ => true)
                .SortByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            return (lastItem?.Id ?? 0) + 1;
        }
    }
}