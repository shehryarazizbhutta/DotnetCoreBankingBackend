using BankingApp.Application.DTOs;
using BankingApp.Application.Interfaces;
using BankingApp.Domain.Entities;
using BankingApp.Domain.Enums;

namespace BankingApp.Application.Services;

public class TransferService(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository
) : ITransferService
{
    public async Task TransferAsync(
        Guid userId,
        TransferRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new Exception("Amount must be greater than zero.");
        }

        if (request.FromAccountNumber == request.ToAccountNumber)
        {
            throw new Exception("Cannot transfer to the same account.");
        }

        var senderAccount =
            await accountRepository
                .GetAccountFromUserIdAndAccountNumberAsync(
                    userId,
                    request.FromAccountNumber
                );

        if (senderAccount is null)
        {
            throw new Exception("Sender account not found.");
        }


        var receiverAccount =
            await accountRepository
                .GetReceiverAccountAsync(
                    request.ToAccountNumber
                );

        if (receiverAccount is null)
        {
            throw new Exception("Receiver account not found.");
        }

        if (senderAccount.Balance < request.Amount)
        {
            throw new Exception("Insufficient balance.");
        }

        senderAccount.Balance -= request.Amount;
        receiverAccount.Balance += request.Amount;

        var senderTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = senderAccount.Id,
            Amount = request.Amount,
            Type = TransactionType.Transfer,
            ReferenceAccountNumber = receiverAccount.AccountNumber,
            CreatedAt = DateTime.UtcNow
        };

        var receiverTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = receiverAccount.Id,
            Amount = request.Amount,
            Type = TransactionType.Transfer,
            ReferenceAccountNumber = senderAccount.AccountNumber,
            CreatedAt = DateTime.UtcNow
        };

        await transactionRepository.TransferAsync(
            senderAccount,
            receiverAccount,
            senderTransaction,
            receiverTransaction
        );
    }
}