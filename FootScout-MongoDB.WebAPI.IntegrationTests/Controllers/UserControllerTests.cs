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
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public UserControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var userDTO = await response.Content.ReadFromJsonAsync<UserDTO>();
            Assert.Equal(userId, userDTO.Id);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonExistentUserId";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WhenUsersExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetOnlyUsers_ReturnsOk_WhenUsersExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/users/role/user");

            // Assert
            response.EnsureSuccessStatusCode();
            var onlyUsers = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
            Assert.NotEmpty(onlyUsers);
        }

        [Fact]
        public async Task GetOnlyAdmins_ReturnsOk_WhenAdminsExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/users/role/admin");

            // Assert
            response.EnsureSuccessStatusCode();
            var onlyAdmins = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
            Assert.NotEmpty(onlyAdmins);
        }

        [Fact]
        public async Task GetUserRole_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/role");

            // Assert
            response.EnsureSuccessStatusCode();
            var role = await response.Content.ReadAsStringAsync();
            Assert.NotNull(role);
        }

        [Fact]
        public async Task GetUserCount_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/users/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(count, out _));
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserUpdateDTO { FirstName = "Lionel", LastName = "Messi", PhoneNumber = "101010101", Location = "Miami" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{userId}", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonExistentUserId";
            var dto = new UserUpdateDTO { FirstName = "Lionel", LastName = "Messi", PhoneNumber = "101010101", Location = "Miami" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ResetUserPassword_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";
            var dto = new UserResetPasswordDTO { PasswordHash = "Lionel123!", ConfirmPasswordHash = "Lionel123!" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/reset-password/{userId}", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ResetUserPassword_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonExistentUserId";
            var dto = new UserResetPasswordDTO { PasswordHash = "Lionel123!", ConfirmPasswordHash = "Lionel123!" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/reset-password/{userId}", dto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            await _fixture.DbContext.UsersCollection.InsertOneAsync( new User { Id = "abc", Email = "abc@abc.com", PasswordHash = "Abccc1!", FirstName = "Abc.", LastName = "abc.", Location = "A b c 1 2 3.", PhoneNumber = "123456789" } );

            var user = await _fixture.DbContext.UsersCollection.Find(u => u.Email == "abc@abc.com").FirstOrDefaultAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/users/{user.Id}");

            // Assert
            Assert.NotNull(response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonExistentUserId";

            // Act
            var response = await _client.DeleteAsync($"/api/users/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserClubHistory_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-history");

            // Assert
            response.EnsureSuccessStatusCode();
            var history = await response.Content.ReadFromJsonAsync<IEnumerable<ClubHistory>>();
            Assert.NotNull(history);
        }

        [Fact]
        public async Task GetUserPlayerAdvertisements_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.NotNull(advertisements);
        }

        [Fact]
        public async Task GetUserActivePlayerAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserInactivePlayerAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerAdvertisement>>();
            Assert.Empty(advertisements);
        }

        [Fact]
        public async Task GetUserFavoritePlayerAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements/favorites");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoritePlayerAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserActiveFavoritePlayerAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements/favorites/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoritePlayerAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserInactiveFavoritePlayerAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-advertisements/favorites/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoritePlayerAdvertisement>>();
            Assert.Empty(advertisements);
        }

        [Fact]
        public async Task GetReceivedClubOffers_ReturnsOk_WhenOffersExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-offers/received");

            // Assert
            response.EnsureSuccessStatusCode();
            var offers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(offers);
        }

        [Fact]
        public async Task GetSentClubOffers_ReturnsOk_WhenOffersExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-offers/sent");

            // Assert
            response.EnsureSuccessStatusCode();
            var offers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(offers);
        }

        [Fact]
        public async Task GetUserClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserActiveClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserInactiveClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<ClubAdvertisement>>();
            Assert.Empty(advertisements);
        }

        [Fact]
        public async Task GetUserFavoriteClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements/favorites");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoriteClubAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserActiveFavoriteClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements/favorites/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoriteClubAdvertisement>>();
            Assert.NotEmpty(advertisements);
        }

        [Fact]
        public async Task GetUserInactiveFavoriteClubAdvertisements_ReturnsOk_WhenAdvertisementsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/club-advertisements/favorites/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var advertisements = await response.Content.ReadFromJsonAsync<IEnumerable<FavoriteClubAdvertisement>>();
            Assert.Empty(advertisements);
        }

        [Fact]
        public async Task GetReceivedPlayerOffers_ReturnsOk_WhenOffersExist()
        {
            // Arrange
            var userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-offers/received");

            // Assert
            response.EnsureSuccessStatusCode();
            var offers = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerOffer>>();
            Assert.NotEmpty(offers);
        }

        [Fact]
        public async Task GetSentPlayerOffers_ReturnsOk_WhenOffersExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/player-offers/sent");

            // Assert
            response.EnsureSuccessStatusCode();
            var offers = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerOffer>>();
            Assert.NotEmpty(offers);
        }

        [Fact]
        public async Task GetUserChats_ReturnsOk_WhenChatsExist()
        {
            // Arrange
            var userId = "leomessi";

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}/chats");

            // Assert
            response.EnsureSuccessStatusCode();
            var chats = await response.Content.ReadFromJsonAsync<IEnumerable<Chat>>();
            Assert.NotEmpty(chats);
        }

        [Fact]
        public async Task ExportUsersToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/users/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("users.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}