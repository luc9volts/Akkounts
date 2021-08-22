using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;

namespace Akkounts.Web.Actors
{
    public class AccountRouter : ReceiveActor
    {
        private readonly DependencyResolver _props;

        public AccountRouter()
        {
            _props = DependencyResolver.For(Context.System);
            Ready();
        }

        private void Ready()
        {
            Receive<IConsistentHashable>(txn =>
            {
                GetChildActor(txn.ConsistentHashKey.ToString()).Forward(txn);
            });
        }

        private IActorRef GetChildActor(string actorName)
        {
            var child = Context.Child(actorName);

            return child is Nobody
                ? Context.ActorOf(_props.Props<AccountActor>(), actorName)
                : child;
        }
    }
}