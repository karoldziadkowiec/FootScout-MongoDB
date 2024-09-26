using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class AccountServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private NewIdGeneratorService _newIdGeneratorService;
        private AccountService _accountService;

        public AccountServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;

            var _mapper = TestBase.CreateMapper();
            _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            var _passwordService = Mock.Of<IPasswordService>();
            var _cookieService = Mock.Of<ICookieService>();

            IUserRepository _userRepository = new UserRepository(_dbContext, _newIdGeneratorService, _mapper, _passwordService);
            IRoleService _roleService = new RoleService(_dbContext, _newIdGeneratorService);
            ITokenService _tokenService = new TokenService(TestBase.CreateConfiguration(), _userRepository);

            _accountService = new AccountService(_dbContext, _mapper, _userRepository, _passwordService, _roleService, _tokenService, _cookieService);
        }

        [Fact]
        public async Task Register_SuccessfulRegistration_CreatesUser()
        {
            // Arrange
            var registerDTO = new RegisterDTO
            {
                Email = "new@user.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "First Name",
                LastName = "Last Name",
                Location = "Location",
                PhoneNumber = "PhoneNumber",
            };

            // Act
            await _accountService.Register(registerDTO);

            // Assert
            var result = await _dbContext.UsersCollection.Find(u => u.Email == registerDTO.Email).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(registerDTO.Email, result.Email);
        }

        [Fact]
        public async Task Login_SuccessfulLogin_ReturnsToken()
        {
            // Arrange
            var registerDTO = new RegisterDTO
            {
                Email = "cr7@gmail.com",
                Password = "Cr7771!",
                ConfirmPassword = "Cr7771!",
                FirstName = "Cristiano",
                LastName = "Ronaldo",
                Location = "Madrid",
                PhoneNumber = "707070707"
            };

            // Act
            await _accountService.Register(registerDTO);

            var loginDTO = new LoginDTO
            {
                Email = "cr7@gmail.com",
                Password = "Cr7771!"
            };

            // Act
            var result = await _accountService.Login(loginDTO);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetRoles_SuccessfulReturnsRoles()
        {
            // Arrange & Act
            var roles = await _accountService.GetRoles();

            // Assert
            var result = await _dbContext.RolesCollection.Find(u => u.Name == RoleName.User).FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(RoleName.User, result.Name);
        }

        [Fact]
        public async Task MakeAnAdmin_Successfully_MakesUserAnAdmin()
        {
            // Arrange
            var user = await _dbContext.UsersCollection.Find(u => u.Id == "leomessi").FirstOrDefaultAsync();

            // Act
            await _accountService.MakeAnAdmin(user.Id);

            // Assert
            var adminRoleId = await _dbContext.RolesCollection.Find(r => r.Name == RoleName.Admin).Project(r => r.Id).FirstOrDefaultAsync();

            var userInAdminRole = await _dbContext.UserRolesCollection.Find(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId).FirstOrDefaultAsync();

            Assert.NotNull(userInAdminRole);

            await _accountService.MakeAnUser(user.Id);
        }

        [Fact]
        public async Task MakeAnUser_Successfully_MakesAdminAnUser()
        {
            // Arrange
            await _dbContext.UsersCollection.InsertOneAsync(new User { Id = "newadmin", Email = "newadmin@newadmin.com", PasswordHash = "Admin1!", FirstName = "Admin F2.", LastName = "Admin L2.", Location = "Admin Loc2.", PhoneNumber = "000000002", CreationDate = DateTime.Now });
            var adminRole = await _dbContext.RolesCollection.Find(r => r.Name == RoleName.Admin).FirstOrDefaultAsync();
            await _dbContext.UserRolesCollection.InsertOneAsync(new UserRole { Id = await _newIdGeneratorService.GenerateNewUserRoleId(), UserId = "newadmin", User = await _dbContext.UsersCollection.Find(u => u.Id == "newadmin").FirstOrDefaultAsync(), RoleId = adminRole.Id, Role = adminRole });

            var user = await _dbContext.UsersCollection.Find(u => u.Id == "admin0").FirstOrDefaultAsync();

            // Act
            await _accountService.MakeAnUser(user.Id);

            // Assert
            var userRoleId = await _dbContext.RolesCollection.Find(r => r.Name == RoleName.User).Project(r => r.Id).FirstOrDefaultAsync();

            var adminInAdminRole = await _dbContext.UserRolesCollection.Find(ur => ur.UserId == user.Id && ur.RoleId == userRoleId).FirstOrDefaultAsync();

            Assert.NotNull(adminInAdminRole);

            await _accountService.MakeAnAdmin(user.Id);
        }
    }
}