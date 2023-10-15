using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class GetAvailableYearsAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    public GetAvailableYearsAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }
    
    [Fact]
    public async Task GetAvailableYearsAsync_ShouldReturnEmptyList_WhenUserHasNoTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _transactionRepository.GetAvailableYearsAsync(
            userId).Returns(new List<int>());
        
        // Act
        var result = await _sut.GetAvailableYearsAsync(
            userId);
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvailableYearsAsync_ShouldReturnListOfYears_WhenUserHasTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        List<int> years = new() { 2023, 2022 };
        _transactionRepository.GetAvailableYearsAsync(
            userId).Returns(years);
        
        // Act
        var result = await _sut.GetAvailableYearsAsync(
            userId);
        
        // Assert
        result.Should().HaveCount(2).And.Contain(2023).And.Contain(2022);
    }
}