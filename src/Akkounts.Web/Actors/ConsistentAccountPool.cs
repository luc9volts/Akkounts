using Akka.Actor;
using Akkounts.Web.Domain;
using Akkounts.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Actors
{
    public class ConsistentAccountPool : ReceiveActor, IWithUnboundedStash
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public IStash Stash { get; set; }

        public ConsistentAccountPool(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
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

        private IActorRef GetChildActor(string actorName)
        {
            var child = Context.Child(actorName);

            return child is Nobody
                ? Context.ActorOf(Props.Create<AccountActor>(_hubContext), actorName)
                : child;
        }
    }
}