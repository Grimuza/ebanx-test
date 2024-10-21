using System.Collections.Concurrent;
using System.Threading.Tasks;
using EbanxApi.Models;

namespace EbanxApi.Services
{
    public class AccountService : IAccountService
    {
        // Thread-safe dictionary to store accounts in memory.
        private readonly ConcurrentDictionary<int, Account> _accounts = new();

        public Task<decimal> GetBalanceAsync(int accountId)
        {
            if (_accounts.TryGetValue(accountId, out var account))
            {
                return Task.FromResult(account.Balance);
            }
            else
            {
                return Task.FromException<decimal>(new KeyNotFoundException("0"));
            }
        }

        public Task ResetAsync()
        {
            _accounts.Clear();
            return Task.CompletedTask;
        }

        public Task<Account> DepositAsync(int accountId, decimal amount)
        {
            var account = _accounts.GetOrAdd(accountId, id => new Account { Id = id, Balance = 0 });
            lock (account.Lock)
            {
                DepositInternal(account, amount);
            }
            return Task.FromResult(account);
        }

        public Task<Account> WithdrawAsync(int accountId, decimal amount)
        {
            if (_accounts.TryGetValue(accountId, out var account))
            {
                lock (account.Lock)
                {
                    return WithdrawInternal(account, amount);
                }
            }
            else
            {
                return Task.FromException<Account>(new KeyNotFoundException("0"));
            }
        }

        public Task<(Account Origin, Account Destination)> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            if (!_accounts.TryGetValue(fromAccountId, out var originAccount))
            {
                return Task.FromException<(Account, Account)>(new KeyNotFoundException("0"));
            }

            var destinationAccount = _accounts.GetOrAdd(toAccountId, id => new Account { Id = id, Balance = 0 });

            // ORder by ID to avoid deadlocks
            var firstLock = fromAccountId < toAccountId ? originAccount : destinationAccount;
            var secondLock = fromAccountId < toAccountId ? destinationAccount : originAccount;

            lock (firstLock.Lock)
            {
                lock (secondLock.Lock)
                {
                    var withdrawalResult = WithdrawInternal(originAccount, amount);
                    if (withdrawalResult.IsFaulted)
                    {
                        return Task.FromException<(Account, Account)>(new InvalidOperationException("Insufficient funds"));
                    }

                    DepositInternal(destinationAccount, amount);

                    return Task.FromResult((originAccount, destinationAccount));
                }
            }
        }


        private void DepositInternal(Account account, decimal amount)
        {
            account.Balance += amount;
        }

        private Task<Account> WithdrawInternal(Account account, decimal amount)
        {
            if (account.Balance >= amount)
            {
                account.Balance -= amount;
                return Task.FromResult(account);
            }
            else
            {
                return Task.FromException<Account>(new InvalidOperationException("Insufficient funds"));
            }
        }
    }
}
