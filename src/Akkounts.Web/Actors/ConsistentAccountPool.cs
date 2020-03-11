using Akka.Actor;
using Akka.DI.Core;
using Akkounts.Domain;

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
            Receive<Transaction>(t =>
            {
                var message = t.Type.Equals(Transaction.TransactionType.Credit)
                    ? (AccountActor.TransactionMessage) new AccountActor.Credit(t.AccountNumber, t.Amount)
                    : new AccountActor.Debit(t.AccountNumber, t.Amount);
                
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