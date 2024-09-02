namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface INewIdGeneratorService
    {
        Task<int> GenerateNewUserRoleId();
        Task<int> GenerateNewOfferStatusId();
        Task<int> GenerateNewPlayerFootId();
        Task<int> GenerateNewPlayerPositionId();
        Task<int> GenerateNewAchievementsId();
        Task<int> GenerateNewClubHistoryId();
        Task<int> GenerateNewSalaryRangeId();
        Task<int> GenerateNewPlayerAdvertisementId();
        Task<int> GenerateNewClubOfferId();
        Task<int> GenerateNewFavoritePlayerAdvertisementId();
        Task<int> GenerateNewClubAdvertisementId();
        Task<int> GenerateNewPlayerOfferId();
        Task<int> GenerateNewFavoriteClubAdvertisementId();
        Task<int> GenerateNewProblemId();
        Task<int> GenerateNewChatId();
        Task<int> GenerateNewMessageId();
    }
}