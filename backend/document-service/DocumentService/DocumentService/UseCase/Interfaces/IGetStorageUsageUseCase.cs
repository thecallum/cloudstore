using DocumentService.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetStorageUsageUseCase
    {
        Task<StorageUsageResponse> Execute(User user);
    }
}
