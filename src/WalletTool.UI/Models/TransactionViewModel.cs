namespace WalletTool.UI.Models;

public class TransactionViewModel
{
    public IEnumerable<Transaction> Transactions { get; set; }
    
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;  
    public int TotalItems { get; set; }
    
    public int? FilterYear { get; set; }
    public List<int> AvailableYears { get; set; } 
    
    public int? FilterMonth { get; set; }
    public List<int> AvailableMonths { get; set; }
}