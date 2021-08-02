using System;
using Akka.Routing;

namespace Akkounts.Web.ActorsMessages
{
    public class TransactionMessage : IConsistentHashable
    {
        public string Account { get; }
        public decimal Amount { get; }
        public DateTime StartDate { get; }

        public TransactionMessage(string account, decimal amount, DateTime startDate)
        {
            Account = account;
            Amount = amount;
            StartDate = startDate;
        }

        public object ConsistentHashKey => Account;
    }
}