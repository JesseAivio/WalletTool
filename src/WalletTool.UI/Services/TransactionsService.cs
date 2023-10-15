using Microsoft.EntityFrameworkCore;
using WalletTool.UI.Database;
using WalletTool.UI.Models;
using WalletTool.UI.Repositories;

namespace WalletTool.UI.Services;

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionsService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Guid userId, int currentPage, int pageSize, int? filterYear, int? filterMonth)
    {
        return await _transactionRepository.GetFilteredTransactionsAsync(userId, currentPage, pageSize, filterYear, filterMonth);
    }

    public async Task<int> GetTotalCountAsync(Guid userId, int? filterYear, int? filterMonth)
    {
        return await _transactionRepository.GetTotalCountAsync(userId, filterYear, filterMonth);
    }

    public async Task<List<int>> GetAvailableYearsAsync(Guid userId)
    {
        return await _transactionRepository.GetAvailableYearsAsync(userId);
    }

    public async Task<List<int>> GetAvailableMonthsAsync(Guid userId)
    {
        return await _transactionRepository.GetAvailableMonthsAsync(userId);
    }

    public async Task<bool> AddTransactionAsync(TransactionDTO transactionDTO, Guid userId)
    {
        Transaction transaction = new()
        {
            Id = Guid.NewGuid(),
            Name = transactionDTO.Name,
            Price = transactionDTO.Price,
            Amount = transactionDTO.Amount,
            Date = transactionDTO.Date,
            Type = transactionDTO.Type == 0 ? TransactionType.Income : TransactionType.Expense,
            UserId = userId
        };
        return await _transactionRepository.AddTransactionAsync(transaction);
    }

    public async Task<bool> UpdateTransactionAsync(TransactionDTO transactionDTO, Guid userId)
    {
        Transaction transaction = new()
        {
            Id = transactionDTO.Id!.Value,
            Name = transactionDTO.Name,
            Price = transactionDTO.Price,
            Amount = transactionDTO.Amount,
            Date = transactionDTO.Date,
            Type = transactionDTO.Type == 0 ? TransactionType.Income : TransactionType.Expense,
            UserId = userId
        };
        return await _transactionRepository.UpdateTransactionAsync(transaction);
    }

    public async Task<bool> DeleteTransactionAsync(Guid id, Guid userId)
    {
        return await _transactionRepository.DeleteTransactionAsync(id, userId);
    }
}