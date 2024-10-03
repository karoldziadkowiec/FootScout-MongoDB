using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Headers;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class PerformanceTestsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public PerformanceTestsControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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

            var adminTokenJWT = _fixture.AdminTokenJWT;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminTokenJWT);
        }

        [Fact]
        public async Task SeedComponents_ReturnsNoContent_WhenValidTestCounterNumber()
        {
            // Arrange
            var testCounter = 10;

            // Act
            var response = await _client.PostAsync($"/api/performance-tests/seed/{testCounter}", null);

            // Assert
            Assert.NotNull(response.StatusCode);

            var usersCount = await _fixture.DbContext.UsersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);
            Assert.True(usersCount >= testCounter);

            var userRolesCount = await _fixture.DbContext.UserRolesCollection.CountDocumentsAsync(FilterDefinition<UserRole>.Empty);
            Assert.True(userRolesCount >= testCounter);

            // Cleaning
            var clearResponse2 = await _client.DeleteAsync("/api/performance-tests/clear");
            clearResponse2.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SeedComponents_ReturnsBadRequest_WhenInvalidTestCounterNumber()
        {
            // Arrange
            var testCounter = -1;

            // Act
            var response = await _client.PostAsync($"/api/performance-tests/seed/{testCounter}", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ClearDatabaseOfSeededComponents_ReturnsNoContent_WhenProccessSucceded()
        {
            // Arrange
            var expectedValue = 0;

            // Act
            var response = await _client.DeleteAsync($"/api/performance-tests/clear");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            response.EnsureSuccessStatusCode();

            var chatsCount = await _fixture.DbContext.ChatsCollection.CountDocumentsAsync(FilterDefinition<Chat>.Empty);
            Assert.Equal(expectedValue, chatsCount);

            var messagesCount = await _fixture.DbContext.MessagesCollection.CountDocumentsAsync(FilterDefinition<Message>.Empty);
            Assert.Equal(expectedValue, messagesCount);

            var playerAdvertisementsCount = await _fixture.DbContext.PlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<PlayerAdvertisement>.Empty);
            Assert.Equal(expectedValue, playerAdvertisementsCount);

            var favoritePlayerAdvertisementsCount = await _fixture.DbContext.FavoritePlayerAdvertisementsCollection.CountDocumentsAsync(FilterDefinition<FavoritePlayerAdvertisement>.Empty);
            Assert.Equal(expectedValue, favoritePlayerAdvertisementsCount);

            var clubOffersCount = await _fixture.DbContext.ClubOffersCollection.CountDocumentsAsync(FilterDefinition<ClubOffer>.Empty);
            Assert.Equal(expectedValue, clubOffersCount);

            var salaryRangesCount = await _fixture.DbContext.SalaryRangesCollection.CountDocumentsAsync(FilterDefinition<SalaryRange>.Empty);
            Assert.Equal(expectedValue, salaryRangesCount);
        }
    }
}