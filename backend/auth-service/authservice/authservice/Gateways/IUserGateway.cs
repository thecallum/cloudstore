using authservice.Infrastructure;
using System;
using System.Threading.Tasks;

namespace authservice.Gateways
{
    public interface IUserGateway
    {
        Task<User> GetUserById(Guid id);
        Task<User> GetUserByEmail(string email);

        Task RegisterUser(User newUser);

        Task DeleteUser(Guid id);
    }
}