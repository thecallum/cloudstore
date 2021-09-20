using authservice.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase
{
    public class DeleteUseCase : IDeleteUseCase
    {
        public Task Execute(string email)
        {
            // call userGateway.deleteUser

            throw new NotImplementedException();
        }
    }
}
