using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace DocumentServiceListener.Tests.Helpers
{
    public static class ContextHelper
    {
        public static User CreateUser(long? storageCapacity = long.MaxValue)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@email.com",
                StorageCapacity = (long)storageCapacity
            };
        }

        public static string CreateToken(User user)
        {
            var tokenService = new TokenService.TokenService("askdjhaskjdasjkldasjkd");

            return tokenService.CreateToken(user);
        }
    }
}
