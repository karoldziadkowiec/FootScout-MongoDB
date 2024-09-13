using AutoMapper;
using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INewIdGeneratorService> _mockNewIdGeneratorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ChatController _chatController;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNewIdGeneratorService = new Mock<INewIdGeneratorService>();
            _mockMapper = new Mock<IMapper>();
            _chatController = new ChatController(_mockChatService.Object, _mockUserRepository.Object, _mockNewIdGeneratorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetChatById_ExistingChat_ReturnsOkResultWithChat()
        {
            // Arrange
            var chatId = 1;
            var chat = new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" };
            _mockChatService.Setup(service => service.GetChatById(chatId)).ReturnsAsync(chat);

            // Act
            var result = await _chatController.GetChatById(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChat = Assert.IsType<Chat>(okResult.Value);
            Assert.Equal(chatId, returnedChat.Id);
        }

        [Fact]
        public async Task GetChatById_NonExistingChat_ReturnsNotFound()
        {
            // Arrange
            var chatId = 1;
            _mockChatService.Setup(service => service.GetChatById(chatId)).ReturnsAsync((Chat)null);

            // Act
            var result = await _chatController.GetChatById(chatId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Chat with ID {chatId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetChats_ReturnsOkResultWithChatsList()
        {
            // Arrange
            var chats = new List<Chat>
            {
                new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" },
                new Chat { Id = 2, User1Id = "admin0", User2Id = "pepguardiola" }
            };
            _mockChatService.Setup(service => service.GetChats()).ReturnsAsync(chats);

            // Act
            var result = await _chatController.GetChats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChats = Assert.IsType<List<Chat>>(okResult.Value);
            Assert.Equal(chats.Count, returnedChats.Count);
        }

        [Fact]
        public async Task GetChatCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockChatService.Setup(service => service.GetChatCount()).ReturnsAsync(count);

            // Act
            var result = await _chatController.GetChatCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetChatIdBetweenUsers_ExistingChat_ReturnsOkResultWithChatId()
        {
            // Arrange
            var user1Id = "leomessi";
            var user2Id = "pepguardiola";
            var chatId = 1;
            _mockChatService.Setup(service => service.GetChatIdBetweenUsers(user1Id, user2Id)).ReturnsAsync(chatId);

            // Act
            var result = await _chatController.GetChatIdBetweenUsers(user1Id, user2Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChatId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(chatId, returnedChatId);
        }

        [Fact]
        public async Task CreateChat_ValidDto_ReturnsCreatedAtActionResultWithChat()
        {
            // Arrange
            var dto = new ChatCreateDTO { User1Id = "leomessi", User2Id = "pepguardiola" };
            var chat = new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" };

            _mockMapper.Setup(m => m.Map<Chat>(dto)).Returns(chat);
            _mockChatService.Setup(service => service.CreateChat(chat)).Returns(Task.CompletedTask);

            // Act
            var result = await _chatController.CreateChat(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedChat = Assert.IsType<Chat>(createdAtActionResult.Value);
            Assert.Equal(chat.Id, returnedChat.Id);
        }

        [Fact]
        public async Task DeleteChat_ExistingChat_ReturnsNoContent()
        {
            // Arrange
            var chatId = 1;
            var chat = new Chat { Id = 1, User1Id = "leomessi", User2Id = "pepguardiola" };
            _mockChatService.Setup(service => service.GetChatById(chatId)).ReturnsAsync(chat);
            _mockChatService.Setup(service => service.DeleteChat(chatId)).Returns(Task.CompletedTask);

            // Act
            var result = await _chatController.DeleteChat(chatId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteChat_NonExistingChat_ReturnsNotFound()
        {
            // Arrange
            var chatId = 1;
            _mockChatService.Setup(service => service.GetChatById(chatId)).ReturnsAsync((Chat)null);

            // Act
            var result = await _chatController.DeleteChat(chatId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Chat with ID {chatId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ExportChatsToCsv_ReturnsFileResult()
        {
            // Arrange
            var csvData = "Chat Id,User1 E-mail,User1 First Name,User1 Last Name,User2 E-mail,User2 First Name,User2 Last Name";
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));

            _mockChatService.Setup(repo => repo.ExportChatsToCsv()).ReturnsAsync(csvStream);

            // Act
            var result = await _chatController.ExportChatsToCsv();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("chats.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var resultCsvData = await reader.ReadToEndAsync();
            Assert.Equal(csvData, resultCsvData);
        }
    }
}