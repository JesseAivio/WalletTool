using System.Text.Json;
using StackExchange.Redis;
using WalletTool.UI.Models;

namespace WalletTool.UI.Services;

public class RedisCachingService : ICachingService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCachingService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var cache = _redis.GetDatabase();
        var cachedData = await cache.StringGetAsync(key);
        if (cachedData.IsNullOrEmpty) return default(T);
        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan duration)
    {
        var cache = _redis.GetDatabase();
        await cache.StringSetAsync(key, JsonSerializer.Serialize(value), duration);
    }

    public async Task InvalidateAsync(string key)
    {
        var cache = _redis.GetDatabase();
        await cache.KeyDeleteAsync(key);
    }
}