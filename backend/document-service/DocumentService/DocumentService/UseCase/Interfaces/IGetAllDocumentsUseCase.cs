using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetAllDocumentsUseCase
    {
        Task<GetAllDocumentsResponse> Execute(Guid userId, GetAllDocumentsQuery query);
    }
}
