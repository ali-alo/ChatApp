using ChatApp.Server.Data;
using ChatApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context) => _context = context;

        public async Task AddMessage(Message message)
        {
            for (int i = 0; i < message.Tags.Count; i++)
            {
                var t = _context.Tags.FirstOrDefault(t => t.Name == message.Tags[i].Name.Trim().ToLower());
                if (t != null)
                    message.Tags[i] = t;
            }
            message.Content = message.Content.Trim();
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task AddTag(Tag tag)
        {
            tag.Name = tag.Name.Trim().ToLower();
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetAllMessagesAsync() => 
            await _context.Messages.OrderBy(m => m.Time).Include(m => m.Tags).ToListAsync();

        public async Task<List<Tag>> GetAllTagsAsync() => await _context.Tags.ToListAsync();

        public async Task<List<Message>> GetMessagesByTagsAsync(List<Tag> tags)
        {
            var messages = await _context.Messages.Include(t => t.Tags).ToListAsync();

            return messages
                .Where(m => m.Tags.Count == 0 || m.Tags.Any(t => tags.Any(tm => tm.Name == t.Name)))
                .ToList();
        }
    }
}
