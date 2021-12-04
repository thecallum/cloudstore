using System;
using System.Linq;
using System.Threading.Tasks;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using authservice.Logging;
using Microsoft.EntityFrameworkCore;

namespace authservice.Gateways
{
    public class UserGateway : IUserGateway
    {
        private readonly UserContext _userContext;

        public UserGateway(UserContext userContext)
        {
            _userContext = userContext;
        }

        public async Task DeleteUser(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "DeleteUser");

            var users = await _userContext.Users.ToListAsync();

            var user = await LoadUser(id);
            if (user == null) throw new UserNotFoundException(id.ToString());

            _userContext.Users.Remove(user);
            await _userContext.SaveChangesAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "GetUserById");

            return await LoadUser(id);
        }

        public async Task RegisterUser(User newUser)
        {
            LogHelper.LogGateway("UserGateway", "RegisterUser");

            var existingUser = await _userContext.Users
                .Where(x => x.Email.ToLower() == newUser.Email.ToLower())
                .SingleOrDefaultAsync();

            if (existingUser != null) throw new UserWithEmailAlreadyExistsException(newUser.Email);

            _userContext.Users.Add(newUser);

            await _userContext.SaveChangesAsync();
        }

        public async Task<User> LoadUser(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "LoadUser");

            var user = await _userContext.Users.FindAsync(id);

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            LogHelper.LogGateway("UserGateway", "GetUserByEmail");

            return await _userContext.Users
                .Where(x => x.Email.ToLower() == email.ToLower())
                .SingleOrDefaultAsync();
        }
    }
}