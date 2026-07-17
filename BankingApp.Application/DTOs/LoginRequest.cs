using System.ComponentModel.DataAnnotations;

namespace BankingApp.Application.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);