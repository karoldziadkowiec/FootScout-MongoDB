using FootScout_MongoDB.WebAPI.Controllers;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_MongoDB.WebAPI.UnitTests.Controllers
{
    public class MessageControllerTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly MessageController _messageController;

        public MessageControllerTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _messageController = new MessageController(_mockMessageService.Object);
        }

        [Fact]
        public async Task GetAllMessages_ReturnsOkResultWithMessagesList()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Id = 1, ChatId = 1, Content = "Hello", SenderId = "leomessi", ReceiverId = "pepguardiola" },
                new Message { Id = 2, ChatId = 1, Content = "Hey", SenderId = "pepguardiola", ReceiverId = "leomessi"  }
            };
            _mockMessageService.Setup(service => service.GetAllMessages()).ReturnsAsync(messages);

            // Act
            var result = await _messageController.GetAllMessages();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsType<List<Message>>(okResult.Value);
            Assert.Equal(messages.Count, returnedMessages.Count);
        }

        [Fact]
        public async Task GetAllMessagesCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var count = 5;
            _mockMessageService.Setup(service => service.GetAllMessagesCount()).ReturnsAsync(count);

            // Act
            var result = await _messageController.GetAllMessagesCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetMessagesForChat_ReturnsOkResultWithMessagesList()
        {
            // Arrange
            var chatId = 1;
            var messages = new List<Message>
            {
                new Message { Id = 1, ChatId = 1, Content = "Hello", SenderId = "leomessi", ReceiverId = "pepguardiola" },
                new Message { Id = 2, ChatId = 1, Content = "Hey", SenderId = "pepguardiola", ReceiverId = "leomessi"  }
            };
            _mockMessageService.Setup(service => service.GetMessagesForChat(chatId)).ReturnsAsync(messages);

            // Act
            var result = await _messageController.GetMessagesForChat(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsType<List<Message>>(okResult.Value);
            Assert.Equal(messages.Count, returnedMessages.Count);
        }

        [Fact]
        public async Task GetMessagesForChatCount_ReturnsOkResultWithCount()
        {
            // Arrange
            var chatId = 1;
            var count = 10;
            _mockMessageService.Setup(service => service.GetMessagesForChatCount(chatId)).ReturnsAsync(count);

            // Act
            var result = await _messageController.GetMessagesForChatCount(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCount = Assert.IsType<int>(okResult.Value);
            Assert.Equal(count, returnedCount);
        }

        [Fact]
        public async Task GetLastMessageDateForChat_ReturnsOkResultWithDate()
        {
            // Arrange
            var chatId = 1;
            var lastMessageDate = DateTime.UtcNow;
            _mockMessageService.Setup(service => service.GetLastMessageDateForChat(chatId)).ReturnsAsync(lastMessageDate);

            // Act
            var result = await _messageController.GetLastMessageDateForChat(chatId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDate = Assert.IsType<DateTime>(okResult.Value);
            Assert.Equal(lastMessageDate, returnedDate);
        }

        [Fact]
        public async Task DeleteMessage_ExistingMessage_ReturnsNoContent()
        {
            // Arrange
            var messageId = 1;
            _mockMessageService.Setup(service => service.DeleteMessage(messageId)).Returns(Task.CompletedTask);

            // Act
            var result = await _messageController.DeleteMessage(messageId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMessage_NonExistingMessage_ReturnsNotFound()
        {
            // Arrange
            var messageId = 1;
            _mockMessageService.Setup(service => service.DeleteMessage(messageId))
                .ThrowsAsync(new ArgumentException("Message not found"));

            // Act
            var result = await _messageController.DeleteMessage(messageId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Message not found", notFoundResult.Value);
        }
    }
}