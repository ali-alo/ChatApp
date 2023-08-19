namespace ChatApp.Server.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
