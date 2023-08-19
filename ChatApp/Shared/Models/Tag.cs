using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Message> Message { get; set; } = new List<Message>();
    }
}
