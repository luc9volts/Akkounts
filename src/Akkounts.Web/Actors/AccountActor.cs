using System;
using Akka.Actor;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using Akkounts.Web.ActorsMessages;
using Akkounts.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Actors
{
    public class AccountActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly TransactionRepository _repository;

        public AccountActor(IHubContext<NotificationHub> hubContext, TransactionRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;
            Ready();
        }

        private void Ready()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(30));

            Receive<Credit>(txnMessage =>
            {
                SaveTransaction(txnMessage);

                NotifyClients(new
                {
                    txnMessage.Account,
                    txnMessage.Amount,
                    Balance = _repository.GetBalance(txnMessage.Account).Amount,
                    TxnAccepted = true
                });
            });

            Receive<Debit>(txnMessage =>
            {
                var balance = _repository.GetBalance(txnMessage.Account);

                if (balance.IsTransactionAllowed(txnMessage.Amount))
                    SaveTransaction(txnMessage);

                NotifyClients(new
                {
                    txnMessage.Account,
                    txnMessage.Amount,
                    Balance = balance.Amount,
                    TxnAccepted = balance.IsTransactionAllowed(txnMessage.Amount)
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
            _repository.Add(new Transaction
            {
                AccountNumber = txnMessage.Account,
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
            var account = Context.Self.Path.ToString();
            _hubContext.Clients.All.SendAsync("ReceiveIdleInfo", account.Substring(account.LastIndexOf('/') + 1));
        }
    }
}