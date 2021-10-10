using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using authservice.Domain;
using authservice.Factories;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using authservice.Logging;

namespace authservice.Gateways
{
    public class UserGateway : IUserGateway
    {
        private readonly IDynamoDBContext _context;

        public UserGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task DeleteUser(string email)
        {
            LogHelper.LogGateway("UserGateway", "DeleteUser");

            var user = await LoadUser(email);
            if (user == null) throw new UserNotFoundException(email);

            await _context.DeleteAsync<UserDb>(email);
        }

        public async Task<UserDb> GetUserByEmailAddress(string email)
        {
            LogHelper.LogGateway("UserGateway", "GetUserByEmailAddress");

            return await LoadUser(email);
        }

        public async Task RegisterUser(User newUser)
        {
            LogHelper.LogGateway("UserGateway", "RegisterUser");

            var user = await LoadUser(newUser.Email);
            if (user != null) throw new UserWithEmailAlreadyExistsException(newUser.Email);

            await _context.SaveAsync(newUser.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<UserDb> LoadUser(string email)
        {
            LogHelper.LogGateway("UserGateway", "LoadUser");

            return await _context.LoadAsync<UserDb>(email).ConfigureAwait(false);
        }
    }
}