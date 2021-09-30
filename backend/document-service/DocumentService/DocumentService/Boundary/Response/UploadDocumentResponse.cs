using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class UploadDocumentResponse
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string S3Location { get; set; }
    }
}
