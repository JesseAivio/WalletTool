using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Models;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class UpdateTransactionAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    
    public UpdateTransactionAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }
    
    [Fact]
    public async Task UpdateTransactionAsync_ShouldUpdateTransaction_WhenTransactionExistsForTheUser()
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
        _transactionRepository.UpdateTransactionAsync(Arg.Any<Transaction>()).Returns(true);
        
        // Act
        var result = await _sut.UpdateTransactionAsync(transactionDto, userId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task UpdateTransactionAsync_ShouldNotUpdateTransaction_WhenTransactionDoesNotExistForTheUser()
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
        _transactionRepository.UpdateTransactionAsync(Arg.Any<Transaction>()).Returns(false);
        
        // Act
        var result = await _sut.UpdateTransactionAsync(transactionDto, userId);
        
        // Assert
        result.Should().BeFalse();
    }
}