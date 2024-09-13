using AutoMapper;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/chats")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserRepository _userRepository;
        private readonly INewIdGeneratorService _newIdGeneratorService;
        private readonly IMapper _mapper;

        public ChatController(IChatService chatService, IUserRepository userRepository, INewIdGeneratorService newIdGeneratorService, IMapper mapper)
        {
            _chatService = chatService;
            _userRepository = userRepository;
            _newIdGeneratorService = newIdGeneratorService;
            _mapper = mapper;
        }

        // GET: api/chats/:chatId
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatById(int chatId)
        {
            var chat = await _chatService.GetChatById(chatId);
            if (chat == null)
                return NotFound($"Chat with ID {chatId} not found.");

            return Ok(chat);
        }

        // GET: api/chats
        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var chats = await _chatService.GetChats();
            return Ok(chats);
        }

        // GET: api/chats/count
        [HttpGet("count")]
        public async Task<IActionResult> GetChatCount()
        {
            int count = await _chatService.GetChatCount();
            return Ok(count);
        }

        // GET: api/chats/between/:user1Id/:user2Id
        [HttpGet("between/{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetChatIdBetweenUsers(string user1Id, string user2Id)
        {
            var chatId = await _chatService.GetChatIdBetweenUsers(user1Id, user2Id);
            return Ok(chatId);
        }

        // POST: api/chats
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateDTO dto)
        {
            var chat = _mapper.Map<Chat>(dto);
            chat.Id = await _newIdGeneratorService.GenerateNewChatId();

            if (chat.User1Id is not null)
            {
                var user1 = await _userRepository.GetUser(chat.User1Id);
                chat.User1 = _mapper.Map<User>(user1);
            }

            if (chat.User2Id is not null)
            {
                var user2 = await _userRepository.GetUser(chat.User2Id);
                chat.User2 = _mapper.Map<User>(user2);
            }

            await _chatService.CreateChat(chat);
            return CreatedAtAction(nameof(GetChatById), new { chatId = chat.Id }, chat);
        }

        // DELETE: api/chats/:chatId
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            var chat = await _chatService.GetChatById(chatId);
            if (chat == null)
                return NotFound($"Chat with ID {chatId} not found.");

            await _chatService.DeleteChat(chatId);
            return NoContent();
        }

        // GET: api/chats/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportChatsToCsv()
        {
            var csvStream = await _chatService.ExportChatsToCsv();
            return File(csvStream, "text/csv", "chats.csv");
        }
    }
}