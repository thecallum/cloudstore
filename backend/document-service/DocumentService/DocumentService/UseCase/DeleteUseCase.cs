using System;
using System.Threading.Tasks;
using DocumentService.Gateways;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;

namespace DocumentService.UseCase
{
    public class DeleteUseCase : IDeleteUseCase
    {
        private readonly IUserGateway _userGateway;

        public DeleteUseCase(IUserGateway userGateway)
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