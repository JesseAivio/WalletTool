using Microsoft.EntityFrameworkCore;
using WalletTool.UI.Models;

namespace WalletTool.UI.Database;

public class WalletContext : DbContext
{
    public WalletContext(DbContextOptions<WalletContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }
}