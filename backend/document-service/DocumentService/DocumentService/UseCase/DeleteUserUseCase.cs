using System;
using System.Threading.Tasks;
using DocumentService.Gateways;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;

namespace DocumentService.UseCase
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserGateway _userGateway;

        public DeleteUserUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task Execute(Guid id)
        {
            LogHelper.LogUseCase("DeleteUseCase");

            await _userGateway.DeleteUser(id);
        }
    }
}