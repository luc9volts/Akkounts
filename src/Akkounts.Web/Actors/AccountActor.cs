using System;
using System.Linq;
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
                    Balance = GetBalance(txnMessage.Account),
                    TxnRefused = false
                });
            });

            Receive<Debit>(txnMessage =>
            {
                SaveTransaction(txnMessage);

                NotifyClients(new
                {
                    txnMessage.Account,
                    Balance = GetBalance(txnMessage.Account),
                    TxnRefused = false
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

        private decimal GetBalance(string accountNumber)
        {
            return _repository.GetAllBy(accountNumber).Sum(t => t.Amount);
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