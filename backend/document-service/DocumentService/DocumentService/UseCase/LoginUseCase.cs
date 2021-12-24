using System.Threading.Tasks;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;

namespace DocumentService.UseCase
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserGateway _userGateway;

        public LoginUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task<UserDb> Execute(string email)
        {
            LogHelper.LogUseCase("LoginUseCase");

            var user = await _userGateway.GetUserByEmail(email).ConfigureAwait(false);

            return user;
        }
    }
}