using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class GetDocumentUploadLinkResponse
    {
        public string UploadUrl { get; set; }
        public Guid DocumentId { get; set; }
    }
}
