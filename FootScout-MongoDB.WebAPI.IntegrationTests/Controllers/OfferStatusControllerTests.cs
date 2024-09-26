using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class OfferStatusControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public OfferStatusControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetOfferStatuses_ReturnsOk_WhenOfferStatusesExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/offer-statuses");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatuses = await response.Content.ReadFromJsonAsync<IEnumerable<OfferStatus>>();
            Assert.NotEmpty(offerStatuses);
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsOk_WhenOfferStatusExists()
        {
            // Arrange
            var offerStatusId = 1;

            // Act
            var response = await _client.GetAsync($"/api/offer-statuses/{offerStatusId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatus = await response.Content.ReadFromJsonAsync<OfferStatus>();
            Assert.Equal(offerStatusId, offerStatus.Id);
        }

        [Fact]
        public async Task GetOfferStatus_ReturnsNotFound_WhenOfferStatusDoesNotExist()
        {
            // Arrange
            var offerStatusId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/offer-statuses/{offerStatusId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetOfferStatusName_ReturnsOk_WhenOfferStatusExists()
        {
            // Arrange
            var offerStatusId = 1;

            // Act
            var response = await _client.GetAsync($"/api/offer-statuses/name/{offerStatusId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatusName = await response.Content.ReadAsStringAsync();
            Assert.Equal("Offered", offerStatusName);
        }

        [Fact]
        public async Task GetOfferStatusId_ReturnsOk_WhenOfferStatusExists()
        {
            // Arrange
            var offerStatusName = "Offered";

            // Act
            var response = await _client.GetAsync($"/api/offer-statuses/id/{offerStatusName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offeredStatusId = await response.Content.ReadAsStringAsync();
            int parsedOfferedStatusId = int.Parse(offeredStatusId);

            Assert.Equal(1, parsedOfferedStatusId);
        }
    }
}