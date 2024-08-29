using FootScout_MongoDB.WebAPI.Models.DTOs;

namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task Register(RegisterDTO registerDTO);
        Task<string> Login(LoginDTO loginDTO);
        Task<IEnumerable<string>> GetRoles();
        Task MakeAnAdmin(string userId);
        Task MakeAnUser(string userId);
    }
}