using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Gateways
{
    public class UserGateway : IUserGateway
    {
        private readonly DocumentServiceContext _documentServiceContext;

        public UserGateway(DocumentServiceContext documentServiceContext)
        {
            _documentServiceContext = documentServiceContext;
        }

        public async Task DeleteUser(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "DeleteUser");

            var user = await LoadUser(id);
            if (user == null) throw new UserNotFoundException(id.ToString());

            _documentServiceContext.Users.Remove(user);
            await _documentServiceContext.SaveChangesAsync();
        }

        public async Task<UserDb> GetUserById(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "GetUserById");

            return await LoadUser(id);
        }

        public async Task RegisterUser(UserDb newUser)
        {
            LogHelper.LogGateway("UserGateway", "RegisterUser");

            var existingUser = await _documentServiceContext.Users
                .Where(x => x.Email.ToLower() == newUser.Email.ToLower())
                .SingleOrDefaultAsync();

            if (existingUser != null) throw new UserWithEmailAlreadyExistsException(newUser.Email);

            _documentServiceContext.Users.Add(newUser);

            await _documentServiceContext.SaveChangesAsync();
        }

        public async Task<UserDb> LoadUser(Guid id)
        {
            LogHelper.LogGateway("UserGateway", "LoadUser");

            var user = await _documentServiceContext.Users.FindAsync(id);

            return user;
        }

        public async Task<UserDb> GetUserByEmail(string email)
        {
            LogHelper.LogGateway("UserGateway", "GetUserByEmail");

            return await _documentServiceContext.Users
                .Where(x => x.Email.ToLower() == email.ToLower())
                .SingleOrDefaultAsync();
        }
    }
}