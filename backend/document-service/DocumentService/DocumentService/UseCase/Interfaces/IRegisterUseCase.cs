using System;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;

namespace DocumentService.UseCase.Interfaces
{
    public interface IRegisterUseCase
    {
        Task<Guid> Execute(RegisterRequestObject requestObject, string hash);
    }
}