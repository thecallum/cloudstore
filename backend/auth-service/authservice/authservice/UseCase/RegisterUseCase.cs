using authservice.Boundary.Request;
using authservice.Domain;
using authservice.Factories;
using authservice.Gateways;
using authservice.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IUserGateway _userGateway;

        public RegisterUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task<Guid> Execute(RegisterRequestObject requestObject, string hash)
        {
            var newUser = requestObject.ToDomain();

            newUser.Hash = hash;

            await _userGateway.RegisterUser(newUser);

            return newUser.Id;
        }
    }
}
