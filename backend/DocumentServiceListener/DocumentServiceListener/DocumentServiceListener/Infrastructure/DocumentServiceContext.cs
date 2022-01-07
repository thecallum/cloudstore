using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Infrastructure
{
    public class DocumentServiceContext : DbContext
    {
        public DocumentServiceContext(DbContextOptions<DocumentServiceContext> options)
         : base(options)
        {
        }

        public DbSet<DocumentDb> Documents { get; set; }
    }
}
