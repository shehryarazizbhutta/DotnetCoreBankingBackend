using BankingApp.Api.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Api.Services;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
namespace BankingApp.Api.Endpoints;

using BankingApp.Api.Data;
using BankingApp.Domain.Enums;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (AppDbContext db, RegisterRequest request, AccountNumberService accountNumberService) =>
        {
            var exists = await db.Users.AnyAsync(user => user.Email == request.Email);

            if (exists)
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

            db.Users.Add(user);
            await db.SaveChangesAsync();

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

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                user.Id,
                user.FullName,
                user.Email
            });
        });

        app.MapPost("/login", async (AppDbContext db, LoginRequest request, JwtService jwt) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Results.BadRequest("Invalid credentials");

            var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValid)
                return Results.BadRequest("Invalid credentials");

            var token = jwt.GenerateToken(user);

            return Results.Ok(new
            {
                access_token = token
            });
        });
    }
}