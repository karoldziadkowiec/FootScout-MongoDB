using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class ClubHistoryControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ClubHistoryControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetClubHistory_ReturnsOk_WhenClubHistoryExists()
        {
            // Arrange
            var clubHistoryId = 1;

            // Act
            var response = await _client.GetAsync($"/api/club-history/{clubHistoryId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubHistory = await response.Content.ReadFromJsonAsync<ClubHistory>();
            Assert.NotNull(clubHistory);
            Assert.Equal(clubHistoryId, clubHistory.Id);
        }

        [Fact]
        public async Task GetClubHistory_ReturnsNotFound_WhenClubHistoryDoesNotExist()
        {
            // Arrange
            var clubHistoryId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/club-history/{clubHistoryId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClubHistory_ReturnsOk_WithListOfClubHistories()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/club-history");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubHistories = await response.Content.ReadFromJsonAsync<IEnumerable<ClubHistory>>();
            Assert.NotNull(clubHistories);
            Assert.NotEmpty(clubHistories);
        }

        [Fact]
        public async Task GetClubHistoryCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/club-history/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(count, out int clubHistoryCount));
            Assert.True(clubHistoryCount > 0);
        }

        [Fact]
        public async Task CreateClubHistory_ReturnsOk_WhenValidDtoProvided()
        {
            // Arrange
            var clubHistoryCreateDto = new ClubHistoryCreateDTO
            {
                PlayerPositionId = 15,
                ClubName = "Club",
                League = "League",
                Region = "Region",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(150),
                Achievements = new Achievements { NumberOfMatches = 1, Goals = 2, Assists = 3, AdditionalAchievements = "No info" },
                PlayerId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-history", clubHistoryCreateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdClubHistory = await response.Content.ReadFromJsonAsync<ClubHistory>();
            Assert.NotNull(createdClubHistory);
            Assert.Equal(clubHistoryCreateDto.ClubName, createdClubHistory.ClubName);

            var clubHistory = await _fixture.DbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "Club").FirstOrDefaultAsync();
            await _fixture.DbContext.ClubHistoriesCollection.DeleteOneAsync(ch => ch.Id == clubHistory.Id);
        }

        [Fact]
        public async Task CreateClubHistory_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange
            ClubHistoryCreateDTO clubHistoryCreateDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-history", clubHistoryCreateDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClubHistory_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var clubHistory = await _fixture.DbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "FC Barcelona").FirstOrDefaultAsync();
            clubHistory.PlayerPositionId = 14;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-history/{clubHistory.Id}", clubHistory);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClubHistory_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var clubHistoryId = 2;
            var clubHistory = await _fixture.DbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "FC Barcelona").FirstOrDefaultAsync();
            clubHistory.PlayerPositionId = 14;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-history/{clubHistoryId}", clubHistory);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClubHistory_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var clubHistoryCreateDto = new ClubHistoryCreateDTO
            {
                PlayerPositionId = 15,
                ClubName = "Club",
                League = "League",
                Region = "Region",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(150),
                Achievements = new Achievements { NumberOfMatches = 1, Goals = 2, Assists = 3, AdditionalAchievements = "No info" },
                PlayerId = "leomessi"
            };
            var response1 = await _client.PostAsJsonAsync("/api/club-history", clubHistoryCreateDto);
            var clubHistory = await _fixture.DbContext.ClubHistoriesCollection.Find(ch => ch.ClubName == "Club").FirstOrDefaultAsync();

            // Act
            var response2 = await _client.DeleteAsync($"/api/club-history/{clubHistory.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);
        }

        [Fact]
        public async Task DeleteClubHistory_ReturnsNotFound_WhenClubHistoryDoesNotExist()
        {
            // Arrange
            var clubHistoryId = 9999;

            // Act
            var response = await _client.DeleteAsync($"/api/club-history/{clubHistoryId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}