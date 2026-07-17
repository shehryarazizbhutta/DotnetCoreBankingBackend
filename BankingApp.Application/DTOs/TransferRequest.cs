using System.ComponentModel.DataAnnotations;

namespace BankingApp.Application.DTOs;

public record TransferRequest(

    [Required]
    string FromAccountNumber,

    [Required]
    string ToAccountNumber,

    [Range(typeof(decimal), "0.01", "999999999")]
    decimal Amount
);