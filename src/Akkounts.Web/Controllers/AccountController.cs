using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Akkounts.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Transaction t)
        {
            return Ok(t);
        }
    }
}