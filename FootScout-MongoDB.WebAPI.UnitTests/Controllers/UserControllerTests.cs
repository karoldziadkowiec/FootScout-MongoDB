using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userController = new UserController(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUser_UserExists_ReturnsOkResult()
        {
            // Arrange
            var userId = "leomessi";
            var userDto = new UserDTO { Id = "leomessi", Email = "lm10@gmail.com" };

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _userController.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDTO>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetUser_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "leomessi";

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _userController.GetUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResultWithUserList()
        {
            // Arrange
            var usersDto = new List<UserDTO>
            {
                new UserDTO { Id = "leomessi", Email = "lm10@gmail.com" },
                new UserDTO { Id = "pepguardiola", Email = "pg8@gmail.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetUsers()).ReturnsAsync(usersDto);

            // Act
            var result = await _userController.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<UserDTO>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task GetOnlyUsers_ReturnsOkResultWithOnlyUsers()
        {
            // Arrange
            var onlyUsersDto = new List<UserDTO>
            {
                new UserDTO { Id = "leomessi", Email = "lm10@gmail.com" },
                new UserDTO { Id = "pepguardiola", Email = "pg8@gmail.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetOnlyUsers()).ReturnsAsync(onlyUsersDto);

            // Act
            var result = await _userController.GetOnlyUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<UserDTO>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task GetOnlyAdmins_ReturnsOkResultWithOnlyAdmins()
        {
            // Arrange
            var onlyAdminsDto = new List<UserDTO>
            {
                new UserDTO { Id = "admin0", Email = "admin@admin.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetOnlyAdmins()).ReturnsAsync(onlyAdminsDto);

            // Act
            var result = await _userController.GetOnlyAdmins();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAdmins = Assert.IsType<List<UserDTO>>(okResult.Value);
            Assert.Equal(1, returnedAdmins.Count);
        }

        [Fact]
        public async Task GetUserRole_ReturnsOkResultWithRole()
        {
            // Arrange
            var userId = "leomessi";
            var role = "User";

            _mockUserRepository.Setup(repo => repo.GetUserRole(userId)).ReturnsAsync(role);

            // Act
            var result = await _userController.GetUserRole(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRole = Assert.IsType<string>(okResult.Value);
            Assert.Equal(role, returnedRole);
        }

        [Fact]
        public async Task GetUserCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;

            _mockUserRepository.Setup(repo => repo.GetUserCount()).ReturnsAsync(count);

            // Act
            var result = await _userController.GetUserCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task UpdateUser_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var userId = "leomessi";
            var userUpdateDto = new UserUpdateDTO { FirstName = "Lionel" };

            _mockUserRepository.Setup(repo => repo.UpdateUser(userId, userUpdateDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _userController.UpdateUser(userId, userUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserUpdateDTO();

            _mockUserRepository.Setup(repo => repo.UpdateUser(userId, dto)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _userController.UpdateUser(userId, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"User {userId} not found", (notFoundResult as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task UpdateUser_DbUpdateConcurrencyException_ThrowsException()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserUpdateDTO();

            _mockUserRepository.Setup(repo => repo.UpdateUser(userId, dto)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(new UserDTO());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _userController.UpdateUser(userId, dto));
        }

        [Fact]
        public async Task ResetUserPassword_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var userId = "leomessi";
            var resetPasswordDto = new UserResetPasswordDTO { PasswordHash = "Lionel123!" };

            _mockUserRepository.Setup(repo => repo.ResetUserPassword(userId, resetPasswordDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _userController.ResetUserPassword(userId, resetPasswordDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ResetUserPassword_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserResetPasswordDTO();

            _mockUserRepository.Setup(repo => repo.ResetUserPassword(userId, dto)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _userController.ResetUserPassword(userId, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"User {userId} not found", (notFoundResult as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task ResetUserPassword_DbUpdateConcurrencyException_ThrowsException()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserResetPasswordDTO();

            _mockUserRepository.Setup(repo => repo.ResetUserPassword(userId, dto)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(new UserDTO());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _userController.ResetUserPassword(userId, dto));
        }

        [Fact]
        public async Task DeleteUser_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var userId = "leomessi";
            var user = new UserDTO { Id = "leomessi" };

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteUser(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "leomessi";

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"User {userId} not found", (notFoundResult as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task DeleteUser_InvalidOperationException_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = "leomessi";

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(new UserDTO());
            _mockUserRepository.Setup(repo => repo.DeleteUser(userId)).ThrowsAsync(new InvalidOperationException("Operation not valid"));

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Operation not valid", (notFoundResult as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task DeleteUser_Exception_ReturnsStatusCode500()
        {
            // Arrange
            var userId = "leomessi";

            _mockUserRepository.Setup(repo => repo.GetUser(userId)).ReturnsAsync(new UserDTO());
            _mockUserRepository.Setup(repo => repo.DeleteUser(userId)).ThrowsAsync(new Exception("Internal error"));

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error: Internal error", objectResult.Value);
        }

        [Fact]
        public async Task GetUserClubHistory_ValidUser_ReturnsOkResultWithHistory()
        {
            // Arrange
            var userId = "leomessi";
            var clubHistory = new List<ClubHistory> { new ClubHistory { Id = 1, ClubName = "FC Barcelona", PlayerId = "leomessi" } };

            _mockUserRepository.Setup(repo => repo.GetUserClubHistory(userId)).ReturnsAsync(clubHistory);

            // Act
            var result = await _userController.GetUserClubHistory(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubHistory>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedHistory = Assert.IsType<List<ClubHistory>>(okResult.Value);
            Assert.Equal(clubHistory.Count, returnedHistory.Count);
        }

        [Fact]
        public async Task GetUserPlayerAdvertisements_ValidUser_ReturnsOkResultWithAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var playerAdvertisements = new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserPlayerAdvertisements(userId)).ReturnsAsync(playerAdvertisements);

            // Act
            var result = await _userController.GetUserPlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(playerAdvertisements.Count, returnedAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserActivePlayerAdvertisements_ValidUser_ReturnsOkResultWithActiveAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var activePlayerAdvertisements = new List<PlayerAdvertisement>
            {
                 new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserActivePlayerAdvertisements(userId)).ReturnsAsync(activePlayerAdvertisements);

            // Act
            var result = await _userController.GetUserActivePlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActiveAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(activePlayerAdvertisements.Count, returnedActiveAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserInactivePlayerAdvertisements_ValidUser_ReturnsOkResultWithInactiveAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var inactivePlayerAdvertisements = new List<PlayerAdvertisement>
            {
                 new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserInactivePlayerAdvertisements(userId)).ReturnsAsync(inactivePlayerAdvertisements);

            // Act
            var result = await _userController.GetUserInactivePlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactiveAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(inactivePlayerAdvertisements.Count, returnedInactiveAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserFavoritePlayerAdvertisements_ValidUser_ReturnsOkResultWithFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var favoritePlayerAdvertisements = new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserFavoritePlayerAdvertisements(userId)).ReturnsAsync(favoritePlayerAdvertisements);

            // Act
            var result = await _userController.GetUserFavoritePlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedFavoriteAdvertisements = Assert.IsType<List<FavoritePlayerAdvertisement>>(okResult.Value);
            Assert.Equal(favoritePlayerAdvertisements.Count, returnedFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserActiveFavoritePlayerAdvertisements_ValidUser_ReturnsOkResultWithActiveFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var activeFavoritePlayerAdvertisements = new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserActiveFavoritePlayerAdvertisements(userId)).ReturnsAsync(activeFavoritePlayerAdvertisements);

            // Act
            var result = await _userController.GetUserActiveFavoritePlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActiveFavoriteAdvertisements = Assert.IsType<List<FavoritePlayerAdvertisement>>(okResult.Value);
            Assert.Equal(activeFavoritePlayerAdvertisements.Count, returnedActiveFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserInactiveFavoritePlayerAdvertisements_ValidUser_ReturnsOkResultWithInactiveFavoriteAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var inactiveFavoritePlayerAdvertisements = new List<FavoritePlayerAdvertisement>
            {
                new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserInactiveFavoritePlayerAdvertisements(userId)).ReturnsAsync(inactiveFavoritePlayerAdvertisements);

            // Act
            var result = await _userController.GetUserInactiveFavoritePlayerAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoritePlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactiveFavoriteAdvertisements = Assert.IsType<List<FavoritePlayerAdvertisement>>(okResult.Value);
            Assert.Equal(inactiveFavoritePlayerAdvertisements.Count, returnedInactiveFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetReceivedClubOffers_ValidUser_ReturnsOkResultWithReceivedOffers()
        {
            // Arrange
            var userId = "leomessi";
            var receivedClubOffers = new List<ClubOffer>
            {
                new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetReceivedClubOffers(userId)).ReturnsAsync(receivedClubOffers);

            // Act
            var result = await _userController.GetReceivedClubOffers(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedOffers = Assert.IsType<List<ClubOffer>>(okResult.Value);
            Assert.Equal(receivedClubOffers.Count, returnedOffers.Count);
        }

        [Fact]
        public async Task GetSentClubOffers_ValidUser_ReturnsOkResultWithSentOffers()
        {
            // Arrange
            var userId = "pepguardiola";
            var sentClubOffers = new List<ClubOffer>
            {
                 new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetSentClubOffers(userId)).ReturnsAsync(sentClubOffers);

            // Act
            var result = await _userController.GetSentClubOffers(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedOffers = Assert.IsType<List<ClubOffer>>(okResult.Value);
            Assert.Equal(sentClubOffers.Count, returnedOffers.Count);
        }

        [Fact]
        public async Task GetUserClubAdvertisements_ValidUser_ReturnsOkResultWithClubAdvertisements()
        {
            // Arrange
            var userId = "pepguardiola";
            var clubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserClubAdvertisements(userId)).ReturnsAsync(clubAdvertisements);

            // Act
            var result = await _userController.GetUserClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(clubAdvertisements.Count, returnedAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserActiveClubAdvertisements_ValidUser_ReturnsOkResultWithActiveClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var activeClubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserActiveClubAdvertisements(userId)).ReturnsAsync(activeClubAdvertisements);

            // Act
            var result = await _userController.GetUserActiveClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActiveAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(activeClubAdvertisements.Count, returnedActiveAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserInactiveClubAdvertisements_ValidUser_ReturnsOkResultWithInactiveClubAdvertisements()
        {
            // Arrange
            var userId = "leomessi";
            var inactiveClubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserInactiveClubAdvertisements(userId)).ReturnsAsync(inactiveClubAdvertisements);

            // Act
            var result = await _userController.GetUserInactiveClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactiveAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(inactiveClubAdvertisements.Count, returnedInactiveAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserFavoriteClubAdvertisements_ValidUser_ReturnsOkResultWithFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "pepguardiola";
            var favoriteClubAdvertisements = new List<FavoriteClubAdvertisement>
            {
                new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserFavoriteClubAdvertisements(userId)).ReturnsAsync(favoriteClubAdvertisements);

            // Act
            var result = await _userController.GetUserFavoriteClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoriteClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedFavoriteAdvertisements = Assert.IsType<List<FavoriteClubAdvertisement>>(okResult.Value);
            Assert.Equal(favoriteClubAdvertisements.Count, returnedFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserActiveFavoriteClubAdvertisements_ValidUser_ReturnsOkResultWithActiveFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "pepguardiola";
            var activeFavoriteClubAdvertisements = new List<FavoriteClubAdvertisement>
            {
                new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserActiveFavoriteClubAdvertisements(userId)).ReturnsAsync(activeFavoriteClubAdvertisements);

            // Act
            var result = await _userController.GetUserActiveFavoriteClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoriteClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActiveFavoriteAdvertisements = Assert.IsType<List<FavoriteClubAdvertisement>>(okResult.Value);
            Assert.Equal(activeFavoriteClubAdvertisements.Count, returnedActiveFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetUserInactiveFavoriteClubAdvertisements_ValidUser_ReturnsOkResultWithInactiveFavoriteClubAdvertisements()
        {
            // Arrange
            var userId = "pepguardiola";
            var inactiveFavoriteClubAdvertisements = new List<FavoriteClubAdvertisement>
            {
                 new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserInactiveFavoriteClubAdvertisements(userId)).ReturnsAsync(inactiveFavoriteClubAdvertisements);

            // Act
            var result = await _userController.GetUserInactiveFavoriteClubAdvertisements(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FavoriteClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactiveFavoriteAdvertisements = Assert.IsType<List<FavoriteClubAdvertisement>>(okResult.Value);
            Assert.Equal(inactiveFavoriteClubAdvertisements.Count, returnedInactiveFavoriteAdvertisements.Count);
        }

        [Fact]
        public async Task GetReceivedPlayerOffers_ValidUser_ReturnsOkResultWithReceivedPlayerOffers()
        {
            // Arrange
            var userId = "leomessi";
            var receivedPlayerOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, PlayerFootId = 1 ,PlayerId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetReceivedPlayerOffers(userId)).ReturnsAsync(receivedPlayerOffers);

            // Act
            var result = await _userController.GetReceivedPlayerOffers(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedOffers = Assert.IsType<List<PlayerOffer>>(okResult.Value);
            Assert.Equal(receivedPlayerOffers.Count, returnedOffers.Count);
        }

        [Fact]
        public async Task GetSentPlayerOffers_ValidUser_ReturnsOkResultWithSentPlayerOffers()
        {
            // Arrange
            var userId = "leomessi";
            var sentPlayerOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, PlayerFootId = 1 ,PlayerId = "leomessi" }
            };

            _mockUserRepository.Setup(repo => repo.GetSentPlayerOffers(userId)).ReturnsAsync(sentPlayerOffers);

            // Act
            var result = await _userController.GetSentPlayerOffers(userId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedOffers = Assert.IsType<List<PlayerOffer>>(okResult.Value);
            Assert.Equal(sentPlayerOffers.Count, returnedOffers.Count);
        }

        [Fact]
        public async Task GetUserChats_ValidUser_ReturnsOkResultWithUserChats()
        {
            // Arrange
            var user1Id = "leomessi";
            var user2Id = "pepguardiola";
            var userChats = new List<Chat>
            {
                new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" }
            };

            _mockUserRepository.Setup(repo => repo.GetUserChats(user1Id)).ReturnsAsync(userChats);

            // Act
            var result = await _userController.GetUserChats(user1Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Chat>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedChats = Assert.IsType<List<Chat>>(okResult.Value);
            Assert.Equal(userChats.Count, returnedChats.Count);
        }

        [Fact]
        public async Task ExportUsersToCsv_ReturnsFileResult_WithCsvStream()
        {
            // Arrange
            var csvData = "E-mail,First Name,Last Name,Phone Number,Location,Creation Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockUserRepository.Setup(repo => repo.ExportUsersToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _userController.ExportUsersToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("users.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}