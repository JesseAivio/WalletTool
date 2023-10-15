using FluentAssertions;
using NSubstitute;
using WalletTool.UI.Models;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit.TransactionService;

public class GetFilteredTransactionsAsyncTests
{
    private readonly TransactionsService _sut;
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    
    public GetFilteredTransactionsAsyncTests()
    {
        _sut = new TransactionsService(_transactionRepository);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldReturnEmptyList_WhenUserHasNoTransactions()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            nonExistentUserId, TestCurrentPage, TestPageSize, null, null).Returns(Enumerable.Empty<Transaction>());

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            nonExistentUserId, TestCurrentPage, TestPageSize, null, null);

        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldReturnTransactions_WhenUserHasTransactions()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var expectedTransactions = new[]
        {
            demoOne
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            null, null).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, null, null);

        // Assert
        result.Single().Should().BeEquivalentTo(demoOne);
        result.Should().BeEquivalentTo(expectedTransactions);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldOnlyReturnMatchingUserIdTransactions()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var SecondUserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var demoTwo = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(1),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoThree = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 3",
            Price = 20,
            Amount = 2,
            Date = DateTime.Now.AddDays(3),
            Type = TransactionType.Income,
            UserId = SecondUserId
        };
        var expectedTransactions = new[]
        {
            demoOne,
            demoTwo
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            null, null).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, null, null);

        // Assert
        result.Should().HaveCount(2)
            .And.Contain(demoOne)
            .And.Contain(demoTwo)
            .And.NotContain(demoThree);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldOnlyReturnMatchingFilteredYearAndFilteredMonthTransactions()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        var UserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var demoTwo = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(1),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoThree = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now,
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoFour = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 3",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddYears(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var expectedTransactions = new[]
        {
            demoThree
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            _dateTimeProvider.Now.Year, _dateTimeProvider.Now.Month).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, _dateTimeProvider.Now.Year, _dateTimeProvider.Now.Month);

        // Assert
        result.Should().HaveCount(1)
            .And.Contain(demoThree)
            .And.NotContain(demoOne)
            .And.NotContain(demoTwo)
            .And.NotContain(demoFour);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldOnlyReturnMatchingFilteredYearTransactions()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        var UserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var demoTwo = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(1),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoThree = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 3",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddYears(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var expectedTransactions = new[]
        {
            demoOne,
            demoTwo
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            _dateTimeProvider.Now.Year, null).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, _dateTimeProvider.Now.Year, null);

        // Assert
        result.Should().HaveCount(2)
            .And.Contain(demoOne)
            .And.Contain(demoTwo)
            .And.NotContain(demoThree);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldOnlyReturnMatchingFilteredMonthTransactions()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        var UserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var demoTwo = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(1),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoThree = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 3",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var expectedTransactions = new[]
        {
            demoOne,
            demoTwo
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            null, _dateTimeProvider.Now.Month).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, null, _dateTimeProvider.Now.Month);

        // Assert
        result.Should().HaveCount(2)
            .And.Contain(demoOne)
            .And.Contain(demoTwo)
            .And.NotContain(demoThree);
    }
    
    [Fact]
    public async Task GetFilteredTransactions_ShouldOnlyReturnCurrentPageAndItemsCountIsPageSizeTransactions()
    {
        // Arrange
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        var UserId = Guid.NewGuid();
        var demoOne = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 1",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(2),
            Type = TransactionType.Expense,
            UserId = UserId
        };
        var demoTwo = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 2",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddDays(1),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoThree = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 3",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoFour = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 4",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoFive = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 5",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoSix = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 6",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoSeven = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 7",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoEight = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 8",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoNine = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 9",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoTen = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 10",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var demoEleven = new Transaction()
        {
            Id = Guid.NewGuid(),
            Name = "Demo 11",
            Price = 20,
            Amount = 2,
            Date = _dateTimeProvider.Now.AddMonths(2),
            Type = TransactionType.Income,
            UserId = UserId
        };
        var expectedTransactions = new[]
        {
            demoOne,
            demoTwo,
            demoThree,
            demoFour,
            demoFive,
            demoSix,
            demoSeven,
            demoEight,
            demoNine,
            demoTen,
        };
        const int TestCurrentPage = 1;
        const int TestPageSize = 10;
        _transactionRepository.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, 
            null, null).Returns(expectedTransactions);

        // Act
        var result = await _sut.GetFilteredTransactionsAsync(
            UserId, TestCurrentPage, TestPageSize, null, null);

        // Assert
        result.Should().HaveCount(10)
            .And.Contain(demoOne)
            .And.Contain(demoTwo)
            .And.Contain(demoThree)
            .And.Contain(demoFour)
            .And.Contain(demoFive)
            .And.Contain(demoSix)
            .And.Contain(demoSeven)
            .And.Contain(demoEight)
            .And.Contain(demoNine)
            .And.Contain(demoTen)
            .And.NotContain(demoEleven);
    }
}