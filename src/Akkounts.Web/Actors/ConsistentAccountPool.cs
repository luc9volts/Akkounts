using Akka.Actor;
using Akka.DI.Core;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using Akkounts.Web.ActorsMessages;

namespace Akkounts.Web.Actors
{
    public class ConsistentAccountPool : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private readonly TransactionRepository _repository;

        public ConsistentAccountPool(TransactionRepository repository)
        {
            _repository = repository;
            Ready();
        }

        protected override void PreStart()
        {
            ReloadAllAccounts();
        }

        protected override void PostRestart(System.Exception reason)
        {
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

        private void ReloadAllAccounts()
        {
            foreach (var acc in _repository.GetAccountList())
            {
                var child = GetChildActor(acc);
                child.Tell(new InitMessage(acc));
            }
       }
    }
}