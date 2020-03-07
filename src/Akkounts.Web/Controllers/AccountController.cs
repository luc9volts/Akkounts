using System.Threading.Tasks;
using Akkounts.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Akkounts.Web.Model;
using Microsoft.AspNetCore.SignalR;

namespace Akkounts.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AccountController(ILogger<AccountController> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Transaction t)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveAccountsTransactions", t.Amount, t.AccountNumber);
            return Accepted(t);
        }
    }
}