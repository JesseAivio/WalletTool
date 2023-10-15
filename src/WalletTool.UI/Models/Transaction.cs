namespace WalletTool.UI.Models;

public class Transaction
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required double Price { get; set; }
    public required double Amount { get; set; }
    public required DateTime Date { get; set; }
    public required TransactionType Type { get; set; }
    public required Guid UserId { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}