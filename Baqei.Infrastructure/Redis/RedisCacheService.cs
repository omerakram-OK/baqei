using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;

namespace Baqei.Infrastructure.Redis;

public class RedisCacheService
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
        _database = _multiplexer.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, System.TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var val = await _database.StringGetAsync(key);
        if (val.IsNullOrEmpty)
        {
            return default;
        }

        var result = JsonSerializer.Deserialize<T>(val!);
        return result;
    }
}
