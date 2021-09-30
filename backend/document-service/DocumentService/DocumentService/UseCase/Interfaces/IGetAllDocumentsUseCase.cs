using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IGetAllDocumentsUseCase
    {
        Task<IEnumerable<Document>> Execute(Guid userId);
    }
}
