using BankingApp.Domain.Enums;

namespace BankingApp.Api.DTOs;

public record AccountResponse(
    Guid Id,
    string AccountNumber,
    AccountType AccountType,
    decimal Balance,
    bool IsPrimary
);