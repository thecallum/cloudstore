using DocumentService.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetStorageUsageUseCase
    {
        Task<GetStorageUsageResponse> Execute(Guid userId);
    }
}
