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
    public class MessageControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public MessageControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetAllMessages_ReturnsOk_WithListOfMessages()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/messages");

            // Assert
            response.EnsureSuccessStatusCode();
            var messages = await response.Content.ReadFromJsonAsync<IEnumerable<Message>>();
            Assert.NotNull(messages);
            Assert.NotEmpty(messages);
        }

        [Fact]
        public async Task GetAllMessagesCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/messages/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(count, out int messageCount));
            Assert.True(messageCount > 0);
        }

        [Fact]
        public async Task GetMessagesForChat_ReturnsOk_WithListOfMessages()
        {
            // Arrange
            var chatId = 1;

            // Act
            var response = await _client.GetAsync($"/api/messages/chat/{chatId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var messages = await response.Content.ReadFromJsonAsync<IEnumerable<Message>>();
            Assert.NotNull(messages);
            Assert.NotEmpty(messages);
        }

        [Fact]
        public async Task GetMessagesForChatCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var chatId = 1;

            // Act
            var response = await _client.GetAsync($"/api/messages/chat/{chatId}/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(count, out int messageCount));
            Assert.True(messageCount > 0);
        }

        [Fact]
        public async Task GetLastMessageDateForChat_ReturnsOk_WithLastMessageDate()
        {
            // Arrange
            var chatId = 1;

            // Act
            var response = await _client.GetAsync($"/api/messages/chat/{chatId}/last-message-date");

            // Assert
            response.EnsureSuccessStatusCode();
            var lastMessageDate = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(lastMessageDate));
        }

        [Fact]
        public async Task DeleteMessage_ReturnsNoContent_WhenMessageExists()
        {
            // Arrange
            var messageId = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/messages/{messageId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteMessage_ReturnsNotFound_WhenMessageDoesNotExist()
        {
            // Arrange
            var messageId = 9999;

            // Act
            var response = await _client.DeleteAsync($"/api/messages/{messageId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}