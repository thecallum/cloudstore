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
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserGateway _userGateway;

        public LoginUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task<User> Execute(string email)
        {
            var user = await _userGateway.GetUserByEmailAddress(email).ConfigureAwait(false);
            if (user == null) return null;

            return user.ToDomain();
        }
    }
}
