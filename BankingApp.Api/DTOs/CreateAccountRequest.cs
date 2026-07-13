using BankingApp.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Api.DTOs;

public record CreateAccountRequest(
    [Required] AccountType AccountType
);