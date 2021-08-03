using System;
using Akka.Routing;

namespace Akkounts.Web.ActorsMessages
{
    public record TransactionMessage(string Account, decimal Amount, DateTime StartDate)
    : IConsistentHashable
    {
        public object ConsistentHashKey => Account;
    }
}