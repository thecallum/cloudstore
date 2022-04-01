using DocumentServiceListener.UseCase.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentServiceListener.Services
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

        public async Task DeleteCache(User user)
        {
            var key = GetKey(user.Id);

            await _database.StringGetDeleteAsync(key);
        }

        private static string GetKey(Guid userId)
        {
            return $"usage:{userId}";
        }
    }
}
