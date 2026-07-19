using BankingApp.Application.Interfaces;
using BankingApp.Application.Services;
using Moq;

namespace BankingApp.Tests.Helpers;

public static class TransferServiceTestHelper
{
    public static (
        TransferService service,
        Mock<IAccountRepository> accountRepository,
        Mock<ITransactionRepository> transactionRepository
    ) Create()
    {
        var accountRepository = new Mock<IAccountRepository>();

        var transactionRepository = new Mock<ITransactionRepository>();

        var service = new TransferService(
            accountRepository.Object,
            transactionRepository.Object
        );

        return (
            service,
            accountRepository,
            transactionRepository
        );
    }
}