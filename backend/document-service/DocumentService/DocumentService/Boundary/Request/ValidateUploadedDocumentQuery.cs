using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request
{
    public class ValidateUploadedDocumentQuery
    {
        [FromRoute(Name = "documentId")]
        public Guid DocumentId { get; set; }
    }
}
