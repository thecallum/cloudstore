using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure
{
    public class DocumentServiceContext : DbContext
    {
        public DocumentServiceContext(DbContextOptions<DocumentServiceContext> options)
         : base(options)
        {
        }

        public DbSet<DocumentDb> Documents { get; set; }

        public DbSet<DirectoryDb> Directories { get; set; }
        public DbSet<UserDb> Users { get; set; }

    }
}
