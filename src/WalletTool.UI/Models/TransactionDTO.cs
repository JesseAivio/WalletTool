namespace WalletTool.UI.Models;

public class TransactionDTO
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; }
    public int CurrentPage { get; set; }
    public int LastPage { get; set; }
    public int? FilterYear { get; set; }
    public int? FilterMonth { get; set; }
}