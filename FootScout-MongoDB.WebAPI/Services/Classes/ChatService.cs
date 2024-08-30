using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using System.Text;
using FootScout_MongoDB.WebAPI.DbManager;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.Services.Classes
{
    public class ChatService : IChatService
    {
        private readonly MongoDBContext _dbContext;

        public ChatService(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Chat> GetChatById(int chatId)
        {
            return await _dbContext.ChatsCollection
                .Find(c => c.Id == chatId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Chat>> GetChats()
        {
            return await _dbContext.ChatsCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<int> GetChatCount()
        {
            return (int)await _dbContext.ChatsCollection.CountDocumentsAsync(FilterDefinition<Chat>.Empty);
        }

        public async Task<int> GetChatIdBetweenUsers(string user1Id, string user2Id)
        {
            return await _dbContext.ChatsCollection
                .Find(c => (c.User1Id == user1Id && c.User2Id == user2Id) || (c.User1Id == user2Id && c.User2Id == user1Id))
                .Project(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateChat(Chat chat)
        {
            await _dbContext.ChatsCollection.InsertOneAsync(chat);
        }

        public async Task DeleteChat(int chatId)
        {
            var chat = await GetChatById(chatId);

            if (chat == null)
                throw new ArgumentException($"No chat found with ID {chatId}");

            await _dbContext.MessagesCollection
                .DeleteManyAsync(m => m.ChatId == chatId);

            await _dbContext.ChatsCollection
                .DeleteOneAsync(c => c.Id == chatId);
        }

        public async Task<MemoryStream> ExportChatsToCsv()
        {
            var chats = await GetChats();
            var csv = new StringBuilder();
            csv.AppendLine("Chat Id,User1 E-mail,User1 First Name,User1 Last Name,User2 E-mail,User2 First Name,User2 Last Name");

            foreach (var chat in chats)
            {
                csv.AppendLine($"{chat.Id},{chat.User1.Email},{chat.User1.FirstName},{chat.User1.LastName},{chat.User2.Email},{chat.User2.FirstName},{chat.User2.LastName}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var csvStream = new MemoryStream(byteArray);

            return csvStream;
        }
    }
}