using FootScout_MongoDB.WebAPI.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}