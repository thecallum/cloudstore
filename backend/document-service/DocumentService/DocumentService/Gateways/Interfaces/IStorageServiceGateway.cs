using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways.Interfaces
{
    public interface IStorageServiceGateway
    {
        Task<StorageUsageResponse> GetUsage(Guid userId, long accountStorageCapacity);
        Task<bool> CanUploadFile(Guid userId, long accountStorageCapacity, long fileSize, long? originalFileSize = null);
        Task AddFile(Guid userId, long fileSize);
        Task RemoveFile(Guid userId, long fileSize);
        Task ReplaceFile(Guid userId, long fileSize, long originalFileSize);
    }
}
