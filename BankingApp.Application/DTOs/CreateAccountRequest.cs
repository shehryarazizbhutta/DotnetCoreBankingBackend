using BankingApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingApp.Application.DTOs;

public record CreateAccountRequest(
    [Required] AccountType AccountType
);