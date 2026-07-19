using BankingApp.Application.DTOs;

namespace BankingApp.Application.Interfaces;

public interface ITransferService
{
    Task TransferAsync(Guid userId, TransferRequest request);
}