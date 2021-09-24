using System.Threading.Tasks;
using authservice.Domain;
using authservice.Infrastructure;

namespace authservice.Gateways
{
    public interface IUserGateway
    {
        Task<UserDb> GetUserByEmailAddress(string email);

        Task RegisterUser(User newUser);

        Task DeleteUser(string email);
    }
}