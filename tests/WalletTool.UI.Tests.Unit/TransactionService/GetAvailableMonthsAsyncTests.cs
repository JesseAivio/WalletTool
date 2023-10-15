using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class GetAvailableMonthsAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    public GetAvailableMonthsAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }
    
    [Fact]
    public async Task GetAvailableMonthsAsync_ShouldReturnEmptyList_WhenUserHasNoTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _transactionRepository.GetAvailableMonthsAsync(
            userId).Returns(new List<int>());
        
        // Act
        var result = await _sut.GetAvailableMonthsAsync(
            userId);
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableMonthsAsync_ShouldReturnListOfMonths_WhenUserHasTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        List<int> months = new() { 1, 2 };
        _transactionRepository.GetAvailableMonthsAsync(
            userId).Returns(months);
        
        // Act
        var result = await _sut.GetAvailableMonthsAsync(
            userId);
        
        // Assert
        result.Should().HaveCount(2).And.Contain(1).And.Contain(2);
    }
}