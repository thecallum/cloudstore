using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class GetAllDirectoriesResponse
    {
        public List<DirectoryResponse> Directories { get; set; }
    }
}
