using BankingApp.Application.DTOs;
using BankingApp.Domain.Entities;
using BankingApp.Tests.Helpers;
using Moq;

namespace BankingApp.Tests.Services;

public class TransferServiceTests
{
    [Fact]
    public async Task TransferAsync_WhenBalanceIsEnough_ShouldTransferSuccessfully()
    {
        // Arrange

        var (
            service,
            accountRepository,
            transactionRepository
        ) = TransferServiceTestHelper.Create();


        var senderAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "100001",
            Balance = 1000
        };

        var receiverAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "100002",
            Balance = 500
        };


        accountRepository
            .Setup(x => x.GetAccountFromUserIdAndAccountNumberAsync(
                It.IsAny<Guid>(),
                "100001"))
            .ReturnsAsync(senderAccount);


        accountRepository
            .Setup(x => x.GetReceiverAccountAsync("100002"))
            .ReturnsAsync(receiverAccount);


        transactionRepository
            .Setup(x => x.TransferAsync(
                It.IsAny<Account>(),
                It.IsAny<Account>(),
                It.IsAny<Transaction>(),
                It.IsAny<Transaction>()))
            .Returns(Task.CompletedTask);


        var request = new TransferRequest(
            "100001",
            "100002",
            200
        );


        // Act

        await service.TransferAsync(
            Guid.NewGuid(),
            request
        );


        // Assert

        Assert.Equal(800, senderAccount.Balance);

        Assert.Equal(700, receiverAccount.Balance);


        transactionRepository.Verify(
            x => x.TransferAsync(
                It.IsAny<Account>(),
                It.IsAny<Account>(),
                It.IsAny<Transaction>(),
                It.IsAny<Transaction>()),
            Times.Once
        );
    }


    [Fact]
    public async Task TransferAsync_WhenBalanceIsInsufficient_ShouldThrowException()
    {
        // Arrange

        var (
            service,
            accountRepository,
            _
        ) = TransferServiceTestHelper.Create();


        var senderAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "100001",
            Balance = 100
        };


        accountRepository
            .Setup(x => x.GetAccountFromUserIdAndAccountNumberAsync(
                It.IsAny<Guid>(),
                "100001"))
            .ReturnsAsync(senderAccount);


        var receiverAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "100002",
            Balance = 500
        };


        accountRepository
            .Setup(x => x.GetReceiverAccountAsync("100002"))
            .ReturnsAsync(receiverAccount);


        var request = new TransferRequest(
            "100001",
            "100002",
            200
        );


        // Act

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.TransferAsync(
                Guid.NewGuid(),
                request
            )
        );


        // Assert

        Assert.Equal(
            "Insufficient balance.",
            exception.Message
        );
    }


    [Fact]
    public async Task TransferAsync_WhenSenderAccountNotFound_ShouldThrowException()
    {
        // Arrange

        var (
            service,
            accountRepository,
            _
        ) = TransferServiceTestHelper.Create();


        accountRepository
            .Setup(x => x.GetAccountFromUserIdAndAccountNumberAsync(
                It.IsAny<Guid>(),
                "100001"))
            .ReturnsAsync((Account?)null);


        var request = new TransferRequest(
            "100001",
            "100002",
            100
        );


        // Act

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.TransferAsync(
                Guid.NewGuid(),
                request
            )
        );


        // Assert

        Assert.Equal(
            "Sender account not found.",
            exception.Message
        );
    }


    [Fact]
    public async Task TransferAsync_WhenReceiverAccountNotFound_ShouldThrowException()
    {
        // Arrange

        var (
            service,
            accountRepository,
            _
        ) = TransferServiceTestHelper.Create();


        var senderAccount = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = "100001",
            Balance = 1000
        };


        accountRepository
            .Setup(x => x.GetAccountFromUserIdAndAccountNumberAsync(
                It.IsAny<Guid>(),
                "100001"))
            .ReturnsAsync(senderAccount);


        accountRepository
            .Setup(x => x.GetReceiverAccountAsync("100002"))
            .ReturnsAsync((Account?)null);


        var request = new TransferRequest(
            "100001",
            "100002",
            100
        );


        // Act

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.TransferAsync(
                Guid.NewGuid(),
                request
            )
        );


        // Assert

        Assert.Equal(
            "Receiver account not found.",
            exception.Message
        );
    }


    [Fact]
    public async Task TransferAsync_WhenAccountsAreSame_ShouldThrowException()
    {
        // Arrange

        var (
            service,
            _,
            _
        ) = TransferServiceTestHelper.Create();


        var request = new TransferRequest(
            "100001",
            "100001",
            100
        );


        // Act

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.TransferAsync(
                Guid.NewGuid(),
                request
            )
        );


        // Assert

        Assert.Equal(
            "Cannot transfer to the same account.",
            exception.Message
        );
    }


    [Fact]
    public async Task TransferAsync_WhenAmountIsZero_ShouldThrowException()
    {
        // Arrange

        var (
            service,
            _,
            _
        ) = TransferServiceTestHelper.Create();


        var request = new TransferRequest(
            "100001",
            "100002",
            0
        );


        // Act

        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.TransferAsync(
                Guid.NewGuid(),
                request
            )
        );


        // Assert

        Assert.Equal(
            "Amount must be greater than zero.",
            exception.Message
        );
    }
}