using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace SampleMvcApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string ticketId, string user, string message)
        {
            await Clients.Group(ticketId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinTicket(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);
        }
    }
}