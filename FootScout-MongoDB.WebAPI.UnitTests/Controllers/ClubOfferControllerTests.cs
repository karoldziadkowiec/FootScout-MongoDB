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
    public class ClubOfferControllerTests
    {
        private readonly Mock<IClubOfferRepository> _mockClubOfferRepository;
        private readonly Mock<IPlayerAdvertisementRepository> _mockPlayerAdvertisementRepository;
        private readonly Mock<IOfferStatusRepository> _mockOfferStatusRepository;
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ClubOfferController _clubOfferController;

        public ClubOfferControllerTests()
        {
            _mockClubOfferRepository = new Mock<IClubOfferRepository>();
            _mockPlayerAdvertisementRepository = new Mock<IPlayerAdvertisementRepository>();
            _mockOfferStatusRepository = new Mock<IOfferStatusRepository>();
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _clubOfferController = new ClubOfferController(_mockClubOfferRepository.Object, _mockPlayerAdvertisementRepository.Object, _mockOfferStatusRepository.Object, _mockPlayerPositionRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetClubOffer_ExistingClubOffer_ReturnsOkResultWithClubOffer()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" };
            _mockClubOfferRepository.Setup(repo => repo.GetClubOffer(clubOfferId)).ReturnsAsync(clubOffer);

            // Act
            var result = await _clubOfferController.GetClubOffer(clubOfferId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ClubOffer>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<ClubOffer>(okResult.Value);
            Assert.Equal(clubOfferId, returnValue.Id);
        }

        [Fact]
        public async Task GetClubOffer_NonExistingClubOffer_ReturnsNotFound()
        {
            // Arrange
            var clubOfferId = 1;
            _mockClubOfferRepository.Setup(repo => repo.GetClubOffer(clubOfferId)).ReturnsAsync((ClubOffer)null);

            // Act
            var result = await _clubOfferController.GetClubOffer(clubOfferId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetClubOffers_ReturnsOkResultWithListOfClubOffers()
        {
            // Arrange
            var clubOffers = new List<ClubOffer>
            {
                new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" },
                new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" }
            };
            _mockClubOfferRepository.Setup(repo => repo.GetClubOffers()).ReturnsAsync(clubOffers);

            // Act
            var result = await _clubOfferController.GetClubOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<ClubOffer>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsOkResultWithActiveClubOffers()
        {
            // Arrange
            var activeClubOffers = new List<ClubOffer>
            {
                new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" },
                new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" }
            };
            _mockClubOfferRepository.Setup(repo => repo.GetActiveClubOffers()).ReturnsAsync(activeClubOffers);

            // Act
            var result = await _clubOfferController.GetActiveClubOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<ClubOffer>>(okResult.Value);
            Assert.Equal(activeClubOffers.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetActiveClubOfferCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockClubOfferRepository.Setup(repo => repo.GetActiveClubOfferCount()).ReturnsAsync(count);

            // Act
            var result = await _clubOfferController.GetActiveClubOfferCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnValue);
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsOkResultWithInactiveClubOffers()
        {
            // Arrange
            var inactiveClubOffers = new List<ClubOffer>
            {
                new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" },
                new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" }
            };
            _mockClubOfferRepository.Setup(repo => repo.GetInactiveClubOffers()).ReturnsAsync(inactiveClubOffers);

            // Act
            var result = await _clubOfferController.GetInactiveClubOffers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubOffer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<ClubOffer>>(okResult.Value);
            Assert.Equal(inactiveClubOffers.Count, returnValue.Count);
        }

        [Fact]
        public async Task CreateClubOffer_ValidDto_ReturnsOkResultWithClubOffer()
        {
            // Arrange
            var dto = new ClubOfferCreateDTO();
            var clubOffer = new ClubOffer { Id = 1 };
            _mockMapper.Setup(m => m.Map<ClubOffer>(dto)).Returns(clubOffer);
            _mockClubOfferRepository.Setup(repo => repo.CreateClubOffer(clubOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubOfferController.CreateClubOffer(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ClubOffer>(okResult.Value);
            Assert.Equal(clubOffer.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdateClubOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = clubOfferId };
            _mockClubOfferRepository.Setup(repo => repo.UpdateClubOffer(clubOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubOfferController.UpdateClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateClubOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" };

            // Act
            var result = await _clubOfferController.UpdateClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteClubOffer_ExistingClubOffer_ReturnsNoContent()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = clubOfferId };
            _mockClubOfferRepository.Setup(repo => repo.GetClubOffer(clubOfferId)).ReturnsAsync(clubOffer);
            _mockClubOfferRepository.Setup(repo => repo.DeleteClubOffer(clubOfferId)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubOfferController.DeleteClubOffer(clubOfferId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteClubOffer_NonExistingClubOffer_ReturnsNotFound()
        {
            // Arrange
            var clubOfferId = 1;
            _mockClubOfferRepository.Setup(repo => repo.GetClubOffer(clubOfferId)).ReturnsAsync((ClubOffer)null);

            // Act
            var result = await _clubOfferController.DeleteClubOffer(clubOfferId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Club offer : {clubOfferId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task AcceptClubOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" };
            _mockClubOfferRepository.Setup(repo => repo.AcceptClubOffer(clubOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubOfferController.AcceptClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AcceptClubOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" };

            // Act
            var result = await _clubOfferController.AcceptClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RejectClubOffer_ValidId_ReturnsNoContent()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 1, ClubName = "Manchester City", ClubMemberId = "pepguardiola" };
            _mockClubOfferRepository.Setup(repo => repo.RejectClubOffer(clubOffer)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubOfferController.RejectClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RejectClubOffer_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var clubOfferId = 1;
            var clubOffer = new ClubOffer { Id = 2, ClubName = "Real Madrid", ClubMemberId = "mourinho" };

            // Act
            var result = await _clubOfferController.RejectClubOffer(clubOfferId, clubOffer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetClubOfferStatusId_ReturnsOkResultWithStatusId()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var userId = "pepguardiola";
            var statusId = 1;
            _mockClubOfferRepository.Setup(repo => repo.GetClubOfferStatusId(playerAdvertisementId, userId)).ReturnsAsync(statusId);

            // Act
            var result = await _clubOfferController.GetClubOfferStatusId(playerAdvertisementId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(statusId, returnValue);
        }

        [Fact]
        public async Task ExportClubOffersToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "Offer Status,E-mail,First Name,Last Name,Position,Club Name,League,Region,Salary,Additional Information,Player's E-mail,Player's First Name,Player's Last Name,Age,Height,Foot,Creation Date,End Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockClubOfferRepository.Setup(repo => repo.ExportClubOffersToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _clubOfferController.ExportClubOffersToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("club-offers.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}