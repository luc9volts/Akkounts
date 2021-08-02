using Akka.Actor;
using Akka.DI.Core;
using Akkounts.Domain;
using Akkounts.Web.ActorsMessages;

namespace Akkounts.Web.Actors
{
    public class ConsistentAccountPool : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        public ConsistentAccountPool()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<Transaction>(txn =>
            {
                var message = new TransactionMessage(txn.AccountNumber, txn.Amount, txn.StartDate);                
                var child = GetChildActor(message.ConsistentHashKey.ToString());
                child.Tell(message);
            });
        }

        private static IActorRef GetChildActor(string actorName)
        {
            var child = Context.Child(actorName);
            
            return child is Nobody
                ? Context.ActorOf(Context.DI().Props<AccountActor>(), actorName)
                : child;
        }
    }
}