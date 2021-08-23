using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using Akkounts.Web.ActorsMessages;
using Akkounts.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Akkounts.Web.Actors
{
    public class AccountActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private readonly IServiceScope _scope;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly TransactionRepository _repository;
        private readonly string _accountNumber;
        private readonly Balance _accountBalance;

        public AccountActor(IServiceProvider sp)
        {
            _scope = sp.CreateScope();
            _repository = _scope.ServiceProvider.GetRequiredService<TransactionRepository>();
            _hub = _scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
            _accountNumber = Context.Self.Path.ToString().Split('/').Last();
            _accountBalance = _repository.GetBalance(_accountNumber);
            
            Ready();
        }

        protected override void PostStop()
        {
            _scope.Dispose();
        }

        private void Ready()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(30));

            Receive<InitMessage>(async init =>
            {
                await NotifyClientsAsync(new
                {
                    init.Account,
                    Amount = 0,
                    Balance = _accountBalance.Amount,
                    TxnAccepted = true
                });
            });

            Receive<TransactionMessage>(async txnMessage =>
            {
                var txnAccepted = _accountBalance.IsTransactionAllowed(txnMessage.Amount);

                if (txnAccepted)
                    SaveTransaction(txnMessage);

                await NotifyClientsAsync(new
                {
                    Account = txnMessage.AccountNumber,
                    txnMessage.Amount,
                    Balance = _accountBalance.Amount,
                    TxnAccepted = txnAccepted
                });
            });

            Receive<ReceiveTimeout>(async timeout =>
            {
                await NotifyClientsActorIdleAsync();
                Context.Stop(Self);
            });
        }

        private void SaveTransaction(TransactionMessage txnMessage)
        {
            _accountBalance.Update(txnMessage.Amount);

            _repository.Add(new Transaction
            {
                AccountNumber = txnMessage.AccountNumber,
                Amount = txnMessage.Amount,
                StartDate = txnMessage.StartDate
            });
        }

        private async Task NotifyClientsAsync(object info) => await _hub.Clients.All.SendAsync("ReceiveTxnInfo", info);

        private async Task NotifyClientsActorIdleAsync() => await _hub.Clients.All.SendAsync("ReceiveIdleInfo", _accountNumber);
    }
}