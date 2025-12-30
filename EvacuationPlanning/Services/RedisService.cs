using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Services;

public class RedisService
{
    private readonly IDatabase _db;
    private ILogger<RedisService> _logger;

    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
    {
        _db = redis.GetDatabase();
        _logger = logger;
    }

    public async Task SetHastSetCacheAsync(string key, HashEntry[] data, TimeSpan? cacheDuration = null)
    {
        _logger.LogInformation($"Setting cache for key: {key}");
        _db.KeyExpireAsync(key, cacheDuration ?? TimeSpan.FromHours(1));
        await _db.HashSetAsync(key, data);
    }

    public async Task UpdateHashSetCacheAsync(string key, HashEntry[] data)
    {
        _logger.LogInformation($"Updating cache for key: {key}");
        await _db.HashSetAsync(key, data);
    }

    public async Task<List<T>> GetHashSetCacheAsync<T>(string key) where T : class
    {
        _logger.LogInformation($"Getting cache for key: {key}");

        var values = await _db.HashGetAllAsync(key);
        List<T> data = values
            .Select(v => JsonSerializer.Deserialize<T>((byte[])v.Value))
            .ToList();
        if (data == null)
        {
            return null;
        }
        return data;
    }
}