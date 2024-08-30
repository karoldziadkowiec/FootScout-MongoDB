using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_MongoDB.WebAPI.Controllers
{
    [Route("api/messages")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: api/messages
        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageService.GetAllMessages();
            return Ok(messages);
        }

        // GET: api/messages/count
        [HttpGet("count")]
        public async Task<IActionResult> GetAllMessagesCount()
        {
            int count = await _messageService.GetAllMessagesCount();
            return Ok(count);
        }

        // GET: api/messages/chat/:chatId
        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetMessagesForChat(int chatId)
        {
            var messages = await _messageService.GetMessagesForChat(chatId);
            return Ok(messages);
        }

        // GET: api/messages/chat/:chatId/count
        [HttpGet("chat/{chatId}/count")]
        public async Task<IActionResult> GetMessagesForChatCount(int chatId)
        {
            int count = await _messageService.GetMessagesForChatCount(chatId);
            return Ok(count);
        }

        // GET: api/messages/chat/:chatId/last-message-date
        [HttpGet("chat/{chatId}/last-message-date")]
        public async Task<IActionResult> GetLastMessageDateForChat(int chatId)
        {
            var lastMessageDate = await _messageService.GetLastMessageDateForChat(chatId);
            return Ok(lastMessageDate);
        }

        // DELETE: api/messages/:messageId
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                await _messageService.DeleteMessage(messageId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}