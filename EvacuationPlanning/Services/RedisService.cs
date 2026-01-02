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

    private bool isRedisAvailable(IConnectionMultiplexer multiplexer)
    {
        return multiplexer.IsConnected;
    }

    public async Task SetHastSetCacheAsync(string key, HashEntry[] data, TimeSpan? cacheDuration = null)
    {
        if (!isRedisAvailable(_redis))
        {
            _logger.LogError("Redis is not available. No set operation performed.");
            return;
        }

        try
        {
            _logger.LogInformation($"Setting cache for key: {key}.");
            _db.KeyExpireAsync(key, cacheDuration ?? TimeSpan.FromHours(1));
            await _db.HashSetAsync(key, data);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting cache. Exception: {ex.Message}.");
        }
    }

    public async Task UpdateHashSetCacheAsync(string key, HashEntry[] data)
    {
        if (!isRedisAvailable(_redis))
        {
            _logger.LogError("Redis is not available. No update operation performed.");
            return;
        }

        try
        {
            _logger.LogInformation($"Updating cache for key: {key}.");
            await _db.HashSetAsync(key, data);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating cache. Exception: {ex.Message}");
        }
    }

    public async Task<List<T>> GetHashSetCacheAsync<T>(string key) where T : class
    {
        List<T> data = new List<T>();
        if (!isRedisAvailable(_redis))
        {
            _logger.LogError("Redis is not available. No get operation performed.");
            return data;
        }

        try
        {
            _logger.LogInformation($"Getting cache for key: {key}.");
            HashEntry[] values = await _db.HashGetAllAsync(key);
            data = values
                .Select(v => JsonSerializer.Deserialize<T>((byte[])v.Value))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting cache. Exception: {ex.Message}.");
            return data;
        }

        return data;
    }

    public async Task DeleteCacheAsync(string key)
    {
        if (!isRedisAvailable(_redis))
        {
            _logger.LogError("Redis is not available. No delete operation performed.");
            return;
        }

        try
        {
            _logger.LogInformation($"Deleting cache for key: {key}.");
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting cache. Exception: {ex.Message}.");
        }
    }
}