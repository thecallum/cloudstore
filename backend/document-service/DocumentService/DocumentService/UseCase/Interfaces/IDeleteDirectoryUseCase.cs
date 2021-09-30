using DocumentService.Boundary.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IDeleteDirectoryUseCase
    {
        Task Execute(DeleteDirectoryQuery query, Guid userId);
    }
}
