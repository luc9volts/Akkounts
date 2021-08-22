using Akka.Routing;

namespace Akkounts.Web.ActorsMessages
{
    public record InitMessage(string Account) : IConsistentHashable
    {
        public object ConsistentHashKey => Account;
    };
}