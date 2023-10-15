using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Models;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class AddTransactionAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    public AddTransactionAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldCreateTransaction_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var transactionDto = new TransactionDTO()
        {
            Id = id,
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(2),
            Type = 1
        };
        
        _transactionRepository.AddTransactionAsync(Arg.Any<Transaction>()).Returns(true);
        
        // Act
        var result = await _sut.AddTransactionAsync(transactionDto, userId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task AddTransactionAsync_ShouldNotCreateTransaction_WhenDataIsInValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var transactionDto = new TransactionDTO()
        {
            Id = id,
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(2),
            Type = 1
        };
        
        _transactionRepository.AddTransactionAsync(Arg.Any<Transaction>()).Returns(false);
        
        // Act
        var result = await _sut.AddTransactionAsync(transactionDto, userId);
        
        // Assert
        result.Should().BeFalse();
    }
}