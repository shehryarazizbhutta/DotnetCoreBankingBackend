using BankingApp.Domain.Enums;
namespace BankingApp.Application.DTOs;

public record TransactionResponse(
    Guid Id,
    decimal Amount,
    TransactionType Type,
    string? ReferenceAccountNumber,
    DateTime CreatedAt
);