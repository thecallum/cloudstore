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
    public class CheckUseCase : ICheckUseCase
    {
        private readonly IUserGateway _userGateway;

        public CheckUseCase(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public async Task<User> Execute(string emailAddress)
        {
            var user = await _userGateway.GetUserByEmailAddress(emailAddress).ConfigureAwait(false);
            if (user == null) return null;

            return user.ToDomain();
        }
    }
}
