using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request
{
    public class CreateDirectoryRequest
    {
        public string Name { get; set; }
        public Guid? ParentDirectoryId { get; set; } = null;
    }
}
