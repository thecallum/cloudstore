using authservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace authservice.Tests
{
    public class DatabaseFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly UserMockWebApplicationFactory<TStartup> _factory;

        public ScopedContext GetContext()
        {
            return _factory.GetContext();
        }
        
  
        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("SECRET", "abcdefg");

            _factory = new UserMockWebApplicationFactory<TStartup>();
            Client = _factory.CreateClient();
        }

        public HttpClient Client { get; }


        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        public void Dispose()
        {
            InMemoryDb.Teardown();

        }

        public async Task SetupTestData(User user)
        {
            using (var ctx = GetContext())
            {
                var db = ctx.DB;

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var ctx = GetContext())
            {
                var db = ctx.DB;

                var user = await db.Users.Where(x => x.Email == email).SingleOrDefaultAsync();

                return user;
            }
        }

        public async Task<User> GetUserById(Guid id)
        {
            using (var ctx = GetContext())
            {
                var db = ctx.DB;

                var user = await db.Users.FindAsync(id);

                return user;
            }
        }


    }

    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}