using authservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Tests
{
    public class InMemoryDb
    {
        private static UserContext _context;

        public static UserContext Instance
        {
            get
            {
                if (_context == null)
                {
                    DbContextOptionsBuilder<UserContext> builder = new DbContextOptionsBuilder<UserContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.ConfigureWarnings(options =>
                    {
                        options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });

                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new UserContext(builder.Options);
                    _context.Database.EnsureCreated();
                }

                return _context;
            }
        }

        public static void Teardown()
        {
            _context = null;
        }
    }
}
