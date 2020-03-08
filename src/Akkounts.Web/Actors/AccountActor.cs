using System;
using Akka.Actor;
using Akka.Routing;
using Akkounts.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Actors
{
    public class AccountActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private readonly IHubContext<NotificationHub> _hubContext;

        public AccountActor(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
            Ready();
        }

        private void Ready()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(15));
            
            Receive<Credit>(transaction =>
            {
                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", transaction.Amount,
                    transaction.Account);
            });

            Receive<Debit>(transaction =>
            {
                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", transaction.Amount,
                    transaction.Account);
            });

            Receive<ReceiveTimeout>(timeout =>
            {
                _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", 0, "Fim");
                // shut self down
                Context.Stop(Self);
            });
        }

        public abstract class TransactionMessage : IConsistentHashable
        {
            public string Account { get; private set; }
            public decimal Amount { get; private set; }

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