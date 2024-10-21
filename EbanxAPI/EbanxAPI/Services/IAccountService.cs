using EbanxApi.Models;

namespace EbanxApi.Services
{
    public interface IAccountService
    {
        decimal GetBalance(int accountId);
        void Reset();
        Account Deposit(int accountId, decimal amount);
        Account Withdraw(int accountId, decimal amount);
        (Account Origin, Account Destination) Transfer(int fromAccountId, int toAccountId, decimal amount);
    }
}
