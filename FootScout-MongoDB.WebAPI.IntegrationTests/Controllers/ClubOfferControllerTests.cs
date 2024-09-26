using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FootScout_MongoDB.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Controllers
{
    public class ClubOfferControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ClubOfferControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetClubOffer_ReturnsOk_WhenClubOfferExists()
        {
            // Arrange
            var clubOfferId = 1;

            // Act
            var response = await _client.GetAsync($"/api/club-offers/{clubOfferId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubOffer = await response.Content.ReadFromJsonAsync<ClubOffer>();
            Assert.Equal(clubOfferId, clubOffer.Id);
        }

        [Fact]
        public async Task GetClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var playerOfferId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/club-offers/{playerOfferId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClubOffers_ReturnsOk_WhenClubOffersExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/club-offers");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(clubOffers);
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsOk_WhenActiveClubOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-offers/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var activeClubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(activeClubOffers);
        }

        [Fact]
        public async Task GetActiveClubOffersCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/club-offers/active/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsOk_WhenInactiveClubOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-offers/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var inactiveClubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.Empty(inactiveClubOffers);
        }

        [Fact]
        public async Task CreateClubOffer_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var adDto = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 232,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-offers", adDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClubOffer>();
            Assert.NotNull(result);
            Assert.Equal(adDto.PlayerPositionId, result.PlayerPositionId);
            Assert.Equal(adDto.ClubName, result.ClubName);
            Assert.Equal(adDto.League, result.League);
            Assert.Equal(adDto.Region, result.Region);
            Assert.Equal(adDto.Salary, result.Salary);
            Assert.Equal(adDto.ClubMemberId, result.ClubMemberId);

            var clubOffer = await _fixture.DbContext.ClubOffersCollection.Find(co => co.PlayerPositionId == adDto.PlayerPositionId && co.ClubName == adDto.ClubName && co.League == adDto.League && co.Region == adDto.Region && co.Salary == adDto.Salary && co.ClubMemberId == adDto.ClubMemberId).FirstOrDefaultAsync();
            await _fixture.DbContext.ClubOffersCollection.DeleteOneAsync(co => co.Id == clubOffer.Id);
        }

        [Fact]
        public async Task CreateClubOffer_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            ClubOfferCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-offers", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task UpdateClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var offerToUpdate = await _fixture.DbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();
            offerToUpdate.ClubName = "Inter Mediolan";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/{offerToUpdate.Id}", offerToUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClubOffer_RemovesClubOffer()
        {
            // Arrange
            var newAd = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 233,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response = await _client.PostAsJsonAsync("/api/club-offers", newAd);
            var clubOffer = await _fixture.DbContext.ClubOffersCollection.Find(co => co.PlayerPositionId == newAd.PlayerPositionId && co.ClubName == newAd.ClubName && co.League == newAd.League && co.Region == newAd.Region && co.Salary == newAd.Salary && co.ClubMemberId == newAd.ClubMemberId).FirstOrDefaultAsync();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/club-offers/{clubOffer.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task AcceptClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var newAd = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 237,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response1 = await _client.PostAsJsonAsync("/api/club-offers", newAd);
            var offerToAccept = await _fixture.DbContext.ClubOffersCollection.Find(co => co.Salary == 237).FirstOrDefaultAsync();

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/club-offers/accept/{offerToAccept.Id}", offerToAccept);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.ClubOffersCollection.Find(co => co.Salary == 237).FirstOrDefaultAsync();
            await _fixture.DbContext.ClubOffersCollection.DeleteOneAsync(co => co.Id == clubOffer.Id);
        }

        [Fact]
        public async Task AcceptClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/accept/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RejectClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var newAd = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 238,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response1 = await _client.PostAsJsonAsync("/api/club-offers", newAd);
            var offerToAccept = await _fixture.DbContext.ClubOffersCollection.Find(co => co.Salary == 238).FirstOrDefaultAsync();

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/club-offers/reject/{offerToAccept.Id}", offerToAccept);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.ClubOffersCollection.Find(co => co.Salary == 238).FirstOrDefaultAsync();
            await _fixture.DbContext.ClubOffersCollection.DeleteOneAsync(co => co.Id == clubOffer.Id);
        }

        [Fact]
        public async Task RejectClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubAdvertisementId = 2;
            var advertisementToUpdate = await _fixture.DbContext.ClubOffersCollection.Find(_ => true).FirstOrDefaultAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/reject/{clubAdvertisementId}", advertisementToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetClubOfferStatusId_ReturnsOk_WhenClubOfferExists()
        {
            // Arrange
            var clubOfferId = 2;
            var playerId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/club-offers/status/{clubOfferId}/{playerId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatusId = await response.Content.ReadFromJsonAsync<int>();
            Assert.Equal(2, offerStatusId);
        }

        [Fact]
        public async Task ExportClubOffersToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/club-offers/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("club-offers.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}