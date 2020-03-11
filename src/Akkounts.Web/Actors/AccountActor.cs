using System;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
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
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(50));

            Receive<Credit>(transaction =>
            {
                _repository.Add(new Transaction
                {
                    AccountNumber = transaction.Account,
                    Amount = 150
                });
                var sum = _repository.GetAllBy(transaction.Account).Sum(t => t.Amount);

                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions",
                    $"{Context.Self.Path} {sum}");
            });

            Receive<Debit>(transaction =>
            {
                _repository.Add(new Transaction
                {
                    AccountNumber = transaction.Account,
                    Amount = 100
                });
                var sum = _repository.GetAllBy(transaction.Account).Sum(t => t.Amount);

                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions",
                    $"{Context.Self.Path} {sum}");
            });

            Receive<ReceiveTimeout>(timeout =>
            {
                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", $"{Context.Self.Path} Fim");
                Context.Stop(Self);
            });
        }

        public abstract class TransactionMessage : IConsistentHashable
        {
            public string Account { get; }
            public decimal Amount { get; }

            protected TransactionMessage(string account, decimal amount)
            {
                Account = account;
                Amount = amount;
            }

            public object ConsistentHashKey => Account;
        }

        public class Credit : TransactionMessage
        {
            public Credit(string account, decimal amount) : base(account, amount)
            {
            }
        }

        public class Debit : TransactionMessage
        {
            public Debit(string account, decimal amount) : base(account, amount)
            {
            }
        }
    }
}