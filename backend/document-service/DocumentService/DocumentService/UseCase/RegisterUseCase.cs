using System;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;

namespace DocumentService.UseCase
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