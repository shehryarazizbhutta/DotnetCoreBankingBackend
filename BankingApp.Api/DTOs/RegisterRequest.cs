using System.ComponentModel.DataAnnotations;

namespace BankingApp.Api.DTOs;


public record RegisterRequest(
    [Required] string FullName,
    [Required][EmailAddress] string Email,
    [Required] string Password
);