using authservice.Boundary.Request;
using authservice.Domain;
using authservice.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase
{
    public class RegisterUseCase : IRegisterUseCase
    {
        public Task<Guid> Execute(RegisterRequestObject requestObject)
        {
            // call userGateway.RegisterUser();

            // return Id

            throw new NotImplementedException();
        }
    }
}
