using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Domain
{
    public class Directory
    {
        public Guid UserId { get; set; }
        public Guid DirectoryId { get; set; }
        public string Name { get; set;}
        public Guid ParentDirectoryId { get; set; }
    }
}
