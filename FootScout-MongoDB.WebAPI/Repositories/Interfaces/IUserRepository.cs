using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;

namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindUserByEmail(string email);
        Task<IEnumerable<string>> GetRolesForUser(string userId);
        Task<IEnumerable<User>> GetUsersByRoleName(string roleName);
        Task<UserDTO> GetUser(string userId);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<IEnumerable<UserDTO>> GetOnlyUsers();
        Task<IEnumerable<UserDTO>> GetOnlyAdmins();
        Task<string> GetUserRole(string userId);
        Task<int> GetUserCount();
        Task UpdateUser(string userId, UserUpdateDTO userUpdateDTO);
        Task ResetUserPassword(string userId, UserResetPasswordDTO userUpdateDTO);
        Task DeleteUser(string userId);
        Task<IEnumerable<ClubHistory>> GetUserClubHistory(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserPlayerAdvertisements(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserActivePlayerAdvertisements(string userId);
        Task<IEnumerable<PlayerAdvertisement>> GetUserInactivePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserActiveFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<FavoritePlayerAdvertisement>> GetUserInactiveFavoritePlayerAdvertisements(string userId);
        Task<IEnumerable<ClubOffer>> GetReceivedClubOffers(string userId);
        Task<IEnumerable<ClubOffer>> GetSentClubOffers(string userId);
        Task<IEnumerable<ClubAdvertisement>> GetUserClubAdvertisements(string userId);
        Task<IEnumerable<ClubAdvertisement>> GetUserActiveClubAdvertisements(string userId);
        Task<IEnumerable<ClubAdvertisement>> GetUserInactiveClubAdvertisements(string userId);
        Task<IEnumerable<FavoriteClubAdvertisement>> GetUserFavoriteClubAdvertisements(string userId);
        Task<IEnumerable<FavoriteClubAdvertisement>> GetUserActiveFavoriteClubAdvertisements(string userId);
        Task<IEnumerable<FavoriteClubAdvertisement>> GetUserInactiveFavoriteClubAdvertisements(string userId);
        Task<IEnumerable<PlayerOffer>> GetReceivedPlayerOffers(string userId);
        Task<IEnumerable<PlayerOffer>> GetSentPlayerOffers(string userId);
        Task<IEnumerable<Chat>> GetUserChats(string userId);
        Task<MemoryStream> ExportUsersToCsv();
    }
}