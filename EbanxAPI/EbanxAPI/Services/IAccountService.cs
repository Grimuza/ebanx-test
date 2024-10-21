using System.Threading.Tasks;
using EbanxApi.Models;

namespace EbanxApi.Services
{
    public interface IAccountService
    {
        Task<decimal> GetBalanceAsync(int accountId);
        Task ResetAsync();
        Task<Account> DepositAsync(int accountId, decimal amount);
        Task<Account> WithdrawAsync(int accountId, decimal amount);
        Task<(Account Origin, Account Destination)> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
    }
}
