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
    public class PlayerOfferControllerTests
    {
        private readonly Mock<IPlayerOfferRepository> _mockPlayerOfferRepository;
        private readonly Mock<IClubAdvertisementRepository> _mockClubAdvertisementRepository;
        private readonly Mock<IOfferStatusRepository> _mockOfferStatusRepository;
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<IPlayerFootRepository> _mockPlayerFootRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PlayerOfferController _playerOfferController;

        public PlayerOfferControllerTests()
        {
            _mockPlayerOfferRepository = new Mock<IPlayerOfferRepository>();
            _mockClubAdvertisementRepository = new Mock<IClubAdvertisementRepository>();
            _mockOfferStatusRepository = new Mock<IOfferStatusRepository>();
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockPlayerFootRepository = new Mock<IPlayerFootRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _playerOfferController = new PlayerOfferController(_mockPlayerOfferRepository.Object, _mockClubAdvertisementRepository.Object, _mockOfferStatusRepository.Object, _mockPlayerPositionRepository.Object, _mockPlayerFootRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetPlayerOffer_ExistingPlayerOffer_ReturnsOkResultWithPlayerOffer()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" };
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOffer(playerOfferId)).ReturnsAsync(playerOffer);

            // Act
            var result = await _playerOfferController.GetPlayerOffer(playerOfferId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PlayerOffer>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<PlayerOffer>(okResult.Value);
            Assert.Equal(playerOfferId, returnValue.Id);
        }

        [Fact]
        public async Task GetPlayerOffer_NonExistingPlayerOffer_ReturnsNotFound()
        {
            // Arrange
            var playerOfferId = 1;
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOffer(playerOfferId)).ReturnsAsync((PlayerOffer)null);

            // Act
            var result = await _playerOfferController.GetPlayerOffer(playerOfferId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPlayerOffers_ReturnsOkResultWithListOfPlayerOffers()
        {
            // Arrange
            var clubOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" },
                new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" }
            };
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOffers()).ReturnsAsync(clubOffers);

            // Act
            var result = await _playerOfferController.GetPlayerOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PlayerOffer>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetActivePlayerOffers_ReturnsOkResultWithActivePlayerOffers()
        {
            // Arrange
            var activePlayerOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" },
                new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" }
            };
            _mockPlayerOfferRepository.Setup(repo => repo.GetActivePlayerOffers()).ReturnsAsync(activePlayerOffers);

            // Act
            var result = await _playerOfferController.GetActivePlayerOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PlayerOffer>>(okResult.Value);
            Assert.Equal(activePlayerOffers.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetActivePlayerOfferCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockPlayerOfferRepository.Setup(repo => repo.GetActivePlayerOfferCount()).ReturnsAsync(count);

            // Act
            var result = await _playerOfferController.GetActivePlayerOfferCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnValue);
        }

        [Fact]
        public async Task GetInactivePlayerOffers_ReturnsOkResultWithInactivePlayerOffers()
        {
            // Arrange
            var inactivePlayerOffers = new List<PlayerOffer>
            {
                new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" },
                new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" }
            };
            _mockPlayerOfferRepository.Setup(repo => repo.GetInactivePlayerOffers()).ReturnsAsync(inactivePlayerOffers);

            // Act
            var result = await _playerOfferController.GetInactivePlayerOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PlayerOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<PlayerOffer>>(okResult.Value);
            Assert.Equal(inactivePlayerOffers.Count, returnValue.Count);
        }

        [Fact]
        public async Task CreatePlayerOffer_ValidDto_ReturnsOkResultWithPlayerOffer()
        {
            // Arrange
            var dto = new PlayerOfferCreateDTO();
            var playerOffer = new PlayerOffer { Id = 1 };
            _mockMapper.Setup(m => m.Map<PlayerOffer>(dto)).Returns(playerOffer);
            _mockPlayerOfferRepository.Setup(repo => repo.CreatePlayerOffer(playerOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerOfferController.CreatePlayerOffer(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PlayerOffer>(okResult.Value);
            Assert.Equal(playerOffer.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdatePlayerOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = playerOfferId };
            _mockPlayerOfferRepository.Setup(repo => repo.UpdatePlayerOffer(playerOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerOfferController.UpdatePlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlayerOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" };

            // Act
            var result = await _playerOfferController.UpdatePlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeletePlayerOffer_ExistingPlayerOffer_ReturnsNoContent()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = playerOfferId };
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOffer(playerOfferId)).ReturnsAsync(playerOffer);
            _mockPlayerOfferRepository.Setup(repo => repo.DeletePlayerOffer(playerOfferId)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerOfferController.DeletePlayerOffer(playerOfferId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlayerOffer_NonExistingPlayerOffer_ReturnsNotFound()
        {
            // Arrange
            var playerOfferId = 1;
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOffer(playerOfferId)).ReturnsAsync((PlayerOffer)null);

            // Act
            var result = await _playerOfferController.DeletePlayerOffer(playerOfferId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Player offer : {playerOfferId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task AcceptPlayerOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" };
            _mockPlayerOfferRepository.Setup(repo => repo.AcceptPlayerOffer(playerOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerOfferController.AcceptPlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AcceptPlayerOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" };

            // Act
            var result = await _playerOfferController.AcceptPlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RejectPlayerOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 1, PlayerFootId = 1, PlayerId = "leomessi" };
            _mockPlayerOfferRepository.Setup(repo => repo.RejectPlayerOffer(playerOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _playerOfferController.RejectPlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RejectPlayerOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var playerOfferId = 1;
            var playerOffer = new PlayerOffer { Id = 2, PlayerFootId = 3, PlayerId = "cr7" };

            // Act
            var result = await _playerOfferController.RejectPlayerOffer(playerOfferId, playerOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetPlayerOfferStatusId_ReturnsOkResultWithStatusId()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var userId = "leomessi";
            var statusId = 1;
            _mockPlayerOfferRepository.Setup(repo => repo.GetPlayerOfferStatusId(clubAdvertisementId, userId)).ReturnsAsync(statusId);

            // Act
            var result = await _playerOfferController.GetPlayerOfferStatusId(clubAdvertisementId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(statusId, returnValue);
        }

        [Fact]
        public async Task ExportPlayerOffersToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "Offer Status,E-mail,First Name,Last Name,Position,Age,Height,Foot,Salary,Additional Information,Club Member's E-mail,Club Member's First Name,Club Member's Last Name,Club Name,League,Region,Creation Date,End Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockPlayerOfferRepository.Setup(repo => repo.ExportPlayerOffersToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _playerOfferController.ExportPlayerOffersToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("player-offers.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}