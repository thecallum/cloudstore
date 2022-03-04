using DocumentService.Domain;
using DocumentService.Middleware;
using DocumentService.Services;
using System;

namespace DocumentService.Tests.Helpers
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
            var tokenService = new TokenService("askdjhaskjdasjkldasjkd");

            return tokenService.CreateToken(user);
        }
    }
}
