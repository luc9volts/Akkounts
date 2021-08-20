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
        private readonly IActorRef _router;

        public AccountController(AccountsActorProvider accountsActorProvider)
        {
            _router = accountsActorProvider.Router;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Transaction t)
        {
            _router.Tell(t);
            return Accepted(t);
        }
    }
}