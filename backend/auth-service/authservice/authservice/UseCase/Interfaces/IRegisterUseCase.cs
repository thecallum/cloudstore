using System;
using System.Threading.Tasks;
using authservice.Boundary.Request;

namespace authservice.UseCase.Interfaces
{
    public interface IRegisterUseCase
    {
        Task<Guid> Execute(RegisterRequestObject requestObject, string hash);
    }
}