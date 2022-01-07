using Amazon.S3;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests
{
    public class DatabaseFixture
    {
        private readonly UserMockWebApplicationFactory _factory;
        private readonly IHost _host;

        public ScopedContext GetContext()
        {
            return _factory.GetContext();
        }

        public DatabaseFixture()
        {
            EnsureEnvVarConfigured("SECRET", "abcdefg");

            _factory = new UserMockWebApplicationFactory();
            _host = _factory.CreateHostBuilder(null).Build();
            _host.Start();

            //Client = _factory.CreateClient();
        }


        public HttpClient Client { get; }

        public IAmazonS3 S3Client => _factory.S3Client;


        public string ValidFilePath => _factory.ValidFilePath;
        public string TooLargeFilePath => _factory.TooLargeFilePath;

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        //public async Task SetupTestData(UserDb user)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        db.Users.Add(user);
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task<UserDb> GetUserByEmail(string email)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var user = await db.Users.Where(x => x.Email == email).SingleOrDefaultAsync();

        //        return user;
        //    }
        //}

        //public async Task<UserDb> GetUserById(Guid id)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var user = await db.Users.FindAsync(id);

        //        return user;
        //    }
        //}

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }





        //public async Task<DirectoryDb> GetDirectoryById(Guid id)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var directory = await db.Directories.Where(x => x.Id == id).SingleOrDefaultAsync();

        //        return directory;
        //    }
        //}

        //public async Task<DocumentDb> GetDocumentById(Guid id)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var document = await db.Documents.Where(x => x.Id == id).SingleOrDefaultAsync();

        //        return document;
        //    }
        //}

        //public async Task SetupTestData(DocumentDb document)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        db.Documents.Add(document);
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task SetupTestData(DirectoryDb directory)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        db.Directories.Add(directory);
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task SetupTestData(IEnumerable<DirectoryDb> directories)
        //{
        //    foreach (var directory in directories)
        //    {
        //        await SetupTestData(directory);
        //    }
        //}

        //public async Task SetupTestData(IEnumerable<DocumentDb> documents)
        //{
        //    foreach (var document in documents)
        //    {
        //        await SetupTestData(document);
        //    }
        //}

        //public async Task<User> GetUserByEmail(string email)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var user = await db.Users.Where(x => x.Email == email).SingleOrDefaultAsync();

        //        return user;
        //    }
        //}

        //public async Task<User> GetUserById(Guid id)
        //{
        //    using (var ctx = GetContext())
        //    {
        //        var db = ctx.DB;

        //        var user = await db.Users.FindAsync(id);

        //        return user;
        //    }
        //}

    }

    [CollectionDefinition("Database collection", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
