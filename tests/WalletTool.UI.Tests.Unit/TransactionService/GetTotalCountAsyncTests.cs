using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Models;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class GetTotalCountAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

    public GetTotalCountAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }
    
    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnZero_ForUnregisteredUserId()
    {
        // Arrange
        var unregisteredUserId = Guid.NewGuid();
        _transactionRepository.GetTotalCountAsync(
            unregisteredUserId, null, null).Returns(0);

        // Act
        var result = await _sut.GetTotalCountAsync(
            unregisteredUserId, null, null);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(0, null, null)]
    [InlineData(3, 2023, null)]
    [InlineData(10, null, 5)]
    [InlineData(100, 2022, 6)]
    public async Task GetTotalCountAsync_GivenVariousCounts_ShouldReturnMatchingCount(int count, int? filterYear, int? filterMonth)
    {
        // Arrange
        var userId = Guid.NewGuid();
        _transactionRepository.GetTotalCountAsync(
            userId, filterYear, filterMonth).Returns(count);

        // Act
        var result = await _sut.GetTotalCountAsync(
            userId, filterYear, filterMonth);

        // Assert
        result.Should().Be(count);
    }
}