using Microsoft.AspNetCore.Mvc;
using EbanxApi.Services;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Reset()
        {
            await _accountService.ResetAsync();
            //return Ok("OK");
            return new ContentResult
            {
                Content = "OK",
                ContentType = "text/plain",
                StatusCode = 200
            };
        }
    }
}
