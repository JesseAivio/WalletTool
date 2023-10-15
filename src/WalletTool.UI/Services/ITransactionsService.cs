using WalletTool.UI.Models;

namespace WalletTool.UI.Services;

public interface ITransactionsService
{
    Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Guid userId, int currentPage, int pageSize, int? filterYear, int? filterMonth);
    Task<int> GetTotalCountAsync(Guid userId, int? filterYear, int? filterMonth);
    Task<List<int>> GetAvailableYearsAsync(Guid userId);
    Task<List<int>> GetAvailableMonthsAsync(Guid userId);
    Task<bool> AddTransactionAsync(TransactionDTO transactionDTO, Guid userId);
    Task<bool> UpdateTransactionAsync(TransactionDTO transactionDTO, Guid userId);
    Task<bool> DeleteTransactionAsync(Guid id, Guid userId);
}