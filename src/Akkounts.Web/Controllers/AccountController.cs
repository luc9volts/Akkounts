using Akkounts.Web.Domain;
using Akka.Actor;
using Akkounts.Web.Actors;
using Microsoft.AspNetCore.Mvc;

namespace Akkounts.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        //private readonly ILogger<AccountController> _logger;
        private readonly IActorRef _accountsRouterActor;

        public AccountController(AccountsActorProvider accountsActorProvider)
        {
            //_logger = logger;
            _accountsRouterActor = accountsActorProvider();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Transaction t)
        {
            var message = t.Type.Equals(Transaction.TransactionType.Credit)
                ? (AccountActor.TransactionMessage) new AccountActor.Credit(t.AccountNumber, t.Amount)
                : new AccountActor.Debit(t.AccountNumber, t.Amount);

            _accountsRouterActor.Tell(message);

            //await _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", t.Amount, t.AccountNumber);
            return Accepted(t);
        }
    }
}