using authservice.Boundary.Request;
using authservice.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase.Interfaces
{
    public interface IRegisterUseCase
    {
        Task<Guid> Execute(RegisterRequestObject requestObject, string hash);
    }
}
