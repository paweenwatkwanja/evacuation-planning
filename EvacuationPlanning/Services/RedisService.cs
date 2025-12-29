using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Services;

public class RedisService
{
    private readonly IDistributedCache _cache;
    private const string EvacuationStatusKeyPrefix = "EvacuationStatus_";

    public RedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetCacheAsync(string key, object data, TimeSpan? cacheDuration = null)
    {
        Console.WriteLine($"Setting cache for key: {key}");
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration ?? TimeSpan.FromHours(1)
        };
        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, serializedData, options);
    }

    public async Task<T?> GetCacheAsync<T>(string key) where T : class
    {
        Console.WriteLine($"Getting cache for key: {key}");
        var serializedData = await _cache.GetStringAsync(key);
        if (serializedData == null)
        {
            return null;
        }
        return JsonSerializer.Deserialize<T>(serializedData);
    }
}