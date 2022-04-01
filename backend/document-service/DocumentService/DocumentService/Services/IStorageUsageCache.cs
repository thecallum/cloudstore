using System;
using System.Threading.Tasks;

namespace DocumentService.Services
{
    public interface IStorageUsageCache
    {
        Task<long?> GetValue(Guid userId);
        Task UpdateValue(Guid userId, long amount);
    }
}
