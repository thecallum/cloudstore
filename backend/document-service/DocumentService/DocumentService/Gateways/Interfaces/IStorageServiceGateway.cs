using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Gateways.Interfaces
{
    public interface IStorageServiceGateway
    {
        Task<StorageUsageResponse> GetUsage(User user);
        Task<bool> CanUploadFile(User user, long fileSize, long? originalFileSize = null);
        Task AddFile(Guid userId, long fileSize);
        Task RemoveFile(Guid userId, long fileSize);
        Task ReplaceFile(Guid userId, long fileSize, long originalFileSize);
    }
}
