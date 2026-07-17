using BankingApp.Domain.Enums;
namespace BankingApp.Api.DTOs;

public record TransactionResponse(
    Guid Id,
    decimal Amount,
    TransactionType Type,
    string? ReferenceAccountNumber,
    DateTime CreatedAt
);