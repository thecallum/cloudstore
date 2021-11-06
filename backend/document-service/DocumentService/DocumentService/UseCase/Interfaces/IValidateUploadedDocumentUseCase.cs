using DocumentService.Boundary.Request;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase.Interfaces
{
    public interface IValidateUploadedDocumentUseCase
    {
        Task<Document> Execute(Guid documentId, ValidateUploadedDocumentRequest request, User user);
    }
}
