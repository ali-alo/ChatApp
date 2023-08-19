using ChatApp.Server.Models;
using ChatApp.Server.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ChatController : ControllerBase
    {
        private readonly IApplicationRepository _repository;

        public ChatController(IApplicationRepository repository) => 
            _repository = repository;

        [HttpGet("GetAllMessages")]
        public async Task<List<Message>> GetAllMessagesAsync()
        {
            var messages = await _repository.GetAllMessagesAsync();
            return messages;
        }

        [HttpPost("SendMessage")]
        public async Task SendMessageAsync(Message message)
        {
            await _repository.AddMessage(message);
        }

        [HttpGet("GetAllTags")]
        public async Task<List<Tag>> GetAllTagsAsync() => await _repository.GetAllTagsAsync();

        [HttpPost("GetMessagesByTags")]
        public async Task<List<Message>> GetMessagesByTagsAsync(List<Tag> tags)
        {
            return await _repository.GetMessagesByTagsAsync(tags);
        }

        [HttpPost("CreateTag")]
        public async Task CreateTag(Tag tag)
        {
            await _repository.AddTag(tag);
        }
    }
}
