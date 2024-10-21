using Microsoft.AspNetCore.Mvc;
using EbanxApi.Services;
using System.Threading.Tasks;

namespace EbanxApi.Controllers
{
    [ApiController]
    [Route("")]
    public class BalanceController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public BalanceController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance([FromQuery] int account_id)
        {
            try
            {
                var balance = await _accountService.GetBalanceAsync(account_id);
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                // Return 404 Not Found with "0" in the body when account doesn't exist.
                return NotFound(ex.Message);
            }
        }
    }
}
