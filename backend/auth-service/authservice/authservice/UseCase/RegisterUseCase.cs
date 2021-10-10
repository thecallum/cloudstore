using System;
using System.Threading.Tasks;
using authservice.Boundary.Request;
using authservice.Factories;
using authservice.Gateways;
using authservice.Logging;
using authservice.UseCase.Interfaces;

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
            LogHelper.LogUseCase("RegisterUseCase");

            var newUser = requestObject.ToDomain();

            newUser.Hash = hash;

            await _userGateway.RegisterUser(newUser);

            return newUser.Id;
        }
    }
}