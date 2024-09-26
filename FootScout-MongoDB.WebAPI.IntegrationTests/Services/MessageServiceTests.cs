using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.IntegrationTests.TestManager;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;
using Moq;

namespace FootScout_MongoDB.WebAPI.IntegrationTests.Services
{
    public class MessageServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MongoDBContext _dbContext;
        private MessageService _messageService;

        public MessageServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            var _newIdGeneratorService = Mock.Of<INewIdGeneratorService>();

            _messageService = new MessageService(_dbContext, _newIdGeneratorService);
        }

        [Fact]
        public async Task GetAllMessages_ReturnsAllMessages()
        {
            // Arrange & Act
            var result = await _messageService.GetAllMessages();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.All(result, message =>
            {
                Assert.NotNull(message.Chat);
                Assert.NotNull(message.Sender);
                Assert.NotNull(message.Receiver);
            });
        }

        [Fact]
        public async Task GetAllMessagesCount_ReturnsCorrectCount()
        {
            // Arrange &  Act
            var result = await _messageService.GetAllMessagesCount();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetMessagesForChat_ReturnsMessagesForChat()
        {
            // Arrange
            var chatId = 1;

            // Act
            var result = await _messageService.GetMessagesForChat(chatId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, message => Assert.Equal(chatId, message.ChatId));
        }

        [Fact]
        public async Task GetMessagesForChatCount_ReturnsCorrectCount()
        {
            // Arrange
            var chatId = 1;

            // Act
            var result = await _messageService.GetMessagesForChatCount(chatId);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetLastMessageDateForChat_ReturnsCorrectLastMessageDate()
        {
            // Arrange
            var chatId = 2;
            var expectedDate = DateTime.Now.Date;

            // Act
            var result = await _messageService.GetLastMessageDateForChat(chatId);

            // Assert
            Assert.Equal(expectedDate, result.Date);
        }

        [Fact]
        public async Task SendMessage_AddsNewMessage()
        {
            // Arrange
            var messageSendDto = new MessageSendDTO
            {
                ChatId = 1,
                SenderId = "leomessi",
                ReceiverId = "pepguardiola",
                Content = "message"
            };

            // Act
            var result = await _messageService.SendMessage(messageSendDto);
            var savedMessage = await _dbContext.MessagesCollection.Find(m => m.Id == result.Id).FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(savedMessage);
            Assert.Equal(messageSendDto.ChatId, savedMessage.ChatId);
            Assert.Equal(messageSendDto.SenderId, savedMessage.SenderId);
            Assert.Equal(messageSendDto.ReceiverId, savedMessage.ReceiverId);
            Assert.Equal(messageSendDto.Content, savedMessage.Content);

            await _dbContext.MessagesCollection.DeleteOneAsync(m => m.Id == savedMessage.Id);
        }

        [Fact]
        public async Task DeleteMessage_RemovesMessage()
        {
            // Arrange
            var messageId = 1;

            // Act
            await _messageService.DeleteMessage(messageId);
            var result = await _dbContext.MessagesCollection.Find(m => m.Id == messageId).FirstOrDefaultAsync();

            // Assert
            Assert.Null(result);
        }
    }
}