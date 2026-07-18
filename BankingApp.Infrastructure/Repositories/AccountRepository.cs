using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infrastructure.Repositories;

public class AccountRepository(AppDbContext db) : IAccountRepository
{
    public async Task AddAsync(Account account)
    {
        await db.Accounts.AddAsync(account);
        await db.SaveChangesAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(Guid accountId, Guid userId)
    {
        return await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == accountId &&
                    a.UserId == userId
                );
    }

    public async Task<List<Account>> GetAccountsByUserIdAsync(Guid userId)
    {
        return await db.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<Account?> GetAccountFromUserIdAndAccountNumberAsync(Guid userId, string accountNumber)
    {
        return await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber &&
                    a.UserId == userId
                );
    }

    public async Task<Account?> GetReceiverAccountAsync(string accountNumber)
    {
        return await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber
                );
    }
}