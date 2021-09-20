using authservice.Domain;
using authservice.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.UseCase
{
    public class CheckUseCase : ICheckUseCase
    {
        public Task<User> Execute(string emailAddress)
        {
            // call userGateway.GetUserByEmailAddress
            // if null, return null

            // return user

            throw new NotImplementedException();
        }
    }
}
