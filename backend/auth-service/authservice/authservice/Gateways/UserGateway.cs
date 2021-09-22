using Amazon.DynamoDBv2.DataModel;
using authservice.Domain;
using authservice.Factories;
using authservice.Infrastructure;
using authservice.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var user = await LoadUser(email);
            if (user == null) throw new UserNotFoundException(email);

            await _context.DeleteAsync<UserDb>(email);
        }

        public async Task<UserDb> GetUserByEmailAddress(string email)
        {
            return await LoadUser(email);
        }

        public async Task RegisterUser(User newUser)
        {
            var user = await LoadUser(newUser.Email);
            if (user != null) throw new UserWithEmailAlreadyExistsException(newUser.Email);

            await _context.SaveAsync<UserDb>(newUser.ToDatabase()).ConfigureAwait(false);
        }

        public async Task<UserDb> LoadUser(string email)
        {
            return await _context.LoadAsync<UserDb>(email).ConfigureAwait(false);
        }
    }
}
