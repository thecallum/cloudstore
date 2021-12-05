using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase.Interfaces
{
    public interface IValidateUploadedDocumentUseCase
    {
        Task<DocumentDomain> Execute(Guid documentId, ValidateUploadedDocumentRequest request, User user);
    }
}
