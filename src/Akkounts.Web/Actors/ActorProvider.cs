using Akka.Actor;

namespace Akkounts.Web.Actors
{
    public interface AccountsActorProvider
    {
        IActorRef Router { get; }
    }
}