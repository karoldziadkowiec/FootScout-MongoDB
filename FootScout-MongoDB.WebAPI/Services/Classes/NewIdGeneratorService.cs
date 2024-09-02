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
            var lastUserRole = await _dbContext.UserRolesCollection
                .Find(_ => true)
                .SortByDescending(ur => ur.Id)
                .FirstOrDefaultAsync();

            return (lastUserRole?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewOfferStatusId()
        {
            var lastOfferStatus = await _dbContext.OfferStatusesCollection
                .Find(_ => true)
                .SortByDescending(os => os.Id)
                .FirstOrDefaultAsync();

            return (lastOfferStatus?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerFootId()
        {
            var lastPlayerFoot = await _dbContext.PlayerFeetCollection
                .Find(_ => true)
                .SortByDescending(pf => pf.Id)
                .FirstOrDefaultAsync();

            return (lastPlayerFoot?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerPositionId()
        {
            var lastPlayerPosition = await _dbContext.PlayerPositionsCollection
                .Find(_ => true)
                .SortByDescending(pp => pp.Id)
                .FirstOrDefaultAsync();

            return (lastPlayerPosition?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewAchievementsId()
        {
            var lastAchievements = await _dbContext.AchievementsCollection
                .Find(_ => true)
                .SortByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            return (lastAchievements?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubHistoryId()
        {
            var lastClubHistory = await _dbContext.ClubHistoriesCollection
                .Find(_ => true)
                .SortByDescending(ch => ch.Id)
                .FirstOrDefaultAsync();

            return (lastClubHistory?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewSalaryRangeId()
        {
            var lastSalaryRange = await _dbContext.SalaryRangesCollection
                .Find(_ => true)
                .SortByDescending(sr => sr.Id)
                .FirstOrDefaultAsync();

            return (lastSalaryRange?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerAdvertisementId()
        {
            var lastPlayerAdvertisement = await _dbContext.PlayerAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(pa => pa.Id)
                .FirstOrDefaultAsync();

            return (lastPlayerAdvertisement?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubOfferId()
        {
            var lastClubOffer = await _dbContext.ClubOffersCollection
                .Find(_ => true)
                .SortByDescending(co => co.Id)
                .FirstOrDefaultAsync();

            return (lastClubOffer?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewFavoritePlayerAdvertisementId()
        {
            var lastFavoritePlayerAdvertisement = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(fpa => fpa.Id)
                .FirstOrDefaultAsync();

            return (lastFavoritePlayerAdvertisement?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewClubAdvertisementId()
        {
            var lastClubAdvertisement = await _dbContext.ClubAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(ca => ca.Id)
                .FirstOrDefaultAsync();

            return (lastClubAdvertisement?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewPlayerOfferId()
        {
            var lastPlayerOffer = await _dbContext.PlayerOffersCollection
                .Find(_ => true)
                .SortByDescending(po => po.Id)
                .FirstOrDefaultAsync();

            return (lastPlayerOffer?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewFavoriteClubAdvertisementId()
        {
            var lastFavoriteClubAdvertisement = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(_ => true)
                .SortByDescending(fca => fca.Id)
                .FirstOrDefaultAsync();

            return (lastFavoriteClubAdvertisement?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewProblemId()
        {
            var lastProblem = await _dbContext.ProblemsCollection
                .Find(_ => true)
                .SortByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            return (lastProblem?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewChatId()
        {
            var lastChat = await _dbContext.ChatsCollection
                .Find(_ => true)
                .SortByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            return (lastChat?.Id ?? 0) + 1;
        }

        public async Task<int> GenerateNewMessageId()
        {
            var lastMessage = await _dbContext.MessagesCollection
                .Find(_ => true)
                .SortByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            return (lastMessage?.Id ?? 0) + 1;
        }
    }
}