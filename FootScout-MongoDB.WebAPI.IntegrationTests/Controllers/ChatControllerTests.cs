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
    public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ChatControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetChatById_ReturnsOk_WhenChatExists()
        {
            // Arrange
            var chatId = 1;

            // Act
            var response = await _client.GetAsync($"/api/chats/{chatId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var chat = await response.Content.ReadFromJsonAsync<Chat>();
            Assert.NotNull(chat);
            Assert.Equal(chatId, chat.Id);
        }

        [Fact]
        public async Task GetChatById_ReturnsNotFound_WhenChatDoesNotExist()
        {
            // Arrange
            var chatId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/chats/{chatId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetChats_ReturnsOk_WithListOfChats()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/chats");

            // Assert
            response.EnsureSuccessStatusCode();
            var chats = await response.Content.ReadFromJsonAsync<IEnumerable<Chat>>();
            Assert.NotNull(chats);
            Assert.NotEmpty(chats);
        }

        [Fact]
        public async Task GetChatCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/chats/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadAsStringAsync();
            Assert.True(int.TryParse(count, out int chatCount));
            Assert.True(chatCount > 0);
        }

        [Fact]
        public async Task GetChatIdBetweenUsers_ReturnsOk_WithChatId()
        {
            // Arrange
            var user1Id = "leomessi";
            var user2Id = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/chats/between/{user1Id}/{user2Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var chatId = await response.Content.ReadAsStringAsync();
            Assert.NotNull(chatId);
            Assert.True(int.TryParse(chatId, out int chatIdParsed));
            Assert.Equal(1, chatIdParsed);
        }

        [Fact]
        public async Task CreateChat_ReturnsCreatedAtAction_WithNewChat()
        {
            // Arrange
            var chatDto = new ChatCreateDTO
            {
                User1Id = "admin0",
                User2Id = "pepguardiola"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/chats", chatDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdChat = await response.Content.ReadFromJsonAsync<Chat>();
            Assert.NotNull(createdChat);
            Assert.Equal(chatDto.User1Id, createdChat.User1Id);
            Assert.Equal(chatDto.User2Id, createdChat.User2Id);

            var chat = await _fixture.DbContext.ChatsCollection.Find(c => c.User1Id == "admin0" && c.User2Id == "pepguardiola").FirstOrDefaultAsync();
            await _fixture.DbContext.ChatsCollection.DeleteOneAsync(c => c.Id == chat.Id);
        }

        [Fact]
        public async Task DeleteChat_ReturnsNoContent_WhenChatExists()
        {
            // Arrange
            var chatDto = new ChatCreateDTO
            {
                User1Id = "admin0",
                User2Id = "unknown9"
            };
            var response1 = await _client.PostAsJsonAsync("/api/chats", chatDto);
            var chat = await _fixture.DbContext.ChatsCollection.Find(c => c.User1Id == "admin0" && c.User2Id == "unknown9").FirstOrDefaultAsync();

            // Act
            var response2 = await _client.DeleteAsync($"/api/chats/{chat.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);
        }

        [Fact]
        public async Task DeleteChat_ReturnsNotFound_WhenChatDoesNotExist()
        {
            // Arrange
            var chatId = 9999;

            // Act
            var response = await _client.DeleteAsync($"/api/chats/{chatId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ExportChatsToCsv_ReturnsCsvFile()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/chats/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("chats.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}