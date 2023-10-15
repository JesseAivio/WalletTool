using WalletTool.UI.Models;

namespace WalletTool.UI.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Guid userId, int currentPage, int pageSize, int? filterYear, int? filterMonth);
    Task<int> GetTotalCountAsync(Guid userId, int? filterYear, int? filterMonth);
    Task<List<int>> GetAvailableYearsAsync(Guid userId);
    Task<List<int>> GetAvailableMonthsAsync(Guid userId);
    Task<bool> AddTransactionAsync(Transaction transaction);
    Task<bool> UpdateTransactionAsync(Transaction transaction);
    Task<bool> DeleteTransactionAsync(Guid id, Guid userId);
}