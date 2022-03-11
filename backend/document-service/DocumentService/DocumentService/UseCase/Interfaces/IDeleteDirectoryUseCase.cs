using DocumentService.Boundary.Request;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IDeleteDirectoryUseCase
    {
        Task Execute(DeleteDirectoryQuery query, User user);
    }
}
