using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class PlayerFootControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PlayerFootControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetPlayerFeet_ReturnsOk_WhenPlayerFeetExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/player-feet");

            // Assert
            response.EnsureSuccessStatusCode();
            var playerFeet = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerFoot>>();
            Assert.NotEmpty(playerFeet);
        }

        [Fact]
        public async Task GetPlayerFootName_ReturnsOk_WhenFootExists()
        {
            // Arrange
            var footId = 1;

            // Act
            var response = await _client.GetAsync($"/api/player-feet/{footId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var footName = await response.Content.ReadAsStringAsync();
            Assert.Equal("Left", footName);
        }
    }
}