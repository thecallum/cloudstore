using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure
{
    public class DocumentUploadResponse
    {
        public string DocumentName { get; set; }
        public string S3Location { get; set; }
        public long FileSize { get; set; }
    }
}
