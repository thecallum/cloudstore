using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public interface IDeleteDocumentUseCase
    {
        Task Execute(Guid userId, Guid documentId);
    }
}
