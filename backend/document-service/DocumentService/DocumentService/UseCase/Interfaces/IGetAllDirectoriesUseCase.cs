using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetAllDirectoriesUseCase
    {
        Task<GetAllDirectoriesResponse> Execute(Guid userId, GetAllDirectoriesQuery query);
    }
}
