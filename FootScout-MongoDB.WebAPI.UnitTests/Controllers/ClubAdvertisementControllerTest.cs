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
    public class ClubAdvertisementControllerTests
    {
        private readonly Mock<IClubAdvertisementRepository> _mockClubAdvertisementRepository;
        private readonly Mock<ISalaryRangeRepository> _mockSalaryRangeRepository;
        private readonly Mock<IPlayerPositionRepository> _mockPlayerPositionRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ClubAdvertisementController _clubAdvertisementController;

        public ClubAdvertisementControllerTests()
        {
            _mockClubAdvertisementRepository = new Mock<IClubAdvertisementRepository>();
            _mockSalaryRangeRepository = new Mock<ISalaryRangeRepository>();
            _mockPlayerPositionRepository = new Mock<IPlayerPositionRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _clubAdvertisementController = new ClubAdvertisementController(_mockClubAdvertisementRepository.Object, _mockSalaryRangeRepository.Object, _mockPlayerPositionRepository.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);}

        [Fact]
        public async Task GetClubAdvertisement_ExistingId_ReturnsOkResultWithClubAdvertisement()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var clubAdvertisement = new ClubAdvertisement { Id = 1, ClubName = "Manchetster City", League = "Premier League", ClubMemberId = "pepguardiola" };
            _mockClubAdvertisementRepository.Setup(repo => repo.GetClubAdvertisement(clubAdvertisementId)).ReturnsAsync(clubAdvertisement);

            // Act
            var result = await _clubAdvertisementController.GetClubAdvertisement(clubAdvertisementId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ClubAdvertisement>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedClubAdvertisement = Assert.IsType<ClubAdvertisement>(okResult.Value);
            Assert.Equal(clubAdvertisementId, returnedClubAdvertisement.Id);
        }

        [Fact]
        public async Task GetClubAdvertisement_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var clubAdvertisementId = 1;
            _mockClubAdvertisementRepository.Setup(repo => repo.GetClubAdvertisement(clubAdvertisementId)).ReturnsAsync((ClubAdvertisement)null);

            // Act
            var result = await _clubAdvertisementController.GetClubAdvertisement(clubAdvertisementId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ClubAdvertisement>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAllClubAdvertisements_ReturnsOkResultWithClubAdvertisements()
        {
            // Arrange
            var clubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchetster City", League = "Premier League", ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { Id = 2, ClubName = "Real Madrid",League = "La Liga", ClubMemberId = "mourinho" }
            };
            _mockClubAdvertisementRepository.Setup(repo => repo.GetAllClubAdvertisements()).ReturnsAsync(clubAdvertisements);

            // Act
            var result = await _clubAdvertisementController.GetAllClubAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedClubAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(clubAdvertisements.Count, returnedClubAdvertisements.Count);
        }

        [Fact]
        public async Task GetActiveClubAdvertisements_ReturnsOkResultWithActiveClubAdvertisements()
        {
            // Arrange
            var activeClubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchetster City", League = "Premier League", ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { Id = 2, ClubName = "Real Madrid",League = "La Liga", ClubMemberId = "mourinho" }
            };
            _mockClubAdvertisementRepository.Setup(repo => repo.GetActiveClubAdvertisements()).ReturnsAsync(activeClubAdvertisements);

            // Act
            var result = await _clubAdvertisementController.GetActiveClubAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedActiveClubAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(activeClubAdvertisements.Count, returnedActiveClubAdvertisements.Count);
        }

        [Fact]
        public async Task GetActiveClubAdvertisementCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockClubAdvertisementRepository.Setup(repo => repo.GetActiveClubAdvertisementCount()).ReturnsAsync(count);

            // Act
            var result = await _clubAdvertisementController.GetActiveClubAdvertisementCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetInactiveClubAdvertisements_ReturnsOkResultWithInactiveClubAdvertisements()
        {
            // Arrange
            var inactiveClubAdvertisements = new List<ClubAdvertisement>
            {
                new ClubAdvertisement { Id = 1, ClubName = "Manchetster City", League = "Premier League", ClubMemberId = "pepguardiola" },
                new ClubAdvertisement { Id = 2, ClubName = "Real Madrid",League = "La Liga", ClubMemberId = "mourinho" }
            };
            _mockClubAdvertisementRepository.Setup(repo => repo.GetInactiveClubAdvertisements()).ReturnsAsync(inactiveClubAdvertisements);

            // Act
            var result = await _clubAdvertisementController.GetInactiveClubAdvertisements();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ClubAdvertisement>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedInactiveClubAdvertisements = Assert.IsType<List<ClubAdvertisement>>(okResult.Value);
            Assert.Equal(inactiveClubAdvertisements.Count, returnedInactiveClubAdvertisements.Count);
        }

        [Fact]
        public async Task CreateClubAdvertisement_ValidDto_ReturnsOkResultWithClubAdvertisement()
        {
            // Arrange
            var dto = new ClubAdvertisementCreateDTO
            {
                SalaryRangeDTO = new SalaryRangeDTO { Min = 1000, Max = 2000 }
            };
            var salaryRange = new SalaryRange { Id = 1 };
            var clubAdvertisement = new ClubAdvertisement { Id = 1 };

            _mockMapper.Setup(m => m.Map<SalaryRange>(dto.SalaryRangeDTO)).Returns(salaryRange);
            _mockMapper.Setup(m => m.Map<ClubAdvertisement>(dto)).Returns(clubAdvertisement);
            _mockSalaryRangeRepository.Setup(repo => repo.CreateSalaryRange(salaryRange)).Returns(Task.CompletedTask);
            _mockClubAdvertisementRepository.Setup(repo => repo.CreateClubAdvertisement(clubAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubAdvertisementController.CreateClubAdvertisement(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedClubAdvertisement = Assert.IsType<ClubAdvertisement>(okResult.Value);
            Assert.Equal(clubAdvertisement.Id, returnedClubAdvertisement.Id);
        }

        [Fact]
        public async Task UpdateClubAdvertisement_ValidIdAndDto_ReturnsNoContent()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var clubAdvertisement = new ClubAdvertisement { Id = 1, ClubName = "Juventus", League = "Serie A", ClubMemberId = "pepguardiola" };
            _mockClubAdvertisementRepository.Setup(repo => repo.UpdateClubAdvertisement(clubAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubAdvertisementController.UpdateClubAdvertisement(clubAdvertisementId, clubAdvertisement);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateClubAdvertisement_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var clubAdvertisement = new ClubAdvertisement { Id = 2, ClubName = "Real Madrid", League = "La Liga", ClubMemberId = "pepguardiola" };
            _clubAdvertisementController.ModelState.AddModelError("Id", "ID mismatch");

            // Act
            var result = await _clubAdvertisementController.UpdateClubAdvertisement(clubAdvertisementId, clubAdvertisement);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteClubAdvertisement_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var clubAdvertisement = new ClubAdvertisement { Id = 1, ClubName = "Manchetster City", League = "Premier League", ClubMemberId = "pepguardiola" };
            _mockClubAdvertisementRepository.Setup(repo => repo.GetClubAdvertisement(clubAdvertisementId)).ReturnsAsync(clubAdvertisement);
            _mockClubAdvertisementRepository.Setup(repo => repo.DeleteClubAdvertisement(clubAdvertisementId)).Returns(Task.CompletedTask);

            // Act
            var result = await _clubAdvertisementController.DeleteClubAdvertisement(clubAdvertisementId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteClubAdvertisement_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var clubAdvertisementId = 1;
            _mockClubAdvertisementRepository.Setup(repo => repo.GetClubAdvertisement(clubAdvertisementId)).ReturnsAsync((ClubAdvertisement)null);

            // Act
            var result = await _clubAdvertisementController.DeleteClubAdvertisement(clubAdvertisementId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ExportClubAdvertisementsToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "E-mail,First Name,Last Name,Position,Club Name,League,Region,Min Salary,Max Salary,Creation Date,End Date";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockClubAdvertisementRepository.Setup(repo => repo.ExportClubAdvertisementsToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _clubAdvertisementController.ExportClubAdvertisementsToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("club-advertisements.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}