using authservice.Domain;
using authservice.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Gateways
{
    public class UserGateway : IUserGateway
    {
        public Task DeleteUser(string email)
        {
            // find user by email
            // if null, throw UserNotFoundException() consumed in controller

            // delete user

            throw new NotImplementedException();
        }

        public Task<UserDb> GetUserByEmailAddress(string email)
        {
            // find user by email
            // if null, return null

            // return user.ToDomain()

            throw new NotImplementedException();
        }

        public Task RegisterUser(User newUser)
        {
            // find user by email
            // if not null, throw UserWithEmailExistsException(), consumed in controller

            // save user to database

            // return userId

            throw new NotImplementedException();
        }
    }
}
