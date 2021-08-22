using System.Threading.Tasks;
using Akkounts.Domain.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly TransactionRepository _repository;

        public NotificationHub(TransactionRepository repository)
        {
            _repository = repository;
        }

        public override async Task OnConnectedAsync()
        {
            ReloadAllAccounts();
            await base.OnConnectedAsync();
        }

        private void ReloadAllAccounts()
        {
            foreach (var acc in _repository.GetAccountList())
            {
                var accountBalance = _repository.GetBalance(acc);
                Clients.Caller.SendAsync("ReceiveTxnInfo", new
                {
                    Account = acc,
                    Amount = 0,
                    Balance = accountBalance.Amount,
                    TxnAccepted = true
                });
            };
        }
    }
}