namespace WalletTool.UI.Services;

public interface ICachingService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan duration);
    Task InvalidateAsync(string key);
}