using DocumentService.Boundary.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface ICreateDirectoryUseCase
    {
        Task<Guid> Execute(CreateDirectoryRequest request, Guid userId);
    }
}
