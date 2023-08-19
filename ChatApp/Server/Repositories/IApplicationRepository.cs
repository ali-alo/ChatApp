using ChatApp.Server.Models;

namespace ChatApp.Server.Repositories
{
    public interface IApplicationRepository
    {
        public Task<List<Message>> GetAllMessagesAsync();
        public Task<List<Message>> GetMessagesByTagsAsync(List<Tag> tags);
        public Task AddMessage(Message message);
        public Task AddTag(Tag tag);
        public Task<List<Tag>> GetAllTagsAsync();
    }
}
