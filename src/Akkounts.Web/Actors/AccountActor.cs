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
        private readonly string _accountNumber;
        private readonly Balance _accountBalance;

        public AccountActor(IHubContext<NotificationHub> hubContext, TransactionRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;

            var account = Context.Self.Path.ToString();
            _accountNumber = account.Substring(account.LastIndexOf('/') + 1);
            _accountBalance = _repository.GetBalance(_accountNumber);

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
                var txnAccepted = _accountBalance.IsTransactionAllowed(txnMessage.Amount);

                if (txnAccepted)
                    SaveTransaction(txnMessage);

                NotifyClients(new
                {
                    txnMessage.Account,
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
            _hubContext.Clients.All.SendAsync("ReceiveIdleInfo", _accountNumber);
        }
    }
}