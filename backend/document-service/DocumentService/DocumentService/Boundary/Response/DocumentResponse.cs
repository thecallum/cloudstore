using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Response
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public Guid? DirectoryId { get; set; }
        public string Name { get; set; }
        public long FileSize { get; set; }
        public string S3Location { get; set; }
        public string Thumbnail { get; set; }
    }
}
