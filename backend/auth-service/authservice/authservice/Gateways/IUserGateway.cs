using authservice.Domain;
using authservice.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Gateways
{
    public interface IUserGateway
    {
        Task<UserDb> GetUserByEmailAddress(string email);

        Task<Guid> RegisterUser(User newUser);

        Task DeleteUser(string email);
    }
}
