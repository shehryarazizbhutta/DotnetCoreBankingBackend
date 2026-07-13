using System.ComponentModel.DataAnnotations;

namespace BankingApp.Api.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);