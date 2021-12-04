using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using authservice.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace authservice.Tests
{
    public class UserMockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string _connection = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<UserContext>));

                services.AddDbContext<UserContext>(options =>
                {
                    if (_connection != null)
                    {
                        options.UseNpgsql(_connection);
                    }
                    else
                    {
                        options.UseInMemoryDatabase("integration");
                        options.ConfigureWarnings(warningOptions =>
                        {
                            warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                        });
                    }
                });

                var serviceProvider = services.BuildServiceProvider();
                InitialiseDB(serviceProvider);
            });
        }

        private static void InitialiseDB(ServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();

            dbContext.Database.EnsureCreated();
            dbContext.SaveChanges();
        }

        public ScopedContext GetContext()
        {
            return new ScopedContext(Services);
        }
    }


    public sealed class ScopedContext : IDisposable
    {
        private readonly IServiceScope _scope;

        public UserContext DB { get; private set; }

        public ScopedContext(IServiceProvider services)
        {
            _scope = services.CreateScope();
            DB = _scope.ServiceProvider.GetRequiredService<UserContext>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}