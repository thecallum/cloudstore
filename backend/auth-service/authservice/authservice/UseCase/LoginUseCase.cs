using authservice.Domain;
using authservice.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase
{
    public class LoginUseCase : ILoginUseCase
    {
        public Task<User> Execute(string email)
        {
            // call userGateway.GTetUserByEmailAddress();

            // if null, return null

            // return user

            throw new NotImplementedException();
        }
    }
}
