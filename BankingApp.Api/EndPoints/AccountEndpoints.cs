using BankingApp.Application.DTOs;
using BankingApp.Domain.Enums;
using BankingApp.Domain.Entities;
using BankingApp.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BankingApp.Infrastructure.Data;
using BankingApp.Application.Interfaces;
namespace BankingApp.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("/accounts", async (IAccountRepository accountRepository, HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var accounts = await accountRepository.GetAccountsByUserIdAsync(userId);

            var response = accounts.Select(account => new AccountResponse(
                account.Id,
                account.AccountNumber,
                account.AccountType,
                account.Balance,
                account.IsPrimary
            ));

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPost("/accounts", async (
            CreateAccountRequest request,
            IAccountRepository accountRepository,
            HttpContext context,
            AccountNumberService accountNumberService) =>
        {
            var userIdClaim = context.User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);


            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountNumber = accountNumberService.Generate(),
                AccountType = request.AccountType,
                Balance = 0,
                IsPrimary = false
            };


            await accountRepository.AddAsync(account);

            return Results.Ok(new
            {
                account.AccountNumber,
                account.AccountType,
                account.Balance,
                account.IsPrimary
            });

        })
        .RequireAuthorization();

        app.MapGet("/accounts/{id}", async (
            Guid id,
            IAccountRepository accountRepository,
            HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);


            var account = await accountRepository.GetAccountByIdAsync(id, userId);


            if (account is null)
            {
                return Results.NotFound();
            }


            var response = new AccountResponse(
                account.Id,
                account.AccountNumber,
                account.AccountType,
                account.Balance,
                account.IsPrimary
            );


            return Results.Ok(response);

        })
        .RequireAuthorization();


        app.MapPost("/accounts/{id}/deposit", async (
            Guid id,
            AmountRequest request,
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var account = await accountRepository.GetAccountByIdAsync(id, userId);

            if (account is null)
            {
                return Results.NotFound();
            }

            account.Balance += request.Amount;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = request.Amount,
                Type = TransactionType.Deposit
            };

            await transactionRepository.DepositAsync(transaction);

            var response = new TransactionResponse(
                transaction.Id,
                transaction.Amount,
                transaction.Type,
                transaction.ReferenceAccountNumber,
                transaction.CreatedAt
            );

            return Results.Ok(response);

        })
        .RequireAuthorization();

        app.MapPost("/accounts/{id}/withdraw", async (
            Guid id,
            AmountRequest request,
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);


            var account = await accountRepository.GetAccountByIdAsync(id, userId);


            if (account is null)
            {
                return Results.NotFound();
            }

            if (account.Balance < request.Amount)
            {
                return Results.BadRequest("Insufficient balance.");
            }


            account.Balance -= request.Amount;


            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Amount = request.Amount,
                Type = TransactionType.Withdrawal
            };

            await transactionRepository.WithdrawAsync(transaction);

            var response = new TransactionResponse(
                transaction.Id,
                transaction.Amount,
                transaction.Type,
                transaction.ReferenceAccountNumber,
                transaction.CreatedAt
            );

            return Results.Ok(response);

        })
        .RequireAuthorization();


        app.MapPost("/accounts/transfer",
        async (
            TransferRequest request,
            ITransferService transferService,
            HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                await transferService.TransferAsync(
                    userId,
                    request
                );

                return Results.Ok(new
                {
                    message = "Transfer successful"
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }

        })
        .RequireAuthorization();


        app.MapGet("/accounts/{accountNumber}/transactions", async (
            string accountNumber,
            int page,
            int pageSize,
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var account = await accountRepository.GetAccountFromUserIdAndAccountNumberAsync(userId, accountNumber);

            if (account is null)
            {
                return Results.NotFound();
            }


            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                pageSize = 10;
            }

            var totalTransactions = await transactionRepository.GetTotalTransactionsByAccountIdAsync(account.Id);

            var transactions = await transactionRepository.GetTransactionsByAccountNumberAsync(account.Id, page, pageSize);

            var response = new AccountStatementResponse(
                account.AccountNumber,
                account.Balance,
                transactions,
                page,
                pageSize,
                totalTransactions
            );


            return Results.Ok(response);

        })
        .RequireAuthorization();


    }
}