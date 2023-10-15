using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WalletTool.UI.Auth;
using WalletTool.UI.Database;
using WalletTool.UI.Models;
using WalletTool.UI.Services;

namespace WalletTool.UI.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly ITransactionsService _transactionsService;
    private readonly ICachingService _cachingService;

    public TransactionsController(ILogger<HomeController> logger, 
        ITransactionsService transactionsService, ICachingService cachingService)
    {
        _logger = logger;
        _transactionsService = transactionsService;
        _cachingService = cachingService;
    }

    public async Task<IActionResult> Index(int currentPage = 1, int? filterYear = null, int? filterMonth = null)
    {
        var userId = HttpContext.GetUserId();
        int pageSize = 10;

        var cacheKey = $"user-{userId}-transactions-page-{currentPage}-year-{filterYear}-month-{filterMonth}";
        
        var transactions = await _cachingService.GetAsync<IEnumerable<Transaction>?>(cacheKey);
        if (transactions is null)
        {
            transactions = await _transactionsService.GetFilteredTransactionsAsync(userId!.Value, currentPage, pageSize, filterYear, filterMonth);
            await _cachingService.SetAsync(cacheKey, transactions, TimeSpan.FromHours(1));
        }

        var totalCount = await _transactionsService.GetTotalCountAsync(userId!.Value, filterYear, filterMonth);
        var availableYears = await _transactionsService.GetAvailableYearsAsync(userId!.Value);
        var availableMonths = await _transactionsService.GetAvailableMonthsAsync(userId!.Value);

        TransactionViewModel viewModel = new TransactionViewModel
        {
            Transactions = transactions,
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            FilterYear = filterYear,
            AvailableYears = availableYears,
            FilterMonth = filterMonth,
            AvailableMonths = availableMonths
        };

        return View(viewModel);
        
    }

    [HttpPost]
    public async Task<IActionResult> AddTransaction(TransactionDTO transactionDTO)
    {
        var userId = HttpContext.GetUserId();
        var success = await _transactionsService.AddTransactionAsync(transactionDTO, userId!.Value);
    
        if (success)
        {
            var cacheKey = $"user-{userId}-transactions-page-{transactionDTO.LastPage}-year-{transactionDTO.FilterYear}-month-{transactionDTO.FilterYear}";
            await _cachingService.InvalidateAsync(cacheKey);
            return Ok(new { success = true });
        }

        return BadRequest(new { success = false });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTransaction(TransactionDTO transactionDTO)
    {
        var userId = HttpContext.GetUserId();
        var success = await _transactionsService.UpdateTransactionAsync(transactionDTO, userId!.Value);
        
        if (success)
        {
            var cacheKey = $"user-{userId}-transactions-page-{transactionDTO.LastPage}-year-{transactionDTO.FilterYear}-month-{transactionDTO.FilterYear}";
            await _cachingService.InvalidateAsync(cacheKey);
            return Ok(new { success = true });
        }

        return BadRequest(new { success = false });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteTransaction(Guid id, int currentPage, int? filterYear = null
        , int? filterMonth = null)
    {
        
        var userId = HttpContext.GetUserId();
        var success = await _transactionsService.DeleteTransactionAsync(id, userId!.Value);

        if (success)
        {
            int pageSize = 10;
            int totalCountAfterDelete = await _transactionsService.GetTotalCountAsync(userId.Value, filterYear, filterMonth);
            int totalPagesAfterDelete = (int)Math.Ceiling((double)totalCountAfterDelete / pageSize);
            for (int i = currentPage; i <= totalPagesAfterDelete; i++)
            {
                await _cachingService.InvalidateAsync($"user-{userId}-transactions-page-{i}-year-{filterYear}-month-{filterMonth}");
            }
        }

        return RedirectToAction("Index");
    }
}