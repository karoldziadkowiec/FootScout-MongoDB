using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FootScout_MongoDB.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class FavoriteClubAdvertisementControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public FavoriteClubAdvertisementControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
        {
            _fixture = fixture;
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MongoDBContext));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddSingleton<MongoDBContext>(_fixture.DbContext);
                });
            }).CreateClient();

            var userTokenJWT = _fixture.UserTokenJWT;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userTokenJWT);
        }

        [Fact]
        public async Task AddToFavorites_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var favoriteDto = new FavoriteClubAdvertisementCreateDTO
            {
                ClubAdvertisementId = 1,
                UserId = "pepguardiola"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-advertisements/favorites", favoriteDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FavoriteClubAdvertisement>();
            Assert.NotNull(result);
            Assert.Equal(favoriteDto.ClubAdvertisementId, result.ClubAdvertisementId);
            Assert.Equal(favoriteDto.UserId, result.UserId);

            var favoriteClubAdvertisement = await _fixture.DbContext.FavoriteClubAdvertisementsCollection.Find(fca => fca.ClubAdvertisementId == 1 && fca.UserId == "pepguardiola").FirstOrDefaultAsync();
            await _fixture.DbContext.FavoriteClubAdvertisementsCollection.DeleteOneAsync(fca => fca.Id == favoriteClubAdvertisement.Id);
        }

        [Fact]
        public async Task AddToFavorites_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            FavoriteClubAdvertisementCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-advertisements/favorites", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task DeleteFromFavorites_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var favoriteDto = new FavoriteClubAdvertisementCreateDTO
            {
                ClubAdvertisementId = 1,
                UserId = "pepguardiola"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/club-advertisements/favorites", favoriteDto);
            var favoriteClubAdvertisement = await _fixture.DbContext.FavoriteClubAdvertisementsCollection.Find(fca => fca.ClubAdvertisementId == 1 && fca.UserId == "pepguardiola").FirstOrDefaultAsync();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/club-advertisements/favorites/{favoriteClubAdvertisement.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_ReturnsOk_WithFavoriteId()
        {
            // Arrange
            int clubAdvertisementId = 1;
            string userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/club-advertisements/favorites/check/{clubAdvertisementId}/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var favoriteId = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(favoriteId);
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_ReturnsOk_WhenNotFavorited()
        {
            // Arrange
            int clubAdvertisementId = 9999;
            string userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/club-advertisements/favorites/check/{clubAdvertisementId}/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var favoriteId = await response.Content.ReadAsStringAsync();
            Assert.Equal("0", favoriteId);
        }
    }
}