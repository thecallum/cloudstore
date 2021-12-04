using System.Threading.Tasks;
using authservice.Factories;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.Logging;
using authservice.UseCase.Interfaces;

namespace authservice.UseCase
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserGateway _userGateway;

        public LoginUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task<User> Execute(string email)
        {
            LogHelper.LogUseCase("LoginUseCase");

            var user = await _userGateway.GetUserByEmail(email).ConfigureAwait(false);

            return user;
        }
    }
}