using System.Collections.Concurrent;
using EbanxApi.Models;

namespace EbanxApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly ConcurrentDictionary<int, Account> _accounts = new();

        public decimal GetBalance(int accountId)
        {
            if (_accounts.TryGetValue(accountId, out var account))
            {
                return account.Balance;
            }
            else
            {
                // Throw exception to indicate account not found.
                throw new KeyNotFoundException("0");
            }
        }

        public void Reset()
        {
            _accounts.Clear();
        }

        public Account Deposit(int accountId, decimal amount)
        {
            var account = _accounts.GetOrAdd(accountId, new Account { Id = accountId, Balance = 0 });
            account.Balance += amount;
            return account;
        }

        public Account Withdraw(int accountId, decimal amount)
        {
            if (_accounts.TryGetValue(accountId, out var account))
            {
                if (account.Balance >= amount)
                {
                    account.Balance -= amount;
                    return account;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds");
                }
            }
            else
            {
                // Throw exception to indicate account not found.
                throw new KeyNotFoundException("0");
            }
        }

        public (Account Origin, Account Destination) Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            lock (_accounts)
            {
                var originAccount = Withdraw(fromAccountId, amount);
                var destinationAccount = Deposit(toAccountId, amount);
                return (originAccount, destinationAccount);
            }
        }
    }
}
