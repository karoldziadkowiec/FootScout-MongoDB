using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class RoleServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private RoleService _roleService;

        public RoleServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            var _newIdGeneratorService = Mock.Of<INewIdGeneratorService>();
            _roleService = new RoleService(_dbContext, _newIdGeneratorService);
        }

        [Fact]
        public async Task CreateNewRole_ShouldCreateNewRole_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleName = "newRole";

            // Act
            await _roleService.CreateNewRole(roleName);

            // Assert
            var role = await _dbContext.RolesCollection.Find(r => r.Name == roleName).FirstOrDefaultAsync();
            Assert.NotNull(role);
            Assert.Equal(roleName, role.Name);
        }

        [Fact]
        public async Task AddRoleToUser_ShouldAssignRoleToUser_WhenRoleExists()
        {
            // Arrange
            var roleName = RoleName.Admin;
            var userId = "unknown9";

            // Act
            await _roleService.AddRoleToUser(userId, roleName);

            // Assert
            var userRole = await _dbContext.UserRolesCollection
                .Find(ur => ur.UserId == userId && ur.Role.Name == roleName)
                .FirstOrDefaultAsync();

            Assert.NotNull(userRole);
            Assert.Equal(userId, userRole.UserId);
            Assert.Equal(roleName, userRole.Role.Name);
        }

        [Fact]
        public async Task AddRoleToUser_ShouldThrowException_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleName = "NonExistentRole";
            var userId = "unknown9";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _roleService.AddRoleToUser(userId, roleName));
            Assert.Equal($"Role {roleName} does not exist.", exception.Message);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ShouldRemoveRole_WhenRoleExists()
        {
            // Arrange
            var roleName = RoleName.User;
            var userId = "unknown9";

            // Act
            await _roleService.RemoveRoleFromUser(userId, roleName);

            // Assert
            var userRole = await _dbContext.UserRolesCollection
                .Find(ur => ur.UserId == userId && ur.Role.Name == roleName)
                .FirstOrDefaultAsync();

            Assert.Null(userRole);
        }

        [Fact]
        public async Task RemoveRoleFromUser_ShouldThrowException_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleName = "NewRole";
            var userId = "unknown9";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _roleService.RemoveRoleFromUser(userId, roleName));
            Assert.Equal($"Role {roleName} does not exist.", exception.Message);
        }

        [Fact]
        public async Task CheckRoleExists_ShouldReturnTrue_WhenRoleExists()
        {
            // Arrange
            var roleName = RoleName.User;

            // Act
            var exists = await _roleService.CheckRoleExists(roleName);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task CheckRoleExists_ShouldReturnFalse_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleName = "NewRole";

            // Act
            var exists = await _roleService.CheckRoleExists(roleName);

            // Assert
            Assert.False(exists);
        }
    }
}