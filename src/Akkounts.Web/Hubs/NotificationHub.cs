using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task Notify(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveAccountsTransactions", user, message);
        }

        // public override async Task OnConnectedAsync()
        // {
        //     await base.OnConnectedAsync();
        // }
    }
}