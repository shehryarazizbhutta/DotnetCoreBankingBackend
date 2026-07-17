using System.ComponentModel.DataAnnotations;

namespace BankingApp.Application.DTOs;

public record AmountRequest(
    [Required][Range(typeof(decimal), "0.01", "999999999")]
    decimal Amount
);