using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Domain
{
    public class Document
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public Guid DirectoryId { get; set; }
        public string Name { get; set; }
        public long FileSize { get; set; }
        public string S3Location { get; set; }
        public string LinkToDocument => $"https://uploadfromcs.s3.eu-west-1.amazonaws.com/{S3Location}";
    }
}
