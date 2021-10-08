using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request
{
    public class ValidateUploadedDocumentRequest
    {
        public string FileName { get; set; }
        public Guid? DirectoryId { get; set; }
    }
}
