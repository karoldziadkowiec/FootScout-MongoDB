using AutoMapper;
using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class PlayerAdvertisementControllerTests
    {
        private readonly Mock<IPlayerAdvertisementRepository> _mockPlayerAdvertisementRepository;
        private readonly Mock<ISalaryRangeRepository> _mockSalaryRangeRepository;
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<IPlayerFootRepository> _mockPlayerFootRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PlayerAdvertisementController _playerAdvertisementController;

        public PlayerAdvertisementControllerTests()
        {
            _mockPlayerAdvertisementRepository = new Mock<IPlayerAdvertisementRepository>();
            _mockSalaryRangeRepository = new Mock<ISalaryRangeRepository>();
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockPlayerFootRepository = new Mock<IPlayerFootRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _playerAdvertisementController = new PlayerAdvertisementController(_mockPlayerAdvertisementRepository.Object, _mockSalaryRangeRepository.Object, _mockPlayerPositionRepository.Object, _mockPlayerFootRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);}

        [Fact]
        public async Task GetPlayerAdvertisement_ExistingId_ReturnsOkResultWithPlayerAdvertisement()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var playerAdvertisement = new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetPlayerAdvertisement(playerAdvertisementId)).ReturnsAsync(playerAdvertisement);

            // Act
            var result = await _playerAdvertisementController.GetPlayerAdvertisement(playerAdvertisementId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PlayerAdvertisement>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPlayerAdvertisement = Assert.IsType<PlayerAdvertisement>(okResult.Value);
            Assert.Equal(playerAdvertisementId, returnedPlayerAdvertisement.Id);
        }

        [Fact]
        public async Task GetPlayerAdvertisement_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var playerAdvertisementId = 1;
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetPlayerAdvertisement(playerAdvertisementId)).ReturnsAsync((PlayerAdvertisement)null);

            // Act
            var result = await _playerAdvertisementController.GetPlayerAdvertisement(playerAdvertisementId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PlayerAdvertisement>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAllPlayerAdvertisements_ReturnsOkResultWithPlayerAdvertisements()
        {
            // Arrange
            var playerAdvertisements = new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" },
                new PlayerAdvertisement { Id = 2, League = "La Liga", PlayerId = "leomessi" }
            };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetAllPlayerAdvertisements()).ReturnsAsync(playerAdvertisements);

            // Act
            var result = await _playerAdvertisementController.GetAllPlayerAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedPlayerAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(playerAdvertisements.Count, returnedPlayerAdvertisements.Count);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisements_ReturnsOkResultWithActivePlayerAdvertisements()
        {
            // Arrange
            var activePlayerAdvertisements = new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" },
                new PlayerAdvertisement { Id = 2, League = "La Liga", PlayerId = "leomessi" }
            };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetActivePlayerAdvertisements()).ReturnsAsync(activePlayerAdvertisements);

            // Act
            var result = await _playerAdvertisementController.GetActivePlayerAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActivePlayerAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(activePlayerAdvertisements.Count, returnedActivePlayerAdvertisements.Count);
        }

        [Fact]
        public async Task GetActivePlayerAdvertisementCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetActivePlayerAdvertisementCount()).ReturnsAsync(count);

            // Act
            var result = await _playerAdvertisementController.GetActivePlayerAdvertisementCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetInactivePlayerAdvertisements_ReturnsOkResultWithInactivePlayerAdvertisements()
        {
            // Arrange
            var inactivePlayerAdvertisements = new List<PlayerAdvertisement>
            {
                new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" },
                new PlayerAdvertisement { Id = 2, League = "La Liga", PlayerId = "leomessi" }
            };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetInactivePlayerAdvertisements()).ReturnsAsync(inactivePlayerAdvertisements);

            // Act
            var result = await _playerAdvertisementController.GetInactivePlayerAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactivePlayerAdvertisements = Assert.IsType<List<PlayerAdvertisement>>(okResult.Value);
            Assert.Equal(inactivePlayerAdvertisements.Count, returnedInactivePlayerAdvertisements.Count);
        }

        [Fact]
        public async Task CreatePlayerAdvertisement_ValidDto_ReturnsOkResultWithPlayerAdvertisement()
        {
            // Arrange
            var dto = new PlayerAdvertisementCreateDTO
            {
                SalaryRangeDTO = new SalaryRangeDTO { Min = 1000, Max = 2000 }
            };
            var salaryRange = new SalaryRange { Id = 1 };
            var playerAdvertisement = new PlayerAdvertisement { Id = 1 };

            _mockMapper.Setup(m => m.Map<SalaryRange>(dto.SalaryRangeDTO)).Returns(salaryRange);
            _mockMapper.Setup(m => m.Map<PlayerAdvertisement>(dto)).Returns(playerAdvertisement);
            _mockSalaryRangeRepository.Setup(repo => repo.CreateSalaryRange(salaryRange)).Returns(Task.CompletedTask);
            _mockPlayerAdvertisementRepository.Setup(repo => repo.CreatePlayerAdvertisement(playerAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerAdvertisementController.CreatePlayerAdvertisement(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPlayerAdvertisement = Assert.IsType<PlayerAdvertisement>(okResult.Value);
            Assert.Equal(playerAdvertisement.Id, returnedPlayerAdvertisement.Id);
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_ValidIdAndDto_ReturnsNoContent()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var playerAdvertisement = new PlayerAdvertisement { Id = 1, League = "Serie A", PlayerId = "leomessi" };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.UpdatePlayerAdvertisement(playerAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerAdvertisementController.UpdatePlayerAdvertisement(playerAdvertisementId, playerAdvertisement);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlayerAdvertisement_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var playerAdvertisement = new PlayerAdvertisement { Id = 2, League = "La Liga", PlayerId = "leomessi" };
            _playerAdvertisementController.ModelState.AddModelError("Id", "ID mismatch");

            // Act
            var result = await _playerAdvertisementController.UpdatePlayerAdvertisement(playerAdvertisementId, playerAdvertisement);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeletePlayerAdvertisement_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var playerAdvertisement = new PlayerAdvertisement { Id = 1, League = "Premier League", PlayerId = "leomessi" };
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetPlayerAdvertisement(playerAdvertisementId)).ReturnsAsync(playerAdvertisement);
            _mockPlayerAdvertisementRepository.Setup(repo => repo.DeletePlayerAdvertisement(playerAdvertisementId)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerAdvertisementController.DeletePlayerAdvertisement(playerAdvertisementId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlayerAdvertisement_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var playerAdvertisementId = 1;
            _mockPlayerAdvertisementRepository.Setup(repo => repo.GetPlayerAdvertisement(playerAdvertisementId)).ReturnsAsync((PlayerAdvertisement)null);

            // Act
            var result = await _playerAdvertisementController.DeletePlayerAdvertisement(playerAdvertisementId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ExportPlayerAdvertisementsToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "E-mail,First Name,Last Name,Position,League,Region,Age,Height,Foot,Min Salary,Max Salary,Creation Date,End Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockPlayerAdvertisementRepository.Setup(repo => repo.ExportPlayerAdvertisementsToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _playerAdvertisementController.ExportPlayerAdvertisementsToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("player-advertisements.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}