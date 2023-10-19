using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using StackExchange.Redis;
using WalletTool.UI.Database;
using WalletTool.UI.Repositories;
using WalletTool.UI.Services;
using Azure.Security.KeyVault;
using Azure.Security.KeyVault.Secrets;
using WalletTool.UI;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    var keyVaultUrl = builder.Configuration["AzureKeyVault"];
    if (string.IsNullOrEmpty(keyVaultUrl))
    {
        throw new InvalidOperationException("No key vault Uri");
    }
    var tokenCredential = new DefaultAzureCredential();
    var secretClient = new SecretClient(new Uri(keyVaultUrl), tokenCredential);
    builder.Configuration.AddAzureKeyVault(secretClient, new CustomKeyVaultSecretManager());
}
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews();
var sqlConnectionString = builder.Configuration["ConnectionStrings:Database"];
if (string.IsNullOrEmpty(sqlConnectionString))
{
    throw new InvalidOperationException("Database connection string is missing or null");
}
builder.Services.AddDbContext<WalletContext>(options => 
    options.UseSqlServer(sqlConnectionString));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddSingleton<ICachingService, RedisCachingService>();

var redisConnectionString = builder.Configuration["ConnectionStrings:Redis"];
if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new InvalidOperationException("Redis connection string is missing or null");
}
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();