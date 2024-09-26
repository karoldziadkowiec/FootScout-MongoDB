using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FootScout_MongoDB.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;
using FootScout_MongoDB.WebAPI.Entities;
using System.Net;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class PlayerPositionControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerPositionControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetPlayerPositions_ReturnsOk_WhenPlayerPositionsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-positions");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerPositions = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerPosition>>();
            Assert.NotEmpty(playerPositions);
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 15;

            // Act
            var response = await _client.GetAsync("/api/player-positions/count");

            // Assert
            response.EnsureSuccessStatusCode();

            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsOk_WhenPositionExists()
        {
            // Arrange
            var positionId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-positions/{positionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var positionName = await response.Content.ReadAsStringAsync();
            Assert.Equal("Goalkeeper", positionName);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/player-positions/{positionId}");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists()
        {
            // Arrange
            var positionName = "Goalkeeper";

            // Act
            var response = await _client.GetAsync($"/api/player-positions/check/name/{positionName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var isExists = await response.Content.ReadAsStringAsync();
            Assert.Equal("true", isExists);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionName = "UnknownPosition";

            // Act
            var response = await _client.GetAsync($"/api/player-positions/check/name/{positionName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var isExists = await response.Content.ReadAsStringAsync();
            Assert.Equal("false", isExists);
        }

        [Fact]
        public async Task CreatePlayerPosition_ReturnsOk_WhenPositionIsValid()
        {
            // Arrange
            var playerPosition = new PlayerPosition { PositionName = "NewPosition" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-positions", playerPosition);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdPosition = await response.Content.ReadFromJsonAsync<PlayerPosition>();
            Assert.Equal(playerPosition.PositionName, createdPosition.PositionName);

            var playerPosition2 = await _fixture.DbContext.PlayerPositionsCollection.Find(pp => pp.PositionName == "NewPosition").FirstOrDefaultAsync();
            await _fixture.DbContext.PlayerPositionsCollection.DeleteOneAsync(pp => pp.Id == playerPosition2.Id);
        }

        [Fact]
        public async Task CreatePlayerPosition_ReturnsBadRequest_WhenPositionIsNull()
        {
            // Arrange
            PlayerPosition playerPosition = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-positions", playerPosition);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
