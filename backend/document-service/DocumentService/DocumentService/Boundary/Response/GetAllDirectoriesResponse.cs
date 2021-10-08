using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class GetAllDirectoriesResponse
    {
        public List<Directory> Directories { get; set; }
    }
}
