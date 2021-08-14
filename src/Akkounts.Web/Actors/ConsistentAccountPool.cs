using System;
using System.Linq;
using Akka.Actor;
using Akka.DependencyInjection;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using Akkounts.Web.ActorsMessages;
using Microsoft.Extensions.DependencyInjection;

namespace Akkounts.Web.Actors
{
    public class ConsistentAccountPool : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }
        private readonly IServiceScope _scope;
        private readonly AccountsActorProvider _actorProvider;
        private readonly TransactionRepository _repository;
        private readonly DependencyResolver _props;

        public ConsistentAccountPool(IServiceProvider sp)
        {
            _scope = sp.CreateScope();
            _actorProvider = _scope.ServiceProvider.GetRequiredService<AccountsActorProvider>();
            _repository = _scope.ServiceProvider.GetRequiredService<TransactionRepository>();
            _props = DependencyResolver.For(Context.System);

            Ready();
        }

        protected override void PreStart()
        {
            ReloadAllAccounts();
        }

        protected override void PostRestart(System.Exception reason)
        {
        }

        protected override void PostStop()
        {
            _scope.Dispose();
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

        private IActorRef GetChildActor(string actorName)
        {
            var child = Context.Child(actorName);

            return child is Nobody
                ? Context.ActorOf(_props.Props<AccountActor>(), actorName)
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