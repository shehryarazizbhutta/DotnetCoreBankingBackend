using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
namespace BankingApp.Application.Interfaces;


public interface ITransactionRepository
{
    Task DepositAsync(Transaction transaction);

    Task WithdrawAsync(Transaction transaction);

    Task TransferAsync(Account senderAccount, Account receiverAccount, Transaction senderTransaction, Transaction receiverTransaction);

    Task<int> GetTotalTransactionsByAccountIdAsync(Guid accountId);

    Task<List<TransactionResponse>> GetTransactionsByAccountNumberAsync(Guid accountId, int page, int pageSize);
}