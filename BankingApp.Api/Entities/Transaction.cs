using BankingApp.Api.Enums;

namespace BankingApp.Api.Entities;

public class Transaction
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ReferenceAccountNumber { get; set; }
}