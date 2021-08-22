using System;
using Akka.Routing;

namespace Akkounts.Web.ActorsMessages
{
    public record TransactionMessage(string AccountNumber, decimal Amount, DateTime StartDate)
    : IConsistentHashable
    {
        public object ConsistentHashKey => AccountNumber;
    }
}