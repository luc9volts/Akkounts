using Akka.Actor;
using Akkounts.Domain;
using Akkounts.Domain.Abstract;
using Akkounts.Web.Actors;
using Akkounts.Web.ActorsMessages;
using Microsoft.AspNetCore.Mvc;

namespace Akkounts.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        //private readonly ILogger<AccountController> _logger;
        private readonly IActorRef _router;

        public AccountController(AccountsActorProvider accountsActorProvider, TransactionRepository repository)
        {
            _router = accountsActorProvider.Router;
            ReloadAllAccounts(repository);
        }

        private void ReloadAllAccounts(TransactionRepository rep)
        {
            foreach (var acc in rep.GetAccountList())
                _router.Tell(new InitMessage(acc));
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransactionMessage t)
        {
            _router.Tell(t);
            return Accepted(t);
        }
    }
}