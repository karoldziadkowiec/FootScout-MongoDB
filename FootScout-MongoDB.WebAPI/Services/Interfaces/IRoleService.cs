using MongoDB.Bson;

namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface IRoleService
    {
        Task CreateNewRole(string roleName);
        Task AddRoleToUser(string userId, string roleName);
        Task RemoveRoleFromUser(string userId, string roleName);
        Task<bool> CheckRoleExists(string roleName);
    }
}