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
    public class ClubAdvertisementControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ClubAdvertisementControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetClubAdvertisement_ReturnsOk_WhenClubAdvertisementExists()
        {
            // Arrange
            var clubAdvertisementId = 1;

            // Act
            var response = await _client.GetAsync($"/api/club-advertisements/{clubAdvertisementId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubAdvertisement = await response.Content.ReadFromJsonAsync<ClubAdvertisement>();
            Assert.Equal(clubAdvertisementId, clubAdvertisement.Id);
        }

        [Fact]
        public async Task GetClubAdvertisement_ReturnsNotFound_WhenClubAdvertisementDoesNotExist()
        {
            // Arrange
            var playerAdvertisementId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/club-advertisements/{playerAdvertisementId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClubAdvertisements_ReturnsOk_WhenClubAdvertisementsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/club-advertisements");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.NotEmpty(clubAdvertisements);
        }

        [Fact]
        public async Task GetActiveClubAdvertisements_ReturnsOk_WhenActiveClubAdvertisementsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-advertisements/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var activeClubAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.NotEmpty(activeClubAdvertisements);
        }

        [Fact]
        public async Task GetActiveClubAdvertisementsCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/club-advertisements/active/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetInactiveClubAdvertisements_ReturnsOk_WhenInactiveClubAdvertisementsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-advertisements/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var inactiveClubAdvertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.Empty(inactiveClubAdvertisements);
        }

        [Fact]
        public async Task CreateClubAdvertisement_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var adDto = new ClubAdvertisementCreateDTO
            {
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                SalaryRangeDTO = new SalaryRangeDTO { Min = 251, Max = 301 },
                ClubMemberId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-advertisements", adDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClubAdvertisement>();
            Assert.NotNull(result);
            Assert.Equal(adDto.PlayerPositionId, result.PlayerPositionId);
            Assert.Equal(adDto.ClubName, result.ClubName);
            Assert.Equal(adDto.League, result.League);
            Assert.Equal(adDto.Region, result.Region);
            Assert.Equal(adDto.ClubMemberId, result.ClubMemberId);

            var clubAdvertisement = await _fixture.DbContext.ClubAdvertisementsCollection.Find(ca => ca.PlayerPositionId == adDto.PlayerPositionId && ca.ClubName == adDto.ClubName &&  ca.League == adDto.League && ca.Region == adDto.Region && ca.ClubMemberId == adDto.ClubMemberId).FirstOrDefaultAsync();
            await _fixture.DbContext.ClubAdvertisementsCollection.DeleteOneAsync(ca => ca.Id == clubAdvertisement.Id);
        }

        [Fact]
        public async Task CreateClubAdvertisement_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            ClubAdvertisementCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-advertisements", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task UpdateClubAdvertisement_ReturnsNoContent_WhenClubAdvertisementExists()
        {
            // Arrange
            var advertisementToUpdate = await _fixture.DbContext.ClubAdvertisementsCollection.Find(_ => true).FirstOrDefaultAsync();
            advertisementToUpdate.ClubName = "Inter Mediolan";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-advertisements/{advertisementToUpdate.Id}", advertisementToUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClubAdvertisement_ReturnsNotFound_WhenClubAdvertisementDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.ClubAdvertisementsCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-advertisements/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFromFavorites_DeleteClubAdFromFavorites()
        {
            // Arrange
            var newAd = new ClubAdvertisementCreateDTO
            {
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                SalaryRangeDTO = new SalaryRangeDTO { Min = 252, Max = 302 },
                ClubMemberId = "leomessi"
            };
            var response = await _client.PostAsJsonAsync("/api/club-advertisements", newAd);
            var clubAdvertisement = await _fixture.DbContext.ClubAdvertisementsCollection.Find(ca => ca.PlayerPositionId == newAd.PlayerPositionId && ca.ClubName == newAd.ClubName && ca.League == newAd.League && ca.Region == newAd.Region && ca.ClubMemberId == newAd.ClubMemberId).FirstOrDefaultAsync();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/club-advertisements/{clubAdvertisement.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task ExportClubAdvertisementsToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/club-advertisements/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("club-advertisements.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}