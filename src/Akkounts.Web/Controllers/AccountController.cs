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
        private readonly IActorRef _mainActor;

        public AccountController(AccountsActorProvider accountsActorProvider)
        {
            _mainActor = accountsActorProvider.MainActor;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Transaction t)
        {
            _mainActor.Tell(t);
            return Accepted(t);
        }
    }
}