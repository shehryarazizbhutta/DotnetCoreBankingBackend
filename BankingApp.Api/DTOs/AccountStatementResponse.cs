namespace BankingApp.Api.DTOs;

public record AccountStatementResponse(
    string AccountNumber,
    decimal Balance,
    IEnumerable<TransactionResponse> Transactions,
    int Page,
    int PageSize,
    int TotalTransactions
);