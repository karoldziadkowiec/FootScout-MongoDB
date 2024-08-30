using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class RoleService : IRoleService
    {
        private readonly MongoDBContext _dbContext;
        private readonly INewIdGeneratorService _newIdGeneratorService;

        public RoleService(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            _dbContext = dbContext;
            _newIdGeneratorService = newIdGeneratorService;
        }

        public async Task CreateNewRole(string roleName)
        {
            var existingRole = await _dbContext.RolesCollection
                .Find(r => r.Name == roleName)
                .FirstOrDefaultAsync();

            if (existingRole != null)
                throw new ArgumentException($"Role {roleName} already exists.");

            var newRole = new Role
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = roleName
            };

            await _dbContext.RolesCollection.InsertOneAsync(newRole);
        }

        public async Task AddRoleToUser(string userId, string roleName)
        {
            var user = await _dbContext.UsersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            var role = await _dbContext.RolesCollection.Find(r => r.Name == roleName).FirstOrDefaultAsync();

            if (role == null)
                throw new ArgumentException($"Role {roleName} does not exist.");

            var userRole = new UserRole
            {
                Id = await _newIdGeneratorService.GenerateNewUserRoleId(),
                UserId = userId,
                User = user,
                RoleId = role.Id,
                Role = role,
            };

            await _dbContext.UserRolesCollection.InsertOneAsync(userRole);
        }

        public async Task RemoveRoleFromUser(string userId, string roleName)
        {
            var role = await _dbContext.RolesCollection
                .Find(r => r.Name == roleName)
                .FirstOrDefaultAsync();

            if (role == null)
                throw new ArgumentException($"Role {roleName} does not exist.");

            await _dbContext.UserRolesCollection
                .DeleteOneAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
        }

        public async Task<bool> CheckRoleExists(string roleName)
        {
            var count = await _dbContext.RolesCollection
                .CountDocumentsAsync(r => r.Name == roleName);

            return count > 0;
        }
    }
}