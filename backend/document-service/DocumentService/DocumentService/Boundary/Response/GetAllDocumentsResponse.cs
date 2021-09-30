using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class GetAllDocumentsResponse
    {
        public List<Document> Documents { get; set; }
    }
}
