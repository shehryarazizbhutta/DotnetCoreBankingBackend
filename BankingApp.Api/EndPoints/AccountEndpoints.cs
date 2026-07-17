using BankingApp.Api.DTOs;
using BankingApp.Domain.Enums;
using BankingApp.Domain.Entities;
using BankingApp.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BankingApp.Infrastructure.Data;
namespace BankingApp.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("/accounts", async (AppDbContext db, HttpContext context) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var accounts = await db.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();

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
            AppDbContext db,
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


            db.Accounts.Add(account);

            await db.SaveChangesAsync();


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
            AppDbContext db,
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


            var account = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == id &&
                    a.UserId == userId
                );


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
            AppDbContext db,
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


            var account = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == id &&
                    a.UserId == userId
                );


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


            db.Transactions.Add(transaction);

            await db.SaveChangesAsync();

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
            AppDbContext db,
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


            var account = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.Id == id &&
                    a.UserId == userId
                );


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


            db.Transactions.Add(transaction);

            await db.SaveChangesAsync();

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


        app.MapPost("/accounts/transfer", async (
            TransferRequest request,
            AppDbContext db,
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


            if (request.FromAccountNumber == request.ToAccountNumber)
            {
                return Results.BadRequest("Cannot transfer to the same account.");
            }


            var senderAccount = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == request.FromAccountNumber &&
                    a.UserId == userId
                );


            if (senderAccount is null)
            {
                return Results.NotFound("Sender account not found.");
            }


            var receiverAccount = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == request.ToAccountNumber
                );
            if (receiverAccount is null)
            {
                return Results.NotFound("Receiver account not found.");
            }
            if (senderAccount.Balance < request.Amount)
            {
                return Results.BadRequest("Insufficient balance.");
            }
            await using var transaction =
                await db.Database.BeginTransactionAsync();
            try
            {
                senderAccount.Balance -= request.Amount;

                receiverAccount.Balance += request.Amount;


                var senderTransaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = senderAccount.Id,
                    Amount = request.Amount,
                    Type = TransactionType.Transfer,
                    ReferenceAccountNumber = receiverAccount.AccountNumber
                };


                var receiverTransaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = receiverAccount.Id,
                    Amount = request.Amount,
                    Type = TransactionType.Transfer,
                    ReferenceAccountNumber = senderAccount.AccountNumber
                };
                db.Transactions.Add(senderTransaction);
                db.Transactions.Add(receiverTransaction);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Results.Ok(new
                {
                    message = "Transfer successful"
                });
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return Results.BadRequest(ex.Message);
            }

        })
        .RequireAuthorization();


        app.MapGet("/accounts/{accountNumber}/transactions", async (
            string accountNumber,
            int page,
            int pageSize,
            AppDbContext db,
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


            var account = await db.Accounts
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber &&
                    a.UserId == userId
                );


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


            var totalTransactions = await db.Transactions
                .CountAsync(t => t.AccountId == account.Id);


            var transactions = await db.Transactions
                .Where(t => t.AccountId == account.Id)
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