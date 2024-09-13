using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class OfferStatusControllerTests
    {
        private readonly Mock<IOfferStatusRepository> _mockOfferStatusRepository;
        private readonly OfferStatusController _offerStatusController;

        public OfferStatusControllerTests()
        {
            _mockOfferStatusRepository = new Mock<IOfferStatusRepository>();
            _offerStatusController = new OfferStatusController(_mockOfferStatusRepository.Object);
        }

        [Fact]
        public async Task GetOfferStatuses_ReturnsOkResult_WithOfferStatuses()
        {
            // Arrange
            var offerStatuses = new List<OfferStatus>
            {
                new OfferStatus { Id = 1, StatusName = "Offered" },
                new OfferStatus { Id = 2, StatusName = "Accepted" },
                new OfferStatus { Id = 3, StatusName = "Rejected" }
            };

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatuses()).ReturnsAsync(offerStatuses);

            // Act
            var result = await _offerStatusController.GetOfferStatuses();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<OfferStatus>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedStatuses = Assert.IsType<List<OfferStatus>>(okResult.Value);
            Assert.Equal(offerStatuses.Count, returnedStatuses.Count);
        }

        [Fact]
        public async Task GetOfferStatus_ValidId_ReturnsOkResult_WithOfferStatus()
        {
            // Arrange
            var offerStatusId = 1;
            var offerStatus = new OfferStatus { Id = 1, StatusName = "Offered" };

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatus(offerStatusId)).ReturnsAsync(offerStatus);

            // Act
            var result = await _offerStatusController.GetOfferStatus(offerStatusId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatus = Assert.IsType<OfferStatus>(okResult.Value);
            Assert.Equal(offerStatusId, returnedStatus.Id);
        }

        [Fact]
        public async Task GetOfferStatus_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var offerStatusId = 999;

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatus(offerStatusId)).ReturnsAsync((OfferStatus)null);

            // Act
            var result = await _offerStatusController.GetOfferStatus(offerStatusId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAdvertisementStatusName_ValidId_ReturnsOkResult_WithStatusName()
        {
            // Arrange
            var statusId = 1;
            var statusName = "Offered";

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatusName(statusId)).ReturnsAsync(statusName);

            // Act
            var result = await _offerStatusController.GetAdvertisementStatusName(statusId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatusName = Assert.IsType<string>(okResult.Value);
            Assert.Equal(statusName, returnedStatusName);
        }

        [Fact]
        public async Task GetAdvertisementStatusName_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var statusId = 999;

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatusName(statusId)).ReturnsAsync((string)null);

            // Act
            var result = await _offerStatusController.GetAdvertisementStatusName(statusId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOfferStatusId_ValidName_ReturnsOkResult_WithStatusId()
        {
            // Arrange
            var statusName = "Offered";
            var statusId = 1;

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatusId(statusName)).ReturnsAsync(statusId);

            // Act
            var result = await _offerStatusController.GetOfferStatusId(statusName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStatusId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(statusId, returnedStatusId);
        }

        [Fact]
        public async Task GetOfferStatusId_InvalidName_ReturnsNotFound()
        {
            // Arrange
            var statusName = "NonExistingStatus";

            _mockOfferStatusRepository.Setup(repo => repo.GetOfferStatusId(statusName)).ReturnsAsync(0);

            // Act
            var result = await _offerStatusController.GetOfferStatusId(statusName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}