using Akka.Actor;
using Akkounts.Domain;
using Akkounts.Web.Actors;
using Microsoft.AspNetCore.Mvc;

namespace Akkounts.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        //private readonly ILogger<AccountController> _logger;
        private readonly IActorRef _accountsPool;

        public AccountController(AccountsActorProvider accountsActorProvider)
        {
            _accountsPool = accountsActorProvider();
        }

        [HttpPost]
        public IActionResult Post([FromBody] Transaction t)
        {
            _accountsPool.Tell(t);
            return Accepted(t);
        }
    }
}