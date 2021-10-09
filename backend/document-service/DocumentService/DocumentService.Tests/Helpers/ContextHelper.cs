using DocumentService.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Tests.Helpers
{
    public static class ContextHelper
    {
        public static Payload CreateUser(Guid? id = null)
        {
            return new Payload
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@email.com"
            };
        }

        public static string CreateToken(Guid id)
        {
            var tokenService = new TokenService.TokenService("askdjhaskjdasjkldasjkd");

            return tokenService.CreateToken(CreateUser(id));
        }
    }
}
