using Microsoft.AspNetCore.Mvc;
using EbanxApi.Services;

namespace EbanxApi.Controllers
{
    [ApiController]
    [Route("")]
    public class ResetController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ResetController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            _accountService.Reset();
            // Return 200 OK with empty body.
            return Ok();
        }
    }
}
