using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Api.Services;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
namespace BankingApp.Api.Endpoints;

using BankingApp.Infrastructure.Data;
using BankingApp.Domain.Enums;
using BankingApp.Application.Interfaces;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (IUserRepository userRepository, IAccountRepository accountRepository, RegisterRequest request, AccountNumberService accountNumberService) =>
        {
            var exists = await userRepository.GetByEmailAsync(request.Email);

            if (exists != null)
                return Results.BadRequest("Email already registered");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user);

            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                AccountNumber = accountNumberService.Generate(),
                Balance = 0,
                AccountType = AccountType.Current,
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow
            };

            await accountRepository.AddAsync(account);

            return Results.Ok(new
            {
                user.Id,
                user.FullName,
                user.Email
            });
        });

        app.MapPost("/login", async (IUserRepository userRepository, LoginRequest request, JwtService jwt) =>
        {
            var user = await userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Results.BadRequest("Invalid Email");

            var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValid)
                return Results.BadRequest("Invalid Password");

            var token = jwt.GenerateToken(user);

            return Results.Ok(new
            {
                access_token = token
            });
        });
    }
}