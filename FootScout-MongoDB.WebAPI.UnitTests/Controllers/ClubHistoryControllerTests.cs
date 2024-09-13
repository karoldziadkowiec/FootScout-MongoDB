using AutoMapper;
using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class ClubHistoryControllerTests
    {
        private readonly Mock<IClubHistoryRepository> _mockClubHistoryRepository;
        private readonly Mock<IAchievementsRepository> _mockAchievementsRepository;
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ClubHistoryController _clubHistoryController;

        public ClubHistoryControllerTests()
        {
            _mockClubHistoryRepository = new Mock<IClubHistoryRepository>();
            _mockAchievementsRepository = new Mock<IAchievementsRepository>();
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _clubHistoryController = new ClubHistoryController(_mockClubHistoryRepository.Object,_mockAchievementsRepository.Object, _mockPlayerPositionRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetClubHistory_ExistingClubHistory_ReturnsOkResultWithClubHistory()
        {
            // Arrange
            var clubHistoryId = 1;
            var clubHistory = new ClubHistory { Id = 1, ClubName = "FC Barcelona", League = "La Liga" };
            _mockClubHistoryRepository.Setup(repo => repo.GetClubHistory(clubHistoryId))
                .ReturnsAsync(clubHistory);

            // Act
            var result = await _clubHistoryController.GetClubHistory(clubHistoryId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ClubHistory>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedClubHistory = Assert.IsType<ClubHistory>(okResult.Value);
            Assert.Equal(clubHistoryId, returnedClubHistory.Id);
        }

        [Fact]
        public async Task GetClubHistory_NonExistingClubHistory_ReturnsNotFound()
        {
            // Arrange
            var clubHistoryId = 1;
            _mockClubHistoryRepository.Setup(repo => repo.GetClubHistory(clubHistoryId))
                .ReturnsAsync((ClubHistory)null);

            // Act
            var result = await _clubHistoryController.GetClubHistory(clubHistoryId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ClubHistory>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAllClubHistory_ReturnsOkResultWithClubHistoriesList()
        {
            // Arrange
            var clubHistories = new List<ClubHistory>
        {
            new ClubHistory { Id = 1, ClubName = "FC Barcelona", League = "La Liga" },
            new ClubHistory { Id = 2, ClubName = "PSG", League = "Ligue 1" }
        };
            _mockClubHistoryRepository.Setup(repo => repo.GetAllClubHistory())
                .ReturnsAsync(clubHistories);

            // Act
            var result = await _clubHistoryController.GetAllClubHistory();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubHistory>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedClubHistories = Assert.IsType<List<ClubHistory>>(okResult.Value);
            Assert.Equal(clubHistories.Count, returnedClubHistories.Count);
        }

        [Fact]
        public async Task GetClubHistoryCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockClubHistoryRepository.Setup(repo => repo.GetClubHistoryCount())
                .ReturnsAsync(count);

            // Act
            var result = await _clubHistoryController.GetClubHistoryCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task CreateClubHistory_ValidDto_ReturnsOkResultWithCreatedClubHistory()
        {
            // Arrange
            var dto = new ClubHistoryCreateDTO { ClubName = "FC Barcelona", League = "La Liga", Achievements = new Achievements() };
            var achievements = new Achievements { Id = 1 };
            var clubHistory = new ClubHistory { Id = 1, ClubName = "FC Barcelona", League = "La Liga", AchievementsId = achievements.Id };

            _mockMapper.Setup(m => m.Map<Achievements>(dto.Achievements)).Returns(achievements);
            _mockMapper.Setup(m => m.Map<ClubHistory>(dto)).Returns(clubHistory);
            _mockAchievementsRepository.Setup(repo => repo.CreateAchievements(achievements)).Returns(Task.CompletedTask);
            _mockClubHistoryRepository.Setup(repo => repo.CreateClubHistory(clubHistory)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubHistoryController.CreateClubHistory(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedClubHistory = Assert.IsType<ClubHistory>(okResult.Value);
            Assert.Equal(clubHistory.Id, returnedClubHistory.Id);
        }

        [Fact]
        public async Task CreateClubHistory_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            ClubHistoryCreateDTO dto = null;

            // Act
            var result = await _clubHistoryController.CreateClubHistory(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateClubHistory_ValidIdAndModel_ReturnsNoContent()
        {
            // Arrange
            var clubHistoryId = 1;
            var clubHistory = new ClubHistory { Id = 1, ClubName = "PSG", League = "Ligue 1" };
            _mockClubHistoryRepository.Setup(repo => repo.UpdateClubHistory(clubHistory)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubHistoryController.UpdateClubHistory(clubHistoryId, clubHistory);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateClubHistory_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var clubHistoryId = 1;
            var clubHistory = new ClubHistory { Id = 2, ClubName = "PSG", League = "Ligue 1" };

            // Act
            var result = await _clubHistoryController.UpdateClubHistory(clubHistoryId, clubHistory);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteClubHistory_ExistingClubHistory_ReturnsNoContent()
        {
            // Arrange
            var clubHistoryId = 1;
            var clubHistory = new ClubHistory { Id = 1, ClubName = "FC Barcelona", League = "La Liga" };
            _mockClubHistoryRepository.Setup(repo => repo.GetClubHistory(clubHistoryId)).ReturnsAsync(clubHistory);
            _mockClubHistoryRepository.Setup(repo => repo.DeleteClubHistory(clubHistoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubHistoryController.DeleteClubHistory(clubHistoryId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteClubHistory_NonExistingClubHistory_ReturnsNotFound()
        {
            // Arrange
            var clubHistoryId = 1;
            _mockClubHistoryRepository.Setup(repo => repo.GetClubHistory(clubHistoryId)).ReturnsAsync((ClubHistory)null);

            // Act
            var result = await _clubHistoryController.DeleteClubHistory(clubHistoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}