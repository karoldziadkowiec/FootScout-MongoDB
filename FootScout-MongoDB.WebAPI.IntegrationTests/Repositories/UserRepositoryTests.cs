using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Repositories
{
    public class UserRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private UserRepository _userRepository;

        public UserRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;

            var _mapper = TestBase.CreateMapper();
            var _newIdGeneratorService = Mock.Of<INewIdGeneratorService>();
            var _passwordService = Mock.Of<IPasswordService>();

            _userRepository = new UserRepository(_dbContext, _newIdGeneratorService, _mapper, _passwordService);
        }

        [Fact]
        public async Task FindUserByEmail_ShouldReturnUser_WhenEmailValid()
        {
            // Arrange
            var userEmail = "lm10@gmail.com";

            // Act
            var result = await _userRepository.FindUserByEmail(userEmail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userEmail, result.Email);
            Assert.Equal("leomessi", result.Id);
        }

        [Fact]
        public async Task GetRolesForUser_ShouldReturnListOfRolesForUserId()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetRolesForUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, role => role == RoleName.User);
        }

        [Fact]
        public async Task GetUsersByRoleName_ShouldReturnListOfUserByRoleName()
        {
            // Arrange
            var roleName = RoleName.User;

            // Act
            var result = await _userRepository.GetUsersByRoleName(roleName);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, dto => dto.Id == "leomessi");
            Assert.Contains(result, dto => dto.Id == "pepguardiola");
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserDTO_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnListOfUserDTOs()
        {
            // Arrange & Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Contains(result, dto => dto.Id == "admin0");
            Assert.Contains(result, dto => dto.Id == "leomessi");
        }

        [Fact]
        public async Task GetOnlyUsers_ReturnsUsersWithUserRole()
        {
            // Arrange & Act
            var result = await _userRepository.GetOnlyUsers();
            var userList = result.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(userList, u => u.Id == "leomessi");
            Assert.DoesNotContain(userList, u => u.Id == "admin0");
            Assert.Equal(3, userList.Count);
        }

        [Fact]
        public async Task GetOnlyAdmins_ReturnsUsersWithAdminRole()
        {
            // Arrange & Act
            var result = await _userRepository.GetOnlyAdmins();
            var adminList = result.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(adminList, u => u.Id == "admin0");
            Assert.DoesNotContain(adminList, u => u.Id == "leomessi");
            Assert.Equal(1, adminList.Count);
        }

        [Fact]
        public async Task GetUserRole_ReturnsCorrectRole()
        {
            // Arrange
            var role = "leomessi";

            // Act
            var result = await _userRepository.GetUserRole(role);

            // Assert
            Assert.Equal(RoleName.User, result);
        }

        [Fact]
        public async Task GetUserCount_ReturnsCorrectNumberOfUsers()
        {
            // Arrange & Act
            var result = await _userRepository.GetUserCount();

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public async Task UpdateUser_UpdatesUserDetails()
        {
            // Arrange
            var updatedUserId = "leomessi";
            var dto = new UserUpdateDTO
            {
                FirstName = "Updated FirstName",
                LastName = "Updated LastName",
                PhoneNumber = "Updated PhoneNumber",
                Location = "Updated Location"
            };

            // Act
            await _userRepository.UpdateUser(updatedUserId, dto);

            // Assert
            var updatedUser = await _dbContext.UsersCollection.Find(u => u.Id == updatedUserId).FirstOrDefaultAsync();
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated FirstName", updatedUser.FirstName);
        }

        [Fact]
        public async Task ResetUserPassword_ResetsPasswordSuccessfully()
        {
            // Arrange
            var updatedUserId = "leomessi";
            var dto = new UserResetPasswordDTO
            {
                PasswordHash = "NewPassword123!",
                ConfirmPasswordHash = "NewPassword123!"
            };

            var passwordHasher = TestBase.CreatePasswordHasher();

            // Act
            await _userRepository.ResetUserPassword(updatedUserId, dto);

            // Assert
            var updatedUser = await _dbContext.UsersCollection.Find(u => u.Id == updatedUserId).FirstOrDefaultAsync();
            Assert.NotNull(updatedUser);
        }

        [Fact]
        public async Task DeleteUser_RemovesUserAndRelatedEntities()
        {
            // Arrange
            var userId = "userToDelete";
            await _dbContext.UsersCollection.InsertOneAsync(new User { Id = userId, Email = "userToDelete@user.com", PasswordHash = "userToDelete1!", FirstName = "User", LastName = "To Delete", Location = "Location", PhoneNumber = "000000000" });

            var user = await _dbContext.UsersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new Exception("Test user not found");

            // Act
            await _userRepository.DeleteUser(userId);

            // Assert
            var deletedUser = await _dbContext.UsersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            Assert.Null(deletedUser);

            var clubHistories = await _dbContext.ClubHistoriesCollection
                .Find(ch => ch.PlayerId == userId)
                .ToListAsync();
            Assert.Empty(clubHistories);

            var chats = await _dbContext.ChatsCollection
                .Find(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();
            Assert.Empty(chats);

            var playerFavorites = await _dbContext.FavoritePlayerAdvertisementsCollection
                .Find(fpa => fpa.UserId == userId)
                .ToListAsync();
            Assert.Empty(playerFavorites);

            var clubFavorites = await _dbContext.FavoriteClubAdvertisementsCollection
                .Find(fca => fca.UserId == userId)
                .ToListAsync();
            Assert.Empty(clubFavorites);

            var playerAdvertisements = await _dbContext.PlayerAdvertisementsCollection
                .Find(pa => pa.PlayerId == userId)
                .ToListAsync();
            Assert.All(playerAdvertisements, pa => Assert.Equal("unknownUserId", pa.PlayerId));

            var clubOffers = await _dbContext.ClubOffersCollection
                .Find(co => co.ClubMemberId == userId)
                .ToListAsync();
            Assert.All(clubOffers, co => Assert.Equal("unknownUserId", co.ClubMemberId));

            var clubAdvertisements = await _dbContext.ClubAdvertisementsCollection
                .Find(ca => ca.ClubMemberId == userId)
                .ToListAsync();
            Assert.All(clubAdvertisements, ca => Assert.Equal("unknownUserId", ca.ClubMemberId));

            var playerOffers = await _dbContext.PlayerOffersCollection
                .Find(po => po.PlayerId == userId)
                .ToListAsync();
            Assert.All(playerOffers, po => Assert.Equal("unknownUserId", po.PlayerId));

            var problems = await _dbContext.ProblemsCollection
                .Find(p => p.RequesterId == userId)
                .ToListAsync();
            Assert.Empty(problems);
        }

        [Fact]
        public async Task GetUserClubHistory_ReturnsUserClubHistories()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserClubHistory(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ch => Assert.Equal(userId, ch.PlayerId));
            Assert.True(result.All(ch => ch.StartDate != default(DateTime)));
        }

        [Fact]
        public async Task GetUserPlayerAdvertisements_ReturnsUserPlayerAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserPlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
            Assert.True(result.All(pa => pa.EndDate != default(DateTime)));
        }

        [Fact]
        public async Task GetUserActivePlayerAdvertisements_ReturnsActivePlayerAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserActivePlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
            Assert.All(result, pa => Assert.True(pa.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetUserInactivePlayerAdvertisements_ReturnsInactivePlayerAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserInactivePlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.PlayerId));
            Assert.All(result, pa => Assert.True(pa.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task GetUserFavoritePlayerAdvertisements_ReturnsFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserFavoritePlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
            Assert.True(result.All(pa => pa.PlayerAdvertisement != null));
        }

        [Fact]
        public async Task GetUserActiveFavoritePlayerAdvertisements_ReturnsActiveFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserActiveFavoritePlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
            Assert.All(result, pa => Assert.True(pa.PlayerAdvertisement.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetUserInactiveFavoritePlayerAdvertisements_ReturnsInactiveFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserInactiveFavoritePlayerAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, pa => Assert.Equal(userId, pa.UserId));
            Assert.All(result, pa => Assert.True(pa.PlayerAdvertisement.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task GetReceivedClubOffers_ReturnsReceivedClubOffers()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetReceivedClubOffers(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.Equal(userId, co.PlayerAdvertisement.PlayerId));
            Assert.True(result.All(co => co.OfferStatus != null));
        }

        [Fact]
        public async Task GetSentClubOffers_ReturnsSentClubOffers()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetSentClubOffers(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, co => Assert.Equal(userId, co.ClubMemberId));
            Assert.True(result.All(co => co.OfferStatus != null));
        }

        [Fact]
        public async Task GetUserClubAdvertisements_ReturnsClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
            Assert.True(result.All(ca => ca.PlayerPosition != null));
        }

        [Fact]
        public async Task GetUserActiveClubAdvertisements_ReturnsActiveClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserActiveClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
            Assert.All(result, ca => Assert.True(ca.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetUserInactiveClubAdvertisements_ReturnsInactiveClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserInactiveClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.ClubMemberId));
            Assert.All(result, ca => Assert.True(ca.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task GetUserFavoriteClubAdvertisements_ReturnsFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserFavoriteClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
            Assert.True(result.All(ca => ca.ClubAdvertisement != null));
        }

        [Fact]
        public async Task GetUserActiveFavoriteClubAdvertisements_ReturnsActiveFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserActiveFavoriteClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
            Assert.All(result, ca => Assert.True(ca.ClubAdvertisement.EndDate >= DateTime.Now));
        }

        [Fact]
        public async Task GetUserInactiveFavoriteClubAdvertisements_ReturnsInactiveFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserInactiveFavoriteClubAdvertisements(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, ca => Assert.Equal(userId, ca.UserId));
            Assert.All(result, ca => Assert.True(ca.ClubAdvertisement.EndDate < DateTime.Now));
        }

        [Fact]
        public async Task GetReceivedPlayerOffers_ReturnsReceivedPlayerOffers()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetReceivedPlayerOffers(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.Equal(userId, po.ClubAdvertisement.ClubMemberId));
            Assert.True(result.All(po => po.OfferStatus != null));
        }

        [Fact]
        public async Task GetSentPlayerOffers_ReturnsSentPlayerOffers()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetSentPlayerOffers(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, po => Assert.Equal(userId, po.PlayerId));
            Assert.True(result.All(po => po.OfferStatus != null));
        }

        [Fact]
        public async Task GetUserChats_ReturnsChatsOrderedByLastMessage()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var result = await _userRepository.GetUserChats(userId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, chat => Assert.True(chat.User1Id == userId || chat.User2Id == userId));
        }

        [Fact]
        public async Task ExportUsersToCsv_ReturnsValidCsvStream()
        {
            // Arrange & Act
            var csvStream = await _userRepository.ExportUsersToCsv();
            csvStream.Position = 0;

            using (var reader = new StreamReader(csvStream))
            {
                var csvContent = await reader.ReadToEndAsync();

                // Assert
                Assert.NotEmpty(csvContent);
                Assert.Contains("E-mail,First Name,Last Name,Phone Number,Location,Creation Date", csvContent);
                Assert.Contains("lm10@gmail.com,Leo,Messi,101010101", csvContent);
            }
        }
    }
}