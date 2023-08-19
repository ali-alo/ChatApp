using ChatApp.Server.Models;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task UpdateTagsList(List<Tag> newTags)
        {
            await Clients.All.SendAsync("GetUpdatedTagsList", newTags);
        }
    }
}
