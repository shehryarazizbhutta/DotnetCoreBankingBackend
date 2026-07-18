using BankingApp.Domain.Entities;

namespace BankingApp.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);
}