using DocumentServiceListener.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Tests
{
    public class InMemoryDb
    {
        private static DocumentServiceContext _context;

        public static DocumentServiceContext Instance
        {
            get
            {
                if (_context == null)
                {
                    DbContextOptionsBuilder<DocumentServiceContext> builder = new DbContextOptionsBuilder<DocumentServiceContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.ConfigureWarnings(options =>
                    {
                        options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });

                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

                    _context = new DocumentServiceContext(builder.Options);
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
