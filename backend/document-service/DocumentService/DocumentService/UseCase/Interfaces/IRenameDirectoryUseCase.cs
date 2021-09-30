using DocumentService.Boundary.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IRenameDirectoryUseCase
    {
        Task Execute(RenameDirectoryQuery query, RenameDirectoryRequest request, Guid userId);
    }
}
