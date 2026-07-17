using BankingApp.Domain.Enums;

namespace BankingApp.Application.DTOs;

public record AccountResponse(
    Guid Id,
    string AccountNumber,
    AccountType AccountType,
    decimal Balance,
    bool IsPrimary
);