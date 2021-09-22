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
;