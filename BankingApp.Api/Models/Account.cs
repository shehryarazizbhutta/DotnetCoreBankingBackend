using BankingApp.Api.Enums;

namespace BankingApp.Api.Models;

public class Account
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public AccountType AccountType { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}