using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class DeleteTransactionAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    public DeleteTransactionAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldDeleteTransaction_WhenTransactionExistsForTheUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _transactionRepository.DeleteTransactionAsync(id, userId).Returns(true);
        
        // Act
        var result = await _sut.DeleteTransactionAsync(id, userId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteTransactionAsync_ShouldNotDeleteTransaction_WhenTransactionDoesntExistsForTheUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _transactionRepository.DeleteTransactionAsync(id, userId).Returns(false);
        
        // Act
        var result = await _sut.DeleteTransactionAsync(id, userId);
        
        // Assert
        result.Should().BeFalse();
    }
}