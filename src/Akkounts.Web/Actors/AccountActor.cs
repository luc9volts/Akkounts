using System;
using System.Linq;
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
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly TransactionRepository _repository;
        private readonly string _accountNumber;
        private readonly Balance _accountBalance;

        public AccountActor(IServiceProvider sp)
        {
            _scope = sp.CreateScope();
            _repository = _scope.ServiceProvider.GetRequiredService<TransactionRepository>();
            _hubContext = _scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
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

            Receive<InitMessage>(init =>
            {
                NotifyClients(new
                {
                    init.Account,
                    Amount = 0,
                    Balance = _accountBalance.Amount,
                    TxnAccepted = true
                });
            });

            Receive<TransactionMessage>(txnMessage =>
            {
                var txnAccepted = _accountBalance.IsTransactionAllowed(txnMessage.Amount);

                if (txnAccepted)
                    SaveTransaction(txnMessage);

                NotifyClients(new
                {
                    Account = txnMessage.AccountNumber,
                    txnMessage.Amount,
                    Balance = _accountBalance.Amount,
                    TxnAccepted = txnAccepted
                });
            });

            Receive<ReceiveTimeout>(timeout =>
            {
                NotifyClientsActorIdle();
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

        private void NotifyClients(object info)
        {
            _hubContext.Clients.All.SendAsync("ReceiveTxnInfo", info);
        }

        private void NotifyClientsActorIdle()
        {
            _hubContext.Clients.All.SendAsync("ReceiveIdleInfo", _accountNumber);
        }
    }
}