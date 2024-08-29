using FootScout_MongoDB.WebAPI.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_MongoDB.WebAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateTokenJWT(User user);
    }
}