using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using userMS.Application.Services;
using userMS.Infrastructure.Com;

namespace userMS.Persistence.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly AppSettings _options;
        private readonly IDatabase _db;

        public RedisCacheService(IOptions<AppSettings> options)
        {
            _options = options.Value;

            var redis = ConnectionMultiplexer.Connect(_options.RedisHost);
            _db = redis.GetDatabase();
        }

        public async Task<bool> DeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);

            return true;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            // get value as redis string
            var value = await _db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public async Task<bool> HasKeyValue(string key)
        {
            var result = await _db.StringGetAsync(key);
            var value = !string.IsNullOrEmpty(result);

            return value;
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var val = JsonSerializer.Serialize(value);

            if (expiration != null)
            {
                await _db.StringSetAsync(key, val, expiration);

                return true;
            }

            await _db.StringSetAsync(key, val);

            return true;
            
        }
    }
}
