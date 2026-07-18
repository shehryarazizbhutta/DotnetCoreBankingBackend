using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankingApp.Infrastructure.Repositories;

public class TransactionRepository(AppDbContext db) : ITransactionRepository
{

    public async Task DepositAsync(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();
    }

    public async Task WithdrawAsync(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();
    }


    public async Task TransferAsync(Account senderAccount, Account receiverAccount, Transaction senderTransaction, Transaction receiverTransaction)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            db.Accounts.Update(senderAccount);
            db.Accounts.Update(receiverAccount);

            await db.Transactions.AddAsync(senderTransaction);
            await db.Transactions.AddAsync(receiverTransaction);

            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    

    public async Task<List<TransactionResponse>> GetTransactionsByAccountNumberAsync(Guid accountId, int page, int pageSize)
    {
        return await db.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse(
                    t.Id,
                    t.Amount,
                    t.Type,
                    t.ReferenceAccountNumber,
                    t.CreatedAt
                ))
                .ToListAsync();
    }

    public async Task<int> GetTotalTransactionsByAccountIdAsync(Guid accountId)
    {
        return await db.Transactions
                .CountAsync(t => t.AccountId == accountId);
    }
}