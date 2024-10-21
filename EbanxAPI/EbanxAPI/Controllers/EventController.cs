using Microsoft.AspNetCore.Mvc;
using EbanxApi.Models;
using EbanxApi.Services;
using System.Threading.Tasks;

namespace EbanxApi.Controllers
{
    [ApiController]
    [Route("")]
    public class EventController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public EventController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("event")]
        public async Task<IActionResult> CreateEvent([FromBody] Event evt)
        {
            try
            {
                switch (evt.Type)
                {
                    case EventType.Deposit:
                        if (evt.Destination.HasValue)
                        {
                            var account = await _accountService.DepositAsync(evt.Destination.Value, evt.Amount);
                            return Created(string.Empty, new
                            {
                                destination = new
                                {
                                    id = account.Id.ToString(),
                                    balance = account.Balance
                                }
                            });
                        }
                        return BadRequest("Destination account is required for deposit.");

                    case EventType.Withdraw:
                        if (evt.Origin.HasValue)
                        {
                            var account = await _accountService.WithdrawAsync(evt.Origin.Value, evt.Amount);
                            return Created(string.Empty, new
                            {
                                origin = new
                                {
                                    id = account.Id.ToString(),
                                    balance = account.Balance
                                }
                            });
                        }
                        return BadRequest("Origin account is required for withdrawal.");

                    case EventType.Transfer:
                        if (evt.Origin.HasValue && evt.Destination.HasValue)
                        {
                            var result = await _accountService.TransferAsync(evt.Origin.Value, evt.Destination.Value, evt.Amount);
                            return Created(string.Empty, new
                            {
                                origin = new
                                {
                                    id = result.Origin.Id.ToString(),
                                    balance = result.Origin.Balance
                                },
                                destination = new
                                {
                                    id = result.Destination.Id.ToString(),
                                    balance = result.Destination.Balance
                                }
                            });
                        }
                        return BadRequest("Both origin and destination accounts are required for transfer.");

                    default:
                        return BadRequest("Invalid event type.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                // Return 404 Not Found with "0" in the body when account doesn't exist.
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Return 400 Bad Request for invalid operations.
                return BadRequest(ex.Message);
            }
        }
    }
}
