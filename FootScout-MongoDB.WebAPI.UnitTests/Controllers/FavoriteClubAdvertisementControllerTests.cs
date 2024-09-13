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
    public class FavoriteClubAdvertisementControllerTests
    {
        private readonly Mock<IFavoriteClubAdvertisementRepository> _mockFavoriteClubAdvertisementRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IClubAdvertisementRepository> _mockClubAdvertisementRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FavoriteClubAdvertisementController _favoriteClubAdvertisementController;

        public FavoriteClubAdvertisementControllerTests()
        {
            _mockFavoriteClubAdvertisementRepository = new Mock<IFavoriteClubAdvertisementRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockClubAdvertisementRepository = new Mock<IClubAdvertisementRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _favoriteClubAdvertisementController = new FavoriteClubAdvertisementController(_mockFavoriteClubAdvertisementRepository.Object, _mockNewIdGeneratorService.Object, _mockUserRepository.Object, _mockClubAdvertisementRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddToFavorites_ValidDto_ReturnsOkResultWithFavoriteClubAdvertisement()
        {
            // Arrange
            var dto = new FavoriteClubAdvertisementCreateDTO { ClubAdvertisementId = 1, UserId = "leomessi" };
            var favoriteClubAdvertisement = new FavoriteClubAdvertisement { Id = 1, ClubAdvertisementId = 1, UserId = "leomessi" };

            _mockMapper.Setup(m => m.Map<FavoriteClubAdvertisement>(dto)).Returns(favoriteClubAdvertisement);
            _mockFavoriteClubAdvertisementRepository.Setup(repo => repo.AddToFavorites(favoriteClubAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _favoriteClubAdvertisementController.AddToFavorites(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFavorite = Assert.IsType<FavoriteClubAdvertisement>(okResult.Value);
            Assert.Equal(favoriteClubAdvertisement.Id, returnedFavorite.Id);
            Assert.Equal(favoriteClubAdvertisement.ClubAdvertisementId, returnedFavorite.ClubAdvertisementId);
            Assert.Equal(favoriteClubAdvertisement.UserId, returnedFavorite.UserId);
        }

        [Fact]
        public async Task AddToClubFavorites_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            FavoriteClubAdvertisementCreateDTO dto = null;

            // Act
            var result = await _favoriteClubAdvertisementController.AddToFavorites(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid dto data.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteFromClubFavorites_ValidId_ReturnsNoContent()
        {
            // Arrange
            var favoriteClubAdvertisementId = 1;

            _mockFavoriteClubAdvertisementRepository.Setup(repo => repo.DeleteFromFavorites(favoriteClubAdvertisementId)).Returns(Task.CompletedTask);

            // Act
            var result = await _favoriteClubAdvertisementController.DeleteFromFavorites(favoriteClubAdvertisementId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_ExistingFavorite_ReturnsOkResultWithFavoriteId()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var userId = "leomessi";
            var favoriteId = 1;

            _mockFavoriteClubAdvertisementRepository.Setup(repo => repo.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId)).ReturnsAsync(favoriteId);

            // Act
            var result = await _favoriteClubAdvertisementController.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFavoriteId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(favoriteId, returnedFavoriteId);
        }
    }
}