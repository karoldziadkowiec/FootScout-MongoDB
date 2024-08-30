using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class MessageService : IMessageService
    {
        private readonly MongoDBContext _dbContext;
        private readonly INewIdGeneratorService _newIdGeneratorService;

        public MessageService(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            _dbContext = dbContext;
            _newIdGeneratorService = newIdGeneratorService;
        }

        public async Task<IEnumerable<Message>> GetAllMessages()
        {
            return await _dbContext.MessagesCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<int> GetAllMessagesCount()
        {
            return (int)await _dbContext.MessagesCollection.CountDocumentsAsync(FilterDefinition<Message>.Empty);
        }

        public async Task<IEnumerable<Message>> GetMessagesForChat(int chatId)
        {
            return await _dbContext.MessagesCollection
                .Find(m => m.ChatId == chatId)
                .SortBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<int> GetMessagesForChatCount(int chatId)
        {
            return (int)await _dbContext.MessagesCollection
                .CountDocumentsAsync(m => m.ChatId == chatId);
        }

        public async Task<DateTime> GetLastMessageDateForChat(int chatId)
        {
            return await _dbContext.MessagesCollection
                .Find(m => m.ChatId == chatId)
                .SortByDescending(m => m.Timestamp)
                .Project(m => m.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<Message> SendMessage(MessageSendDTO dto)
        {
            var chat = await _dbContext.ChatsCollection.Find(C => C.Id == dto.ChatId).FirstOrDefaultAsync();
            var sender = await _dbContext.UsersCollection.Find(u => u.Id == dto.SenderId).FirstOrDefaultAsync();
            var receiver = await _dbContext.UsersCollection.Find(u => u.Id == dto.ReceiverId).FirstOrDefaultAsync();

            var message = new Message
            {
                Id = await _newIdGeneratorService.GenerateNewMessageId(),
                ChatId = dto.ChatId,
                Chat = chat,
                SenderId = dto.SenderId,
                Sender = sender,
                ReceiverId = dto.ReceiverId,
                Receiver = receiver,
                Content = dto.Content,
                Timestamp = DateTime.Now
            };

            await _dbContext.MessagesCollection.InsertOneAsync(message);
            return message;
        }

        public async Task DeleteMessage(int messageId)
        {
            var message = await _dbContext.MessagesCollection
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
                throw new ArgumentException($"No message found with ID {messageId}");

            await _dbContext.MessagesCollection.DeleteOneAsync(m => m.Id == messageId);
        }
    }
}