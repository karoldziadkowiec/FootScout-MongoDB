using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class TokenServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private TokenService _tokenService;

        public TokenServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            var _mapper = TestBase.CreateMapper();
            INewIdGeneratorService _newIdGeneratorService = new NewIdGeneratorService(_dbContext);
            var _passwordService = Mock.Of<IPasswordService>();
            var _cookieService = Mock.Of<ICookieService>();

            IUserRepository _userRepository = new UserRepository(_dbContext, _newIdGeneratorService, _mapper, _passwordService);
            _tokenService = new TokenService(TestBase.CreateConfiguration(), _userRepository);
        }

        [Fact]
        public async Task CreateTokenJWT_ShouldGenerateValidJwtToken_ForValidUser()
        {
            // Arrange
            var userId = "leomessi";
            var user = await _dbContext.UsersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

            // Act
            var token = await _tokenService.CreateTokenJWT(user);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(TestBase.CreateConfiguration()["JWT:ValidIssuer"], token.Issuer);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            var claims = tokenHandler.ReadJwtToken(tokenString).Claims;

            Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId);
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        }
    }
}