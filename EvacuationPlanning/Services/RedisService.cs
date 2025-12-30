using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Services;

public class RedisService
{
     private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private ILogger<RedisService> _logger;

    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;
    }

     private bool IsAvailable => _redis.IsConnected;

    public async Task SetHastSetCacheAsync(string key, HashEntry[] data, TimeSpan? cacheDuration = null)
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Redis is not available.");
            return;
        }
        _logger.LogInformation($"Setting cache for key: {key}");
        _db.KeyExpireAsync(key, cacheDuration ?? TimeSpan.FromHours(1));
        await _db.HashSetAsync(key, data);
    }

    public async Task UpdateHashSetCacheAsync(string key, HashEntry[] data)
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Redis is not available.");
            return;
        }
        _logger.LogInformation($"Updating cache for key: {key}");
        await _db.HashSetAsync(key, data);
    }

    public async Task<List<T>> GetHashSetCacheAsync<T>(string key) where T : class
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Redis is not available.");
            return null;
        }
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

    public async Task DeleteCacheAsync(string key)
    {
        if (!IsAvailable)
        {
            _logger.LogWarning("Redis is not available.");
            return;
        }
        _logger.LogInformation($"Deleting cache for key: {key}");
        await _db.KeyDeleteAsync(key);
    }
}