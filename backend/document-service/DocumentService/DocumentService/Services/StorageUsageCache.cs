using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DocumentService.Services
{
    public class StorageUsageCache : IStorageUsageCache
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public StorageUsageCache(IConnectionMultiplexer redis)
        {
            _redis = redis;

            _database = _redis.GetDatabase();
        }

        public async Task<long?> GetValue(Guid userId)
        {
            var key = GetKey(userId);

            var result = await _database.StringGetAsync(key);
            if (result.IsNull) return null;

            return long.Parse(result);
        }

        public async Task UpdateValue(Guid userId, long amount)
        {
            var key = GetKey(userId);

            var expiryTimeSpan = TimeSpan.FromHours(2);
                
            await _database.StringSetAsync(key, amount, expiryTimeSpan);
        }

        private static string GetKey(Guid userId)
        {
            return $"usage:{userId}";
        }
    }
}
