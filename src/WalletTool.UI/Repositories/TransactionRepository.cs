using Microsoft.EntityFrameworkCore;
using WalletTool.UI.Database;
using WalletTool.UI.Models;

namespace WalletTool.UI.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly WalletContext _context;

    public TransactionRepository(WalletContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(Guid userId, int currentPage, int pageSize, int? filterYear, int? filterMonth)
    {
        var transactionsQuery = _context.Transactions.Where(x => x.UserId == userId).AsQueryable();

        if (filterYear.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.Date.Year == filterYear.Value);
        }
        if (filterMonth.HasValue)
        {
            transactionsQuery = transactionsQuery.Where(t => t.Date.Month == filterMonth.Value);
        }

        return await transactionsQuery
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(Guid userId, int? filterYear, int? filterMonth)
    {
        var query = _context.Transactions.Where(t => t.UserId == userId).AsQueryable();

        if (filterYear.HasValue)
        {
            query = query.Where(t => t.Date.Year == filterYear.Value);
        }
        if (filterMonth.HasValue)
        {
            query = query.Where(t => t.Date.Month == filterMonth.Value);
        }

        return await query.CountAsync();
    }

    public async Task<List<int>> GetAvailableYearsAsync(Guid userId)
    {
        return await _context.Transactions.Where(x => x.UserId == userId)
            .Select(t => t.Date.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();
    }

    public async Task<List<int>> GetAvailableMonthsAsync(Guid userId)
    {
        return  await _context.Transactions.Where(x => x.UserId == userId)
            .Select(t => t.Date.Month)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();
    }

    public async Task<bool> AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTransactionAsync(Transaction transaction)
    {
        var item = await _context.Transactions.SingleOrDefaultAsync(x => x.Id == transaction.Id && x.UserId == transaction.UserId);
        
        if (item is not null)
        {
            item.Name = transaction.Name;
            item.Price = transaction.Price;
            item.Amount = transaction.Amount;
            item.Date = transaction.Date;
            item.Type =  transaction.Type;
            return await _context.SaveChangesAsync() > 0;
        }

        return false;
    }

    public async Task<bool> DeleteTransactionAsync(Guid id, Guid userId)
    {
        var item = await _context.Transactions.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (item is not null)
        {
            _context.Transactions.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }

        return false;
    }
}