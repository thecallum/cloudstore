using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Boundary.Request
{
    public class GetAllDocumentsQuery
    {
        [FromQuery(Name = "pageSize")]
        public int? PageSize { get; set; } = 10;

        [FromQuery(Name = "page")]
        public int? Page { get; set; } = 0;
    }
}
