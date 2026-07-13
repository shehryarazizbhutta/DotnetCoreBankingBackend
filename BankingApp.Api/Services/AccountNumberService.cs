namespace BankingApp.Api.Services;

public class AccountNumberService
{
    public string Generate()
    {
        var number = Random.Shared.Next(1, 999999999);

        return $"PK-001-{number:D10}";
    }
}