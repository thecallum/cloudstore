using System.Threading.Tasks;
using authservice.Gateways;
using authservice.UseCase.Interfaces;

namespace authservice.UseCase
{
    public class DeleteUseCase : IDeleteUseCase
    {
        private readonly IUserGateway _userGateway;

        public DeleteUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task Execute(string email)
        {
            await _userGateway.DeleteUser(email);
        }
    }
}