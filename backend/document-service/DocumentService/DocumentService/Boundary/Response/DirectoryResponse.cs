using System;

namespace DocumentService.Boundary.Response
{
    public class DirectoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentDirectoryId { get; set; }
    }
}
