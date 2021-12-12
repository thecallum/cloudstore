using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Domain
{
    public class DocumentDomain
    {
        public Guid Id { get; set; }
        public Guid? DirectoryId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string S3Location { get; set; }
        public long FileSize { get; set; }
        public string Thumbnail { get; set; }
    }
}
