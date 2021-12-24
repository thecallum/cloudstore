using DocumentService.Infrastructure;
using System;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface IUserGateway
    {
        Task<UserDb> GetUserById(Guid id);
        Task<UserDb> GetUserByEmail(string email);

        Task RegisterUser(UserDb newUser);

        Task DeleteUser(Guid id);
    }
}