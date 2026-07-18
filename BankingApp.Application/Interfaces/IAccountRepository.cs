using BankingApp.Domain.Entities;

namespace BankingApp.Application.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account);

    Task<List<Account>> GetAccountsByUserIdAsync(Guid userId);

    Task<Account?> GetAccountByIdAsync(Guid accountId, Guid userId);

    Task<Account?> GetAccountFromUserIdAndAccountNumberAsync(Guid userId, string accountNumber); 

    Task<Account?> GetReceiverAccountAsync(string accountNumber);
}